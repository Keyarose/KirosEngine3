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
        protected string[] _text = [];

        /// <summary>
        /// The screen position of the text's origin.
        /// </summary>
        protected Vec2 _pos;

        /// <summary>
        /// The size of the text object on screen.
        /// </summary>
        protected Vec2 _size;

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
        /// The vertices of the text.
        /// </summary>
        protected TexturedVertex2D[][] _verts = [];

        /// <summary>
        /// The indices of the list.
        /// </summary>
        protected uint[][] _indices = [];

        /// <summary>
        /// The number of lines the text spans.
        /// </summary>
        protected int _lineCount = 1;

        /// <summary>
        /// The vertex array object.
        /// </summary>
        protected int _VAO;

        /// <summary>
        /// The vertex buffer object.
        /// </summary>
        protected int[] _VBO = [];

        /// <summary>
        /// The element/index buffer object.
        /// </summary>
        protected int[] _EBO = [];

        /// <summary>
        /// Flag to toggle the use of blend in render.
        /// </summary>
        protected bool _blendEnabled = true;

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
        /// The text draw mode to be used.
        /// </summary>
        protected TextDrawMode _tDrawMode = TextDrawMode.ByBlock;

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
        public string[] Lines { get { return _text; } set { _text = value; _textChanged = true; Update(); } }

        /// <summary>
        /// Get the number of lines the text spans.
        /// </summary>
        public int LineCount { get { return _lineCount; } }

        /// <summary>
        /// The position of the text
        /// </summary>
        public Vec2 Position { get { return _pos; } set { _pos = value; } }

        /// <summary>
        /// The size of the text object on screen.
        /// </summary>
        public Vec2 Size { get { return _size; } set { _size = value; } }

        /// <summary>
        /// The width of the text object.
        /// </summary>
        public float Width { get { return _size.X; } set { _size.X = value; } }
        /// <summary>
        /// The height of the text object.
        /// </summary>
        public float Height { get { return _size.Y; } set { _size.Y = value; } }

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
        public TexturedVertex2D[][] Vertices { get { return _verts; } }

        /// <summary>
        /// The indices of the text object.
        /// </summary>
        public uint[][] Indices { get { return _indices; } }

        /// <summary>
        /// The color of the text
        /// </summary>
        public Color4 Color { get { return _sentence.Color; } set { _sentence.Color = value; } }

        /// <summary>
        /// The text draw mode to be used in rendering.
        /// </summary>
        public TextDrawMode TextDrawMode { get { return _tDrawMode; } set { _tDrawMode = value; } }

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
        public Text(Vec2 pos, string text) : this(pos, FontManager.Default, text) { }

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
            _pos = pos;

            //process text, if there is no n\ then text is only one line
            _text = text.Split("\n");
            _lineCount = _text.Length;

            _sentence = new()
            {
                Color = Color4.Black
            };
        }

        /// <summary>
        /// Enable the use of blend in rendering.
        /// </summary>
        public void EnableBlend()
        {
            _blendEnabled = true;
        }

        /// <summary>
        /// Disable the use of blend in rendering.
        /// </summary>
        public void DisableBlend()
        {
            _blendEnabled = false;
        }

        #region Loading
        /// <summary>
        /// Initialize the text for rendering on it's own.
        /// </summary>
        /// <returns>True if init is successful, false otherwise.</returns>
        public bool Init()
        {
            string[] endText = FitText(_text);

            _verts = new TexturedVertex2D[endText.Length][];
            _indices = new uint[endText.Length][];

            //compile string data
            for (int i = 0; i < endText.Length; i++)
            {
                //set the position to the font height * the line number
                Vec2 posOffset = new Vec2(0f, _font.Size * i);
                Tuple<TexturedVertex2D[], uint[]> lineData = _font.QuadsForString(endText[i], posOffset);

                _verts[i] = lineData.Item1;
                _indices[i] = lineData.Item2;
            }

            _VBO = new int[endText.Length];
            _EBO = new int[endText.Length];

            //gen the buffers, but don't buffer until draw
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);

            GL.GenBuffers(_verts.Length, _VBO);
            GL.GenBuffers(_indices.Length, _EBO);

            GL.BindVertexArray(0);

            Update();

            _loaded = true;
            return true;
        }

        /// <summary>
        /// Fit the provided text to the width of the text's size using the set font.
        /// </summary>
        /// <param name="text">The text to fit.</param>
        /// <returns>The resulting array of strings sized to fit.</returns>
        private string[] FitText(string[] text)
        {
            string[] result = [];
            for (int li = 0; li < text.Length; li++)
            {
                string line = text[li];

                float textWidth = _font.TextWidth(line);

                if (textWidth > _size.X)//text is too long needs to wrap
                {
                    string[] subs = line.Split(' ');//todo: delimiter setting.

                    string[] resLines = new string[subs.Length];//at most sub.length lines
                    int resI = 0;

                    for (int i = 0; i < subs.Length; i++)
                    {
                        float subWidth = _font.TextWidth(subs[i]);

                        string section = subs[i];
                        int subI = i + 1;
                        while (subWidth < _size.X && subI < subs.Length)//while under the width and have subs left
                        {
                            subWidth = _font.TextWidth(section + " " + subs[subI]);

                            if (subWidth > _size.X)//too big, exit the loop and move on
                            {
                                continue;
                            }
                            else //can fit more add the current section and move to the next.
                            {
                                section += " " + subs[subI];
                                subI++;
                            }
                        }

                        resLines[resI] = section;
                        resI++;
                    }

                    resLines = resLines.Where(x => !string.IsNullOrEmpty(x)).ToArray();//trim any empty values
                    foreach (var r in resLines)//copy to the final text array
                    {
                        result = [.. result, r];
                    }
                }
                else //the line fits
                {
                    result = [.. result, line];
                }
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Set the text to the given string
        /// </summary>
        /// <param name="text">The string to set the text to.</param>
        public void SetText(string text)
        {
            _text = text.Split("\n");
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
            for (int i = 0; i < shTexUniforms.Length; i++)
            {
                sh.SetUniformIntGL(shTexUniforms[i], Texture.TextureUnitToInt(tu[i]));
            }

            sh.SetUniformMat4GL("model", vm.Model * _scale * Matrix4.CreateTranslation(_pos.AsVec3()));
            sh.SetUniformMat4GL("proj", vm.UIOrtho);

            sh.SetUniformVec4GL("aTextColor", (Vec4)_sentence.Color);

            GL.BindVertexArray(_VAO);

            if (_blendEnabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }

            for (int i = 0; i < _verts.Length; i++)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * _verts[i].Length, _verts[i], _bufferUse);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO[i]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices[i].Length, _indices[i], _bufferUse);


                sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
                sh.SetUVAttribGL(new ShaderAttribSettings { Offset = TexturedVertex2D.UVOffset, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
                GL.DrawElements(_drawMode, _indices[i].Length, DrawElementsType.UnsignedInt, 0);
            }

            if (_blendEnabled)
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
                string[] endText = FitText(_text);

                _verts = new TexturedVertex2D[endText.Length][];
                _indices = new uint[endText.Length][];

                //compile string data
                for (int i = 0; i < endText.Length; i++)
                {
                    //set the position to the font height * the line number
                    Vec2 posOffset = new Vec2(0f, _font.Size * i);
                    Tuple<TexturedVertex2D[], uint[]> lineData = _font.QuadsForString(endText[i], posOffset);

                    _verts[i] = lineData.Item1;
                    _indices[i] = lineData.Item2;
                }

                UpdateBuffers();

                _textChanged = false;
            }
        }

        /// <summary>
        /// Update the buffers for changes to the text
        /// </summary>
        private void UpdateBuffers()
        {
            //delete the old buffers if not null
            GL.DeleteBuffers(_verts.Length, _VBO);
            GL.DeleteBuffers(_indices.Length, _EBO);

            GL.BindVertexArray(_VAO);

            //create the new ones
            GL.GenBuffers(_verts.Length, _VBO);
            GL.GenBuffers(_indices.Length, _EBO);
            for (int i = 0; i < _verts.Length; i++)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * _verts[i].Length, _verts[i], _bufferUse);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO[i]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices[i].Length, _indices[i], _bufferUse);
            }

            GL.BindVertexArray(0);
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

                GL.DeleteBuffers(_verts.Length, _VBO);
                GL.DeleteBuffers(_indices.Length, _EBO);
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
