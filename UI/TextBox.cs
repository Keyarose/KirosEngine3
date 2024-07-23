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
    /// A textbox UI element.
    /// </summary>
    public class TextBox : UIElement
    {
        //origin at upper left

        /// <summary>
        /// The unique name of the textbox.
        /// </summary>
        protected string _name;

        /// <summary>
        /// The name of the shader to be used in rendering.
        /// </summary>
        protected string _shaderName = ShaderManager.DefaultTextShaderName ?? "";

        /// <summary>
        /// The font to be used in the textbox.
        /// </summary>
        protected Font _font = FontManager.Default;

        /// <summary>
        /// The context in which the textbox should receive keyboard input.
        /// </summary>
        protected string _inputContext = "";

        /// <summary>
        /// The color to be used for the text.
        /// </summary>
        protected Color4 _textColor = Color4.Black;

        /// <summary>
        /// The collection of lines of text in the textbox.
        /// </summary>
        protected List<Tuple<string, TexturedVertex2D[], uint[]>> _lines;
        /// <summary>
        /// The number of lines that fit within the textbox.
        /// </summary>
        protected int _visibleLines;

        /// <summary>
        /// The position to which the textbox has be scrolled to.
        /// </summary>
        protected int _scrollPos = 0;

        /// <summary>
        /// The currently active line.
        /// </summary>
        protected string _activeLine = "";

        private TexturedVertex2D[] _aLVerts = [];
        private uint[] _aLIndices = [];

        /// <summary>
        /// The maximum number of lines in the textbox.
        /// </summary>
        protected int _maxLines;

        /// <summary>
        /// The draw mode to use in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        #region Flags
        /// <summary>
        /// Flag that shows if the textbox has been loaded
        /// </summary>
        protected bool _loaded = false;
        /// <summary>
        /// Flag that shows if the textbox is being disposed of.
        /// </summary>
        protected bool _disposed = false;
        private bool _warnOnce = false;

        /// <summary>
        /// Flag that shows if the text has changed since the last update
        /// </summary>
        protected bool _textChanged = false;
        /// <summary>
        /// Flag that shows if the active text has changed since the last update
        /// </summary>
        protected bool _activeTextChanged = false;

        //settings
        /// <summary>
        /// Flag that shows if blend is enabled.
        /// </summary>
        protected bool _blendEnabled = true;
        /// <summary>
        /// Flag that shows if the textbox should recognize commands and pass them to the command system.
        /// </summary>
        protected bool _receivesCommands = false;
        #endregion

        #region Properties
        /// <summary>
        /// The on screen position of the textbox.
        /// </summary>
        public override Vec2 Position
        {
            get { return _position; }
            set { _position = value; }//todo: clamp to prevent the box from going off screen
        }

        /// <summary>
        /// The size of the textbox.
        /// </summary>
        public override Vec2 Size
        {
            get { return _size; }
            set { _size = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The width of the textbox.
        /// </summary>
        public override float Width
        {
            get { return _size.X; }
            set { _size.X = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The height of the textbox.
        /// </summary>
        public override float Height
        {
            get { return _size.Y; }
            set { _size.Y = value; }//todo: clamp
        }

        /// <summary>
        /// The context in which the textbox should receive keyboard input.
        /// </summary>
        public string InputContext
        {
            get { return _inputContext; }
        }

        /// <summary>
        /// The maximum number of lines in the textbox.
        /// </summary>
        public int MaxLines
        {
            get { return _maxLines; }
            set { _maxLines = value; }
        }

        /// <summary>
        /// Whether the textbox should process application commands from the user.
        /// </summary>
        public bool ReceivesCommands
        {
            get { return _receivesCommands; }
            set { _receivesCommands = value; }
        }
        #endregion

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="position">The position in screen coordinates.</param>
        /// <param name="size">The size in screen coordinates.</param>
        /// <param name="name">The name of the textbox component.</param>
        /// <param name="font">The font to be used.</param>
        /// <param name="maxLines">The maximum number of lines.</param>
        public TextBox(Vec2 position, Vec2 size, string name, Font? font, int maxLines)
        {
            _position = position;
            _size = size;
            _name = name;
            //todo: clamp size to fit on screen

            _font = font ?? FontManager.Default;
            _visibleLines = (int)(_size.Y / _font.Size);

            _maxLines = maxLines;
            _lines = new List<Tuple<string, TexturedVertex2D[], uint[]>>(maxLines);

            _inputContext = "textbox-" + name;
            KeyboardEventManager.SubscribeKeyboardEvents(_inputContext, KeyboardEventManager.AlphaNum, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Space, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Backspace, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Enter, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Up, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Down, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvents(_inputContext, KeyboardEventManager.AlphaNum, KeyboardEventType.KeyHeld, OnKeyHeld);
        }

        #region Load
        /// <summary>
        /// Initialize the textbox.
        /// </summary>
        /// <returns>True if successful.</returns>
        public override bool Init()
        {
            if (!base.Init()) return false; //perform base class init and fail if it fails

            _borderVerts = new Vertex2D[6];
            //_borderIndices = new uint[14];


            //define border verts
            float abvLL = _font.Size * (_visibleLines - 1) - 1;//one px above last line
            _borderVerts[0] = new Vertex2D { Position = _position };
            _borderVerts[1] = new Vertex2D { Position = _position + new Vec2(Width, 0f) };
            _borderVerts[2] = new Vertex2D { Position = _position + new Vec2(Width, abvLL) };
            _borderVerts[3] = new Vertex2D { Position = _position + new Vec2(0f, abvLL) };
            _borderVerts[4] = new Vertex2D { Position = _position + new Vec2(0f, Height) };
            _borderVerts[5] = new Vertex2D { Position = _size + _position };

            _borderIndices = [0, 1, 1, 2, 2, 3, 3, 0, 3, 4, 4, 5, 5, 2];

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

        /// <summary>
        /// Draw the textbox using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to be used in rendering.</param>
        /// <param name="tu">The texture units to be used in rendering.</param>
        public override void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            if (!_loaded || _disposed)
            {
                if (!_warnOnce)
                {
                    Logger.WriteToLog("Attempt to draw unloaded TextBox object: {0}", this);
                    Console.WriteLine("Attempt to draw unloaded TextBox object: {0}", this);
                    DebugConsole.WriteLine("Attempt to draw unloaded TextBox object: {0}", this);

                    _warnOnce = true;
                }

                return;
            }
            base.DrawGL(vm, tu);

            //todo: move use font call up to the caller of this method and group elements using the same font to render in a batch
            //if use font fails or font is null
            if (!_font.UseFont(tu))
            {
                Logger.WriteToLog("Attempt to use font: {0} failed", _font);
                Console.WriteLine("Attempt to use font: {0} failed", _font);
                DebugConsole.WriteLine("Attempt to use font: {0} failed", _font);
            }

            //if the shader fails to be added to the pipeline it is logged in TryGetShader
            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }

            sh.UseGL();

            //set the shader uniforms
            string[] shTexUniforms = sh.TextureUniforms;
            for (int i = 0; i < shTexUniforms.Length; i++)//todo: handle too few textureUnits
            {
                sh.SetUniformIntGL(shTexUniforms[i], Texture.TextureUnitToInt(tu[i]));
            }

            sh.SetUniformMat4GL("model", vm.Model * Matrix4.CreateTranslation(_position.AsVec3()));
            sh.SetUniformMat4GL("proj", vm.UIOrtho);

            sh.SetUniformVec4GL("aTextColor", (Vec4)_textColor);

            GL.BindVertexArray(_VAO);

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

            //draw the active line
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);//use the last vert buffer
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * _aLVerts.Length, _aLVerts, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _aLIndices.Length, _aLIndices, BufferUsageHint.DynamicDraw);

            sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
            sh.SetUVAttribGL(new ShaderAttribSettings { Offset = TexturedVertex2D.UVOffset, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
            GL.DrawElements(_drawMode, _aLIndices.Length, DrawElementsType.UnsignedInt, 0);

            //draw the border if enabled
            /*if (_border)
            {
                DrawBorderGL(vm);
            }*/

            if (_blendEnabled)
            {
                GL.DepthFunc(DepthFunction.Less);//return to default
                GL.Disable(EnableCap.Blend);
            }

            GL.BindVertexArray(0);
        }

        /// <inheritdoc/>
        public override void DrawDX()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void DrawBorderGL(ViewMatrixes vm)
        {
            //get the color shader
            if (!ShaderManager.TryGetShader(ShaderManager.DefaultColor2DShaderName, out Shader? sh))
            {
                Console.WriteLine("Cannot draw border for {0} when default color shader is null.", nameof(TextBox));
                Logger.WriteToLog("Cannot draw border for {0} when default color shader is null.", nameof(TextBox));
                DebugConsole.WriteLine("Cannot draw border for {0} when default color shader is null.", nameof(TextBox));
                return;
            }

            sh.UseGL();
            sh.SetUniformMat4GL("model", vm.Model);
            sh.SetUniformMat4GL("proj", vm.UIOrtho);
            sh.SetUniformVec4GL("aColor", (Vec4)_borderColor);

            //buffer border
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertex2D.SizeInBytesU * _borderVerts.Length, _borderVerts, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _borderIndices.Length, _borderIndices, BufferUsageHint.StaticDraw);

            sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = Vertex2D.SizeInBytesU });
            GL.DrawElements(PrimitiveType.Lines, _borderIndices.Length, DrawElementsType.UnsignedInt, 0);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the vertex data to reflect changes to the text
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
        /// Update the vertex data to reflect changes to the active text.
        /// </summary>
        private void UpdateActiveText()
        {
            if (_font != null)
            {
                Tuple<TexturedVertex2D[], uint[]> lineData = _font.QuadsForString(_activeLine, new Vec2(0, _font.Size * (_visibleLines - 1)));//position at the last line

                _aLVerts = lineData.Item1;
                _aLIndices = lineData.Item2;

                _activeTextChanged = false;
            }
        }

        /// <summary>
        /// Update the textbox for changes in the last frame.
        /// </summary>
        public void Update()
        {
            if (_textChanged)
            {
                UpdateText();
            }

            if (_activeTextChanged)
            {
                UpdateActiveText();
            }
        }

        /// <summary>
        /// Force an update to the text data.
        /// </summary>
        public void ForceUpdate()
        {
            _textChanged = true;
            _activeTextChanged = true;
        }

        /// <summary>
        /// Do updates and changes that need to be done when the box or it's parent container have be resized.
        /// </summary>
        public void OnResize()
        {
            //todo:
        }
        #endregion

        /// <summary>
        /// Add a string to the textbox, which will then fit the string to one or more lines.
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
        }

        /// <summary>
        /// Fit the given line to the width of the textbox.
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
                float lineWidth = _font.TextWidth(workingLine);
                while (lineWidth < Width)
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
            char nChar;
            if (args.Key == Keys.Backspace)
            {
                if (_activeLine.Length > 0)
                {
                    _activeLine = _activeLine.Remove(_activeLine.Length - 1);//remove the last char
                }
                else
                {
                    //active line is 0
                    //todo: if allowed bring the previous line back to active, if there is one
                }
            }
            else if (args.Key == Keys.Enter)
            {
                _lines.Add(new Tuple<string, TexturedVertex2D[], uint[]>(_activeLine, [], []));//store the line as is
                _activeLine = "";//and start a new one

                if (_lines.Count > _visibleLines - 1)//if we're over the number of lines visible in the box
                {
                    if (_scrollPos < _maxLines)
                    {
                        _scrollPos++;
                    }
                }

                if (_lines.Count > _maxLines)
                {
                    _lines.RemoveAt(0);//remove the first line
                }

                //if this textbox can receive commands
                if (_receivesCommands)
                {
                    //if the first char of the submitted string is \
                    if (_lines.Last().Item1[0] == '\\')
                    {
                        //CommandManager.Execute(_lines.Last().Item1);
                    }
                }

                _textChanged = true;
            }
            else if (args.Key == Keys.Up)
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
            else
            {
                if (KeyboardEventManager.AlphaNum.Contains(args.Key))
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                    {
                        nChar = (char)args.Key;

                    }
                    else
                    {
                        nChar = char.ToLower((char)args.Key);
                    }
                }
                else if (args.Key == Keys.Space)
                {
                    nChar = ' ';
                }
                else if (args.Key == Keys.Enter)
                {
                    nChar = '\n';
                }
                else
                {
                    nChar = '\u25A1';
                }

                float lineLength = _font.TextWidth(_activeLine + nChar);
                if (lineLength < Width)//if the line fits
                {
                    _activeLine += nChar;
                }
                else //if it doesn't
                {
                    _lines.Add(new Tuple<string, TexturedVertex2D[], uint[]>(_activeLine, [], []));//store the line as is
                    _activeLine = "" + nChar;//and start a new one
                    if (_lines.Count > _visibleLines - 1)//if we're over the number of lines visible in the box
                    {
                        if (_scrollPos < _maxLines)
                        {
                            _scrollPos++;
                        }
                    }

                    if (_lines.Count > _maxLines)
                    {
                        _lines.RemoveAt(0);//remove the first line
                    }

                    _textChanged = true;
                }
            }
            _activeTextChanged = true;
        }

        /// <summary>
        /// Handle received keyboard input (held).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyHeld(object sender, KeyboardEventArgs args)
        {

            //_textChanged = true;
        }
        #endregion
    }
}
