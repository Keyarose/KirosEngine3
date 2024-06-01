using KirosEngine3.Config;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Textures
{
    public class Text : IDisposable
    {
        protected Font? _font;
        protected string _shaderName;//todo: set as default text shader
        protected string _text;
        protected Vec2 _pos;
        protected Matrix4 _scale;
        protected Matrix4 _rotation;

        protected SentenceData _sentence;
        protected int _VAO;
        protected int _VBO;
        protected int _EBO;

        protected bool _loaded = false;
        protected bool _disposed = false;
        protected bool _textChanged = false;

        protected bool _warnOnce = false;//flag to ensure that not loaded is logged only once instead of every frame

        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        /// <summary>
        /// The font to draw the text with
        /// </summary>
        public Font? Font { get { return _font; } set { _font = value; _textChanged = true; Update(); } }

        /// <summary>
        /// The shader to be used
        /// </summary>
        public string Shader { get { return _shaderName; } set { _shaderName = value; } }

        /// <summary>
        /// The text itself
        /// </summary>
        public string Sentence { get { return _text; } set { _text = value; _textChanged = true; Update(); } }

        /// <summary>
        /// The position of the text
        /// </summary>
        public Vec2 Position { get { return _pos; } set { _pos = value; } }

        /// <summary>
        /// Get or set the scale of the text.
        /// </summary>
        public Vec3 Scale 
        {
            get { return _scale.GetScale(); }
            set
            {
                _scale = Matrix4.CreateScale(value);
            }
        }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Color4 Color { get { return _sentence.Color; } set { _sentence.Color = value; } }

        /// <summary>
        /// The draw mode to be used during rendering.
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        /// <summary>
        /// Basic constructor for a Text object
        /// </summary>
        /// <param name="pos">The screen origin position of the text</param>
        public Text(Vec2 pos) : this(pos, string.Empty)
        {
        }

        public Text(Vec2 pos, string text)
        {
            if (!FontManager.TryGetFont(ConfigKeys.D_FONT_NAME_KEY, out _font))
            {
                Console.WriteLine("Warning: Default font is not configured.");
                Logger.WriteToLog("Warning: Default font is not configured.");
            }
            _shaderName = "text"; //todo: define environment var for default text shader
            _text = text;
            _pos = pos;

            _sentence = new SentenceData
            {
                Vertices = [],
                Indexes = [],
                Color = Color4.Black //default to black
            };
        }

        public Text(Vec2 pos, Font font, string text)
        {
            _font = font;
            _shaderName = "text";
            _text = text;
            _pos = pos;

            _sentence = font.TextForString(text, pos.AsVec3());
            _sentence.Color = Color4.Black;
        }

        #region Loading
        /// <summary>
        /// Initialize the text for rendering on it's own.
        /// </summary>
        /// <returns>True if init is successful, false otherwise.</returns>
        public bool Init()
        {
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            _VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex.SizeInBytesU * _sentence.Vertices.Length, _sentence.Vertices, BufferUsageHint.DynamicDraw);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _sentence.Indexes.Length, _sentence.Indexes, BufferUsageHint.DynamicDraw);

            //if the shader fails to be found it is logged in TryGetShader.
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return false;
            }

            //set the shader attributes for TexturedVertex
            sh.SetAttribsGL<TexturedVertex>();

            GL.BindVertexArray(0);

            UpdateSentence();

            //debug
            foreach (var v in _sentence.Vertices)
            {
                Console.WriteLine(v.ToString());
            }

            foreach (var v in _sentence.Indexes)
            {
                Console.WriteLine(v.ToString());
            }

            _loaded = true;
            return true;
        }
        #endregion

        /// <summary>
        /// Set the text to the given string
        /// </summary>
        /// <param name="text">The string to set the text to</param>
        public void SetText(string text)
        {
            Sentence = text;
        }

        #region Draw
        public void DrawGL(ViewMatrixes vm, TextureUnit tu)
        {
            if (!_loaded || _disposed)
            {
                if (!_warnOnce)
                {
                    Logger.WriteToLog("Attempt to draw unloaded Text object: {0}", this);
                    Console.WriteLine("Attempt to draw unloaded Text object: {0}", this);
                    //todo: write to debug
                    _warnOnce = true;
                }

                return;
            }

            //if use font fails or font is null
            if (!_font?.UseFont(tu) ?? false)
            {
                Logger.WriteToLog("Attempt to use non-existent font texture: {0}", _font);
                Console.WriteLine("Attempt to use non-existent font texture: {0}", _font);
                //todo: write to debug
            }

            //if the shader fails to be added to the pipeline it is logged in TryGetShader
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }
            sh.UseGL();
            sh.SetUniformIntGL("texture0", (int)tu);

            sh.SetUniformMat4GL("model", vm.Model * Matrix4.CreateScale(1f));
            sh.SetUniformMat4GL("view", vm.View);
            sh.SetUniformMat4GL("proj", vm.Orthographic);

            sh.SetUniformVec4GL("aTextColor", (Vec4)_sentence.Color);

            GL.BindVertexArray(_VAO);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DrawElements(_drawMode, _sentence.Indexes.Length, DrawElementsType.UnsignedInt, 0);

            GL.Disable(EnableCap.Blend);
            GL.BindVertexArray(0);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the sentence data for any changes in position or text
        /// </summary>
        private void UpdateSentence()
        {
            if (_font != null)
            {
                SentenceData ns = _font.TextForString(_text, _pos.AsVec3());

                _sentence.Vertices = ns.Vertices;
                _sentence.Indexes = ns.Indexes;

                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex.SizeInBytesU * _sentence.Vertices.Length, _sentence.Vertices, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _sentence.Indexes.Length, _sentence.Indexes, BufferUsageHint.DynamicDraw);

                _textChanged = false;
            }
        }

        public void Update()
        {
            if (_textChanged)
            {
                UpdateSentence();
            }
            //todo: handle updated position
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //clear managed items
                }

                GL.DeleteBuffer(_VBO);
                GL.DeleteBuffer(_EBO);
                GL.DeleteVertexArray(_VAO);
                _disposed = true;
            }
        }

        /// <summary>
        /// Release the text's resources for unloading
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Text()
        {
            Dispose(false);
        }
    }

    public struct SentenceData
    {
        public TexturedVertex[] Vertices;
        public uint[] Indexes;
        public Color4 Color;
    }
}
