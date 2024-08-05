using KirosEngine3.Input;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KirosEngine3.UI
{
    /// <summary>
    /// A Scrollable Text Log UI Element.
    /// </summary>
    public class ScrollableTextLog : UIElement
    {
        /// <summary>
        /// The font to be used in rendering.
        /// </summary>
        protected Font _font = FontManager.Default;

        /// <summary>
        /// The context in which the log should receive input.
        /// </summary>
        protected string _inputContext = "";

        /// <summary>
        /// The color to be used for the text.
        /// </summary>
        protected Color4 _textColor = Color4.Black;

        /// <summary>
        /// The collection of lines of text in the log.
        /// </summary>
        protected List<Tuple<string, TexturedVertex2D[], uint[]>> _lines;

        /// <summary>
        /// The number of lines that fit within the log.
        /// </summary>
        protected int _visibleLines;

        /// <summary>
        /// The position to which the log has been scrolled to.
        /// </summary>
        protected int _scrollPos = 0;

        /// <summary>
        /// The maximum number of lines in the log.
        /// </summary>
        protected int _maxLines;

        /// <summary>
        /// The draw mode to use in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        #region Flags
        /// <summary>
        /// Flag that shows if the text has changed since the last update.
        /// </summary>
        protected bool _textChanged = false;

        /// <summary>
        /// Flag that shows if blend is enabled.
        /// </summary>
        protected bool _blendEnabled = true;
        #endregion

        #region Properties
        /// <summary>
        /// The maximum number of lines in the log.
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        /// <summary>
        /// Get or set the log's current scroll position.
        /// </summary>
        public int ScrollPosition
        {
            get { return _scrollPos; }
            set { _scrollPos = value; }
        }
        #endregion

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="position">The position in screen coordinates.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="font">The font to use in rendering, null for default.</param>
        /// <param name="maxLines">The maximum number of lines in the log.</param>
        public ScrollableTextLog(Vec2 position, Vec2 size, string name, Font? font, int maxLines) : this(position, size, name, font, maxLines, null, "") { }

        /// <summary>
        /// Basic constructor with given inputContext and parent element.
        /// </summary>
        /// <param name="position">The position in screen coordinates.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="font">The font to use in rendering, null for default.</param>
        /// <param name="maxLines">The maximum number of lines in the log.</param>
        /// <param name="parent">The containing UI Element.</param>
        /// <param name="inputContext">The input context in which to receive input.</param>
        public ScrollableTextLog(Vec2 position, Vec2 size, string name, Font? font, int maxLines, UIElement? parent, string inputContext)
        {
            _position = position;
            _size = size;
            _name = name;
            _shaderName = ShaderManager.DefaultTextShaderName ?? "";
            //todo: clamp size to container

            _font = font ?? FontManager.Default;
            _visibleLines = (int)(_size.Y / _font.Size);

            _maxLines = maxLines;
            _lines = new List<Tuple<string, TexturedVertex2D[], uint[]>>(maxLines);

            if (inputContext == "")
                _inputContext = "textLog-" + name;
            else
                _inputContext = inputContext;

            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Up, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Down, KeyboardEventType.KeyPressed, OnKeyPress);
        }

        #region Load
        /// <summary>
        /// Initialize the text log.
        /// </summary>
        /// <param name="VAO">The parent element's vertex array object.</param>
        /// <returns>True if successful.</returns>
        public override bool Init(int VAO = 0)
        {
            if (!base.Init(VAO)) return false;


            _loaded = true;
            return true;
        }
        #endregion

        #region Draw
        /// <inheritdoc/>
        public override void Draw()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawDX()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            if (!_loaded || _disposed)
            {
                if (!_warnOnce)
                {
                    Client.Report("Attempt to draw unloaded ScrollableTextLog object: {0}", this);

                    _warnOnce = true;
                }

                return;
            }
            vm.Model *= Matrix4.CreateTranslation(_position.AsVec3());

            if (!_font.UseFont(tu))
            {
                Client.Report("Attempt to use font: {0} failed.", _font);
            }

            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }

            if (!_parentVAO)
                GL.BindVertexArray(_VAO);

            base.DrawGL(vm, tu);

            sh.UseGL();

            //set the shader uniforms
            string[] shTexUniforms = sh.TextureUniforms;
            if (shTexUniforms.Length != tu.Length)
            {
                Client.Report("Too few texture units provided to {0}, {1} are needed.", this, shTexUniforms.Length);
                return;
            }

            for (int i = 0; i < shTexUniforms.Length; i++)
            {
                sh.SetUniformIntGL(shTexUniforms[i], Texture.TextureUnitToInt(tu[i]));
            }

            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("proj", vm.UIOrtho);

            sh.SetUniformVec4GL("aTextColor", (Vec4)_textColor);

            if (_blendEnabled)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                GL.DepthFunc(DepthFunction.Lequal);
            }

            for (int i = 0; i < int.Min(_visibleLines - 1, _lines.Count - _scrollPos); i++) //for each visible line, or all existing lines if fewer than vis lines
            {
                Tuple<string, TexturedVertex2D[], uint[]> line = _lines[_scrollPos + i];
                GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * line.Item2.Length, line.Item2, BufferUsageHint.DynamicDraw);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * line.Item3.Length, line.Item3, BufferUsageHint.DynamicDraw);


                sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
                sh.SetUVAttribGL(new ShaderAttribSettings { Offset = TexturedVertex2D.UVOffset, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
                GL.DrawElements(_drawMode, line.Item3.Length, DrawElementsType.UnsignedInt, 0);
            }

            if (_blendEnabled)
            {
                GL.DepthFunc(DepthFunction.Less);//return to default
                GL.Disable(EnableCap.Blend);
            }

            if (!_parentVAO)
                GL.BindVertexArray(0);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the text log for changes in the last frame.
        /// </summary>
        public void Update()
        {
            if (_textChanged)
            {
                UpdateText();
            }
        }

        /// <summary>
        /// Update the vertex data to reflect changes to the text.
        /// </summary>
        private void UpdateText()
        {
            if (_font != null)
            {
                for (int i = 0; i < int.Min(_visibleLines - 1, _lines.Count - _scrollPos); i++) //for each line, except 1 reserved for the active line
                {
                    Tuple<string, TexturedVertex2D[], uint[]> line = _lines[i + _scrollPos];//get the line based on an offset of scroll pos
                    Tuple<TexturedVertex2D[], uint[]> lineData = _font.QuadsForString(line.Item1, new Vec2(0, _font.Size * i));

                    _lines[i + _scrollPos] = new Tuple<string, TexturedVertex2D[], uint[]>(line.Item1, lineData.Item1, lineData.Item2);
                }

                _textChanged = false;
            }
        }

        /// <summary>
        /// Force an update to the text data.
        /// </summary>
        public void ForceUpdate()
        {
            _textChanged = true;
        }

        /// <summary>
        /// Do updates and changes that need to be done when the element or parent are resized.
        /// </summary>
        public void OnResize()
        {
            //todo: resize.
        }
        #endregion

        /// <summary>
        /// Add a string to the text log, which will then fit the string to one or more lines.
        /// </summary>
        /// <param name="line">The string to be added.</param>
        public void AddLine(string line)
        {
            string[] lines = FitLine(line);
            foreach (string l in lines)
            {
                _lines.Add(new Tuple<string, TexturedVertex2D[], uint[]>(l, [], []));

                if (_lines.Count > _visibleLines - 1)//if we're over the number of lines visible in the box
                {
                    if (_scrollPos < _maxLines)
                    {
                        _scrollPos++;
                    }
                }

                if (_lines.Count > _maxLines)
                {
                    _lines.RemoveAt(0);//remove the first line if we're over the number of allowed lines.
                }
            }
            _textChanged = true;
        }

        /// <summary>
        /// Fit the given line to the width of the text log.
        /// </summary>
        /// <param name="line">The line to fit.</param>
        /// <returns>The resulting lines of a size that will fit.</returns>
        public string[] FitLine(string line)
        {
            string[] result = [];
            if (line == string.Empty)//if it's empty string return empty
                return result;

            if (_font.TextWidth(line) < Width)//if it fits return it
            {
                return [line];
            }

            char[] subs = line.ToCharArray();


            for (int i = 0; i < subs.Length;)
            {
                string workingLine = "" + subs[i];
                i++;//index i was used so go to the next

                float lineWidth = _font.TextWidth(workingLine);

                while (lineWidth < Width && i < subs.Length)
                {
                    lineWidth = _font.TextWidth(workingLine + subs[i]);
                    if (lineWidth > Width)//if adding the next char is too long break the loop
                        break;
                    else //it will fit so add it and check the next
                    {
                        workingLine += subs[i];
                        i++;
                    }
                }

                result = [.. result, workingLine];//working line fits add it to result
            }

            return result;
        }

        #region Input
        /// <summary>
        /// Handle received keyboard input (pressed).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyPress(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.Up)
            {
                //scroll up
                _scrollPos--;
                _scrollPos = int.Clamp(_scrollPos, 0, _lines.Count - 1);
                _textChanged = true;
            }
            else if (args.Key == Keys.Down) 
            {
                //scroll down
                _scrollPos++;
                _scrollPos = int.Clamp(_scrollPos, 0, _lines.Count - 1);//todo: allow scrolling to be configured to not scroll until only one line shows
                _textChanged = true;
            }
        }

        /// <summary>
        /// Handle received keyboard input (held).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyHeld(object sender, KeyboardEventArgs args)
        {
            //todo: on held
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~ScrollableTextLog()
        {
            Dispose(false);
        }
        #endregion
    }
}
