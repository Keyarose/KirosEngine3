using KirosEngine3.Debug;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;

namespace KirosEngine3.UI
{
    /// <summary>
    /// A Chat Box UI element.
    /// </summary>
    public class ChatBox : UIElement
    {
        /// <summary>
        /// The textbox component of the Chat Box.
        /// </summary>
        protected TextBox _textbox;

        /// <summary>
        /// The text log component of the Chat Box.
        /// </summary>
        protected ScrollableTextLog _textLog;

        /// <summary>
        /// The font to be used in the ChatBox.
        /// </summary>
        protected Font _font = FontManager.Default;

        /// <summary>
        /// The context in which the ChatBox should receive input.
        /// </summary>
        protected string _inputContext = "";

        #region Properties
        /// <summary>
        /// The on screen position of the ChatBox.
        /// </summary>
        public override Vec2 Position
        {
            get { return _position; }
            set { _position = value; }//todo: clamp to prevent the box from going off screen
        }

        /// <summary>
        /// The size of the ChatBox.
        /// </summary>
        public override Vec2 Size
        {
            get { return _size; }
            set { _size = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The width of the ChatBox.
        /// </summary>
        public override float Width
        {
            get { return _size.X; }
            set { _size.X = value; }//todo: clamp to prevent off screen flow
        }

        /// <summary>
        /// The height of the ChatBox.
        /// </summary>
        public override float Height
        {
            get { return _size.Y; }
            set { _size.Y = value; }//todo: clamp
        }

        /// <summary>
        /// The context in which the ChatBox should receive keyboard input.
        /// </summary>
        public string InputContext
        {
            get { return _inputContext; }
        }

        /// <summary>
        /// The maximum number of lines in the log.
        /// </summary>
        public int MaxLines
        {
            get { return _textLog.MaxLines; }
            set { _textLog.MaxLines = value; }
        }

        /// <summary>
        /// Whether the ChatBox should process application commands from the user.
        /// </summary>
        public bool ReceivesCommands
        {
            get { return _textbox.ReceivesCommands; }
            set 
            { 
                _textbox.ReceivesCommands = value;
                if (value)
                    _textbox.ActionOnEnter = AddToTextLogSendCmd;
                else
                    _textbox.ActionOnEnter = AddToTextLog;
            }
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
        public ChatBox(Vec2 position, Vec2 size, string name, Font? font, int maxLines)
        {
            _position = position;
            _size = size;
            _name = name;
            //todo: clamp size to fit container

            _font = font ?? FontManager.Default;

            _inputContext = "chatbox-" + name;

            _textLog = new ScrollableTextLog(Vec2.Zero, new Vec2(size.X, size.Y - _font.Size), "textLog-" + name, _font, maxLines, this, _inputContext)
            {
                Border = true
            };

            //position is X = 0 in cb coordinates, and Y = size - font size, thus the last row
            _textbox = new TextBox(new Vec2(0.0f, size.Y - _font.Size), new Vec2(size.X, _font.Size), "textbox-" + name, _font, AddToTextLog, this, _inputContext)
            {
                Border = true
            };
        }

        #region Load
        /// <summary>
        /// Initialize the textbox.
        /// </summary>
        /// <returns>True if successful.</returns>
        public override bool Init(int VAO = 0)
        {
            if (!base.Init(VAO)) return false; //perform base class init and fail if it fails

            _textbox.Init(_VAO);//give the textbox the chatbox vao
            _textLog.Init(_VAO);

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
                    Client.Report("Attempt to draw unloaded ChatBox object: {0}", this);

                    _warnOnce = true;
                }

                return;
            }
            vm.Model *= Matrix4.CreateTranslation(_position.AsVec3());

            base.DrawGL(vm, tu);

            //todo: move use font call up to the caller of this method and group elements using the same font to render in a batch
            //if use font fails or font is null
            if (!_font.UseFont(tu))
            {
                Client.Report("Attempt to use font: {0} failed", _font);
            }

            if (!_parentVAO)
                GL.BindVertexArray(_VAO);

            _textLog.DrawGL(vm, tu);
            _textbox.DrawGL(vm, tu);

            if (!_parentVAO)
                GL.BindVertexArray(0);
        }

        /// <inheritdoc/>
        public override void DrawDX()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the chat box for changes in the last frame.
        /// </summary>
        public void Update()
        {
            _textLog.Update();
            _textbox.Update();
        }

        /// <summary>
        /// Force an update to the text data.
        /// </summary>
        public void ForceUpdate()
        {
            _textLog.ForceUpdate();
            _textbox.ForceUpdate();
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
        /// Delegate method to pass text to the log.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="text">The text to go to the log.</param>
        public void AddToTextLog(object sender, string text)
        {
            //todo: allow changes to the text based on sender.
            _textLog.AddLine(text);
        }

        /// <summary>
        /// Delegate method to pass text to the log and send commands to the command manager.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="text">The text to send.</param>
        public void AddToTextLogSendCmd(object sender, string text)
        {
            _textLog.AddLine(text);//send text to the log
            if (text != string.Empty && text[0] == '\\')
            {
                CommandManager.ProcessCommand(text);
            }
        }

        #region Dispose
        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~ChatBox()
        {
            Dispose(false);
        }
        #endregion
    }
}
