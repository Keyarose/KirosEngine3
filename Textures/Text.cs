using KirosEngine3.Config;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.Textures
{
    /// <summary>
    /// A renderable Text object that uses a bitmap font.
    /// </summary>
    public class Text : IDisposable
    {
        /// <summary>
        /// The font to be used to generate the text.
        /// </summary>
        protected Font _font = FontManager.Default;

        /// <summary>
        /// The name of the shader to be used in rendering.
        /// </summary>
        protected string _shaderName = ShaderManager.DefaultTextShaderName ?? "";

        /// <summary>
        /// The text to be rendered to the screen.
        /// </summary>
        protected string _text = "";

        /// <summary>
        /// The screen position of the text's origin.
        /// </summary>
        protected Vec2 _pos;

        /// <summary>
        /// The scale of the text.
        /// </summary>
        protected Matrix4 _scale = Matrix4.CreateScale(1.0f);

        /// <summary>
        /// The rotation of the text.
        /// </summary>
        protected Matrix4 _rotation;

        /// <summary>
        /// The render data for the text.
        /// </summary>
        protected SentenceData _sentence;

        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected int _VAO;

        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        protected int _VBO;

        /// <summary>
        /// The element/index buffer object.
        /// </summary>
        protected int _EBO;

        /// <summary>
        /// Flag that shows if the Text has been loaded.
        /// </summary>
        protected bool _loaded = false;

        /// <summary>
        /// Flag that shows if the Text has been unloaded.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// Flag that shows if the text has been changed the data needs to be updated.
        /// </summary>
        protected bool _textChanged = false;

        private bool _warnOnce = false;//flag to ensure that not loaded is logged only once instead of every frame

        /// <summary>
        /// The draw mode to be used in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        /// <summary>
        /// The buffer usage mode to use when rendering the text.
        /// </summary>
        protected BufferUsageHint _bufferUse = BufferUsageHint.StaticDraw;

        /// <summary>
        /// The font to draw the text with
        /// </summary>
        public Font Font { get { return _font; } set { _font = value; _textChanged = true; Update(); } }

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
        /// The vertices of the text object.
        /// </summary>
        public TexturedVertex2D[] Vertices { get { return _sentence.Vertices; } }

        /// <summary>
        /// The indices of the text object.
        /// </summary>
        public uint[] Indices { get { return _sentence.Indexes; } }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Color4 Color { get { return _sentence.Color; } set { _sentence.Color = value; } }

        /// <summary>
        /// The draw mode to be used during rendering.
        /// </summary>
        public PrimitiveType DrawMode { get { return _drawMode; } set { _drawMode = value; } }

        /// <summary>
        /// The buffer usage mode to use when rendering the text.
        /// </summary>
        public BufferUsageHint BufferUsage { get { return _bufferUse; } set { _bufferUse = value; UpdateBuffers(); } }

        /// <summary>
        /// Basic constructor for XML use.
        /// </summary>
        public Text() { }

        /// <summary>
        /// Basic constructor for a Text object
        /// </summary>
        /// <param name="pos">The screen origin position of the text</param>
        public Text(Vec2 pos) : this(pos, string.Empty)
        {
        }

        /// <summary>
        /// Constructor for a Text object.
        /// </summary>
        /// <param name="pos">The screen origin position of the text.</param>
        /// <param name="text">The text to be rendered.</param>
        public Text(Vec2 pos, string text)
        {
            _shaderName = ShaderManager.DefaultTextShaderName ?? "";
            _text = text;
            _pos = pos;

            _sentence = _font.TextForString(text, Vec2.Zero);
            _sentence.Color = Color4.Black;
        }

        /// <summary>
        /// Constructor for a Text object.
        /// </summary>
        /// <param name="pos">The screen origin position of the text.</param>
        /// <param name="font">The font for the text to be rendered in.</param>
        /// <param name="text">The text to be rendered.</param>
        public Text(Vec2 pos, Font font, string text)
        {
            _font = font;
            _shaderName = ShaderManager.DefaultTextShaderName ?? "";
            _text = text;
            _pos = pos;

            _sentence = _font.TextForString(text, Vec2.Zero);
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
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * _sentence.Vertices.Length, _sentence.Vertices, _bufferUse);

            _EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _sentence.Indexes.Length, _sentence.Indexes, _bufferUse);

            //if the shader fails to be found it is logged in TryGetShader.
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return false;
            }

            //set the shader attributes for TexturedVertex2D
            sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
            sh.SetUVAttribGL(new ShaderAttribSettings { Offset = TexturedVertex2D.UVOffset, Size = 2, Stride= TexturedVertex2D.SizeInBytesU });
            //sh.SetAttribsGL<TexturedVertex>();

            GL.BindVertexArray(0);

            Update();

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
        /// <summary>
        /// DrawGL the text using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to be used in rendering.</param>
        /// <param name="tu">The texture unit to be used in rendering.</param>
        public void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
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

            //todo: move use font call up to the caller of this method and group elements using the same font to render in a batch
            //if use font fails or font is null
            if (!_font?.UseFont(tu) ?? false)
            {
                Logger.WriteToLog("Attempt to use non-existent font: {0}", _font);
                Console.WriteLine("Attempt to use non-existent font: {0}", _font);
                //todo: write to debug
            }

            //if the shader fails to be added to the pipeline it is logged in TryGetShader
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }

            //todo: font paging need more work
            sh.UseGL();
            //set the shader uniforms
            string[] shTexUniforms = sh.TextureUniforms;
            for (int i  = 0; i < shTexUniforms.Length; i++) 
            {
                sh.SetUniformIntGL(shTexUniforms[i], Texture.TextureUnitToInt(tu[i]));
            }

            sh.SetUniformMat4GL("model", vm.Model * _scale * Matrix4.CreateTranslation(_pos.AsVec3()));
            sh.SetUniformMat4GL("proj", vm.UIOrtho);

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
                SentenceData ns = _font.TextForString(_text, Vec2.Zero);

                _sentence.Vertices = ns.Vertices;
                _sentence.Indexes = ns.Indexes;

                UpdateBuffers();

                _textChanged = false;
            }
        }

        /// <summary>
        /// Update the buffers for changes to the text
        /// </summary>
        private void UpdateBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex.SizeInBytesU * _sentence.Vertices.Length, _sentence.Vertices, _bufferUse);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _sentence.Indexes.Length, _sentence.Indexes, _bufferUse);
        }

        /// <summary>
        /// Update Text for changes in the last frame.
        /// </summary>
        public void Update()
        {
            if (_textChanged)
            {
                UpdateSentence();
            }
        }
        #endregion

        /// <summary>
        /// Dispose of unmanaged resources.
        /// </summary>
        /// <param name="disposing">If true program is calling dispose, if false GC is.</param>
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

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~Text()
        {
            Dispose(false);
        }
    }

    /// <summary>
    /// The collection of data that defines a piece of text.
    /// </summary>
    public struct SentenceData
    {
        /// <summary>
        /// The vertices of the text.
        /// </summary>
        public TexturedVertex2D[] Vertices;

        /// <summary>
        /// Vertex indices of the text.
        /// </summary>
        public uint[] Indexes;

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color4 Color;
    }
}
