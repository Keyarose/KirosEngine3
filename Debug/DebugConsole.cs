using KirosEngine3.Config;
using KirosEngine3.Input;
using KirosEngine3.Math.Vector;
using KirosEngine3.Textures;
using KirosEngine3.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Debug
{
    /// <summary>
    /// The application debug console for development and testing.
    /// </summary>
    public class DebugConsole : IDisposable
    {
        private static DebugConsole? _instance;

        /// <summary>
        /// The chat box that displays the debug log and allows input.
        /// </summary>
        protected ChatBox _chatBox;

        private static readonly Queue<string> _preloadBuffer = new Queue<string>();

        /// <summary>
        /// A label that displays the current mouse position in screen coordinates.
        /// </summary>
        protected Label _mousePosLabel;

        /// <summary>
        /// Whether the DebugConsole is currently visible.
        /// </summary>
        protected bool _visible = false;

        /// <summary>
        /// Flag that shows if the DebugConsole has been loaded.
        /// </summary>
        protected bool _loaded = false;

        /// <summary>
        /// Flag that shows if the DebugConsole is being disposed of.
        /// </summary>
        protected bool _disposed = false;

        /// <summary>
        /// The position on the screen of the DebugConsole.
        /// </summary>
        public Vec2 Position
        {
            get { return _chatBox.Position; }
            set { _chatBox.Position = value; }
        }

        /// <summary>
        /// The size of the DebugConsole.
        /// </summary>
        public Vec2 Size
        {
            get { return _chatBox.Size; }
            set { _chatBox.Size = value; }
        }

        /// <summary>
        /// Get or Set the maximum number of lines the console is to remember
        /// </summary>
        public int MaxLines
        {
            get { return _chatBox.MaxLines; }
            set { _chatBox.MaxLines = value; }
        }

        /// <summary>
        /// Get and Set the console's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// The DebugConsole Instance.
        /// </summary>
        public static DebugConsole? Instance
        {
            get
            {
                if (_instance == null)
                {
                    Logger.WriteToLog("Call to Debug Console before it was created.");
                    Console.WriteLine("Call to Debug Console before it was created.");
                }
                return _instance;
            }
        }

        private DebugConsole(Vec2 pos, Vec2 size, int maxLines) :
            this((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, maxLines)
        { }

        private DebugConsole(Vec2 pos, Vec2 size, int maxLines, Font? font) :
            this((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, maxLines, font)
        { }

        private DebugConsole(int xPos, int yPos, int width, int height, int maxLines) : this(xPos, yPos, width, height, maxLines, FontManager.Default)
        { }

        private DebugConsole(int xPos, int yPos, int width, int height, int maxLines, Font? font)
        {
            _chatBox = new ChatBox(new Vec2(xPos, yPos), new Vec2(width, height), "debug", font, maxLines);
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.GraveAccent, KeyboardEventType.KeyPressed, OnKeyPress);

            _mousePosLabel = new Label(new Vec2(1, 1), new Vec2(65, 20), font, "()");
            MouseEventManager.SubscribeMouseEvent(MouseEventManager.GLOBAL_CONTEXT, Input.MouseButton.None, MouseEventType.Moved, OnMouseMove);

            _visible = false;
        }

        /// <summary>
        /// Create a new _instance of the debug console if one does not exist, log a message if one does.
        /// </summary>
        /// <param name="xPos">The X position of the console.</param>
        /// <param name="yPos">The Y position of the console.</param>
        /// <param name="width">The width of the console.</param>
        /// <param name="height">The height of the console.</param>
        /// <param name="maxLines">The maximum number of lines in the textbox.</param>
        /// <param name="font">The font to use.</param>
        public static void Create(int xPos, int yPos, int width, int height, int maxLines, Font? font)
        {
            if (_instance == null)
            {
                _instance = new DebugConsole(xPos, yPos, width, height, maxLines, font);

                _instance.Init();
            }
            else
            {
                Client.Report("Debug Console already created.");
            }
        }

        /// <summary>
        /// Write a string to the debug console.
        /// </summary>
        /// <param name="message">The string to be written.</param>
        public static void WriteLine(string message)
        {
            if (Instance == null)
            {
                _preloadBuffer.Enqueue(message);
            }
            else
            {
                Instance._chatBox.AddToTextLog("debugWrite", message);
            }
        }

        /// <summary>
        /// Write a formatted string to the debug console.
        /// </summary>
        /// <param name="message">The string format to write.</param>
        /// <param name="arg0">The data to be inserted into the format.</param>
        public static void WriteLine(string message, object? arg0)
        {
            WriteLine(string.Format(message, arg0));
        }

        /// <summary>
        /// Write a formatted string to the debug console.
        /// </summary>
        /// <param name="message">The string format to write.</param>
        /// <param name="args">The data to be inserted into the format.</param>
        public static void WriteLine(string message, params object?[] args)
        {
            WriteLine(string.Format(message, args));
        }

        #region Load
        /// <summary>
        /// Load the debug console
        /// </summary>
        public void Init()
        {
            _mousePosLabel.Init();
            _chatBox.Init();

            int pbCount = _preloadBuffer.Count;
            for (int i = 0; i < pbCount; i++) //add all buffered messages to the textbox
            {
                _chatBox.AddToTextLog("preloadBuffer", _preloadBuffer.Dequeue());
            }

            _chatBox.Border = true;
            _chatBox.ReceivesCommands = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// DrawGL the visible lines only if the console is open, and thus visible
        /// </summary>
        public void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            if (_visible)
            {
                _mousePosLabel.DrawGL(vm, tu);
                _chatBox.DrawGL(vm, tu);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the console.
        /// </summary>
        public void Update()
        {
            if (_visible)
            {
                _mousePosLabel.Update();
                _chatBox.Update();
            }
        }
        #endregion

        #region Input
        /// <summary>
        /// On Key pressed event input processing.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">The event arguments.</param>
        public void OnKeyPress(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.GraveAccent)//if the grave key is pressed toggle visibility and current input context 
            {
                _visible = !_visible;

                if (_visible)
                {
                    KeyboardEventManager.SetContext(_chatBox.InputContext);
                    _chatBox.ForceUpdate();
                }
                else
                {
                    KeyboardEventManager.RevertContext();
                }
            }
        }

        /// <summary>
        /// On mouse move event processing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        public void OnMouseMove(object sender, MouseEventArgs args)
        {
            Vec2 position = args.ScreenPosition;

            _mousePosLabel.LabelText = [position.ToString()];
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose of unmanaged resources.
        /// </summary>
        /// <param name="disposing">If true the program is calling, false is the GC.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                if (disposing)
                {
                    //clear managed items
                }

                _chatBox.Dispose();
                _mousePosLabel.Dispose();

                _disposed = true;
            }
        }

        /// <summary>
        /// Release the DebugConsole's resources for unloading.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Deconstructor.
        /// </summary>
        ~DebugConsole()
        {
            Dispose(false);
        }
        #endregion
    }
}
