using KirosEngine3.Input;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KirosEngine3.UI
{
    /// <summary>
    /// Defines a single line text entry UI Element.
    /// </summary>
    public class TextBox : UIElement
    {
        /// <summary>
        /// The font to be used in the textbox.
        /// </summary>
        protected Font _font = FontManager.Default;

        /// <summary>
        /// The context in which the textbox should receive keyboard input.
        /// </summary>
        protected string _inputContext = "";

        /// <summary>
        /// The current position along the text string of the cursor.
        /// </summary>
        protected int _cursorPos = 0;

        /// <summary>
        /// The color to be used for the text.
        /// </summary>
        protected Color4 _textColor = Color4.Black;

        /// <summary>
        /// The text in the textbox.
        /// </summary>
        protected string _text = "";

        private TexturedVertex2D[] _verts = [];
        private uint[] _indices = [];

        /// <summary>
        /// The draw mode to use in rendering.
        /// </summary>
        protected PrimitiveType _drawMode = PrimitiveType.Triangles;

        #region Flags
        /// <summary>
        /// Flag that shows if blending is enabled.
        /// </summary>
        protected bool _blendEnabled = true;

        /// <summary>
        /// Whether the textbox should process application commands from the user.
        /// </summary>
        protected bool _receivesCommands = false;

        /// <summary>
        /// Flag that shows if the text has changed since the last update
        /// </summary>
        protected bool _textChanged = false;
        #endregion

        #region Properties
        /// <summary>
        /// The context in which the textbox should receive keyboard input.
        /// </summary>
        public string InputContext { get { return _inputContext; } }

        /// <summary>
        /// Whether the textbox should process applications commands from the user.
        /// </summary>
        public bool ReceivesCommands
        {
            get { return _receivesCommands; }
            set { _receivesCommands = value; }
        }

        /// <summary>
        /// Whether blending is enabled when drawing the textbox.
        /// </summary>
        public bool BlendEnabled
        {
            get { return _blendEnabled; } 
            set { _blendEnabled = value; }
        }
        #endregion

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="pos">The position of the textbox, relative to it's container.</param>
        /// <param name="size">The size of the textbox.</param>
        /// <param name="name">The name of the textbox element.</param>
        /// <param name="font">The font to use for the textbox, or null for default font.</param>
        public TextBox(Vec2 pos, Vec2 size, string name, Font? font) : this(pos, size, name, font, null, "textbox-" + name) { }

        /// <summary>
        /// Basic constructor with element parent.
        /// </summary>
        /// <param name="pos">The position of the textbox, relative to it's container.</param>
        /// <param name="size">The size of the textbox.</param>
        /// <param name="name">The name of the textbox element.</param>
        /// <param name="font">The font to use for the textbox, or null for default font.</param>
        /// <param name="parent">The containing UI Element.</param>
        /// <param name="inputContext">The input context to use for the textbox.</param>
        public TextBox(Vec2 pos, Vec2 size, string name, Font? font, UIElement? parent, string inputContext)
        {
            _position = pos;
            _size = size;
            _name = name;
            _shaderName = ShaderManager.DefaultTextShaderName ?? "";
            //todo: clamp size to container

            _font = font ?? FontManager.Default;

            _containingElement = parent;

            if (inputContext == "")
                _inputContext = "textbox-" + name;
            else
                _inputContext = inputContext;

            KeyboardEventManager.SubscribeKeyboardEvents(_inputContext, KeyboardEventManager.AlphaNum, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvents(_inputContext, KeyboardEventManager.PunctuationAndSymbols, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Space, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Backspace, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Enter, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Left, KeyboardEventType.KeyPressed, OnKeyPress);
            KeyboardEventManager.SubscribeKeyboardEvent(_inputContext, Keys.Right, KeyboardEventType.KeyPressed, OnKeyPress);
        }

        #region Load
        /// <summary>
        /// Initialize the textbox.
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
                    Client.Report("Attempt to draw unloaded ChatBox object: {0}", this);

                    _warnOnce = true;
                }

                return;
            }
            base.DrawGL(vm, tu);

            if (!_font.UseFont(tu))
            {
                Client.Report("Attempt to use font: {0} failed.", _font);
            }

            if (!ShaderManager.TryGetShader(_shaderName, out Shader? sh))
            {
                return;
            }

            sh.UseGL();

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

            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, TexturedVertex2D.SizeInBytesU * _verts.Length, _verts, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * _indices.Length, _indices, BufferUsageHint.DynamicDraw);

            sh.SetPositionAttribGL(new ShaderAttribSettings { Offset = 0, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
            sh.SetUVAttribGL(new ShaderAttribSettings { Offset = TexturedVertex2D.UVOffset, Size = 2, Stride = TexturedVertex2D.SizeInBytesU });
            GL.DrawElements(_drawMode, _indices.Length, DrawElementsType.UnsignedInt, 0);

            //done using blend, disable it
            if (_blendEnabled)
            {
                GL.DepthFunc(DepthFunction.Less);//return to default
                GL.Disable(EnableCap.Blend);
            }

            GL.BindVertexArray(0);
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the vertex data to reflect changes to the text string.
        /// </summary>
        private void UpdateText()
        {
            if (_font != null) 
            {
                Tuple<TexturedVertex2D[], uint[]> txtData = _font.QuadsForString(_text, Vec2.Zero);

                _verts = txtData.Item1;
                _indices = txtData.Item2;

                _textChanged = false;
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
        }

        /// <summary>
        /// Force an update to the text data.
        /// </summary>
        public void ForceUpdate()
        {
            _textChanged = true;
        }

        /// <summary>
        /// Do updates and changes that need to be done when the textbox or it's parent container are resized.
        /// </summary>
        public void OnResize()
        {
            //todo: resize
        }
        #endregion

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
                if (_text.Length > 0)
                {
                    _text = _text.Remove(_text.Length - 1);//remove the last char
                    _cursorPos--;
                    _textChanged = true;
                }
            }
            else if (args.Key == Keys.Enter)
            {
                //pass the text to where it needs to go.

                //if this textbox can receive commands
                if (_receivesCommands)
                {
                    //if the first char of the submitted string is \
                    if (_text[0] == '\\')
                    {
                        //CommandManager.Execute(_lines.Last().Item1);
                    }
                }
                _cursorPos = 0;
                _textChanged = true;
            }
            else if (args.Key == Keys.Left)
            {
                if (_cursorPos > 0)
                    _cursorPos--;
            }
            else if (args.Key == Keys.Right)
            {
                if (_cursorPos < _text.Length - 1)
                    _cursorPos++;
            }
            else
            {
                if (KeyboardEventManager.AlphaNum.Contains(args.Key))
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = (char)args.Key;
                    else
                        nChar = char.ToLower((char)args.Key);
                }
                else if (args.Key == Keys.Apostrophe)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '"';
                    else
                        nChar = '\'';
                }
                else if (args.Key == Keys.Semicolon)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = ':';
                    else
                        nChar = ';';
                }
                else if (args.Key == Keys.Comma)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '<';
                    else
                        nChar = ',';
                }
                else if (args.Key == Keys.Period)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '>';
                    else
                        nChar = '.';
                }
                else if (args.Key == Keys.Slash)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '?';
                    else
                        nChar = '/';
                }
                else if (args.Key == Keys.Backslash)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '|';
                    else
                        nChar = '\\';
                }
                else if (args.Key == Keys.LeftBracket)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '{';
                    else
                        nChar = '[';
                }
                else if (args.Key == Keys.RightBracket)
                {
                    if (args.ModifierKeys.HasFlag(ActiveModifierKeys.Shift))
                        nChar = '}';
                    else
                        nChar = ']';
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

                float lineLength = _font.TextWidth(_text + nChar);
                if (lineLength < Width)//if the line fits
                {
                    _text += nChar;
                    _cursorPos++;
                    _textChanged = true;
                }
            }
        }

        /// <summary>
        /// Handle received keyboard input (held).
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyHeld(object sender, KeyboardEventArgs args)
        {
            //todo:OnHeld
        }
        #endregion
    }
}
