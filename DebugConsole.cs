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

namespace KirosEngine3
{
    internal class DebugConsole
    {
        private static DebugConsole? _instance;

        protected TextBox _textBox;

        /// <summary>
        /// A label that displays the current mouse position in screen coordinates.
        /// </summary>
        protected Label _mousePosLabel;

        protected bool _visible;
        //todo: preload line buffer for messages before the _instance is created
        public Vec2 Position
        {
            get { return _textBox.Position; }
            set { _textBox.Position = value; }
        }

        public Vec2 Size
        {
            get { return _textBox.Size; }
            set { _textBox.Size = value; }
        }

        /// <summary>
        /// Get or Set the maximum number of lines the console is to remember
        /// </summary>
        public int MaxLines
        {
            get { return _textBox.MaxLines; }
            set { _textBox.MaxLines = value; }
        }

        /// <summary>
        /// Get and Set the console's visibility
        /// </summary>
        public bool IsVisible
        {
            get { return _visible; }
            set { _visible = value; }
        }

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

        private DebugConsole(int xPos,  int yPos, int width, int height, int maxLines) : this(xPos, yPos, width, height, maxLines, FontManager.Default)
        { }

        private DebugConsole(int xPos, int yPos, int width, int height, int maxLines, Font? font)
        {
            _textBox = new TextBox(new Vec2(xPos, yPos), new Vec2(width, height), "debug", font, maxLines);
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.GraveAccent, KeyboardEventType.KeyPressed, OnKeyPress);

            _mousePosLabel = new Label(new Vec2(1, 1), new Vec2(50, 20), font, "()");
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
                Console.WriteLine("Debug Console already created.");
                Logger.WriteToLog("Debug Console already created.");
                //write to debug
            }
        }

        #region Load
        /// <summary>
        /// Load the debug console
        /// </summary>
        public void Init()
        {
            _mousePosLabel.Init();
            _textBox.Init();
            _textBox.Border = true;
        }
        #endregion

        #region Draw
        /// <summary>
        /// DrawGL the visible lines only if the console is open, and thus visible
        /// </summary>
        public void DrawGL(ViewMatrixes vm, params TextureUnit[] tu)
        {
            if(_visible) 
            {
                _mousePosLabel.DrawGL(vm, tu);
                _textBox.DrawGL(vm, tu);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the console.
        /// </summary>
        public void Update()
        {
            _textBox.Update();
        }
        #endregion

        #region Input
        public void OnKeyPress(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.GraveAccent)//if the grave key is pressed toggle visibility and current input context 
            {
                _visible = !_visible;
                
                if (_visible ) 
                {
                    KeyboardEventManager.SetContext(_textBox.InputContext);
                }
                else
                {
                    KeyboardEventManager.RevertContext();
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs args)
        {
            Vec2 position = args.ScreenPosition;

            _mousePosLabel.LabelText = [position.ToString()];
        }
        #endregion
    }
}
