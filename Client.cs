using KirosEngine3.Math.Matrix;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    /// <summary>
    /// Abstract class to be inherited by individual works using the engine
    /// </summary>
    public abstract class Client : GameWindow
    {
        /// <summary>
        /// Key for the graphics mode variable
        /// </summary>
        public const string GRAPHICSMODE_KEY = "GRAPHICS_MODE";

        /// <summary>
        /// Value constant for the OpenGL graphics mode
        /// </summary>
        public const string GRAPHICSMODE_GL_VAL = "OPENGL";

        /// <summary>
        /// Value constant for the DirectX graphics mode
        /// </summary>
        public const string GRAPHICSMODE_DX_VAL = "DIRECTX";

        protected static bool _showGLDebugNotify = false;

        public Client(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {

        }

        /// <summary>
        /// OpenGL debug delegate method
        /// </summary>
        /// <param name="source">Source for the debug message</param>
        /// <param name="type">The type of the message</param>
        /// <param name="id">The message id</param>
        /// <param name="severity">The severity of the message</param>
        /// <param name="length">The length of the message</param>
        /// <param name="pMessage">The pointer to the message</param>
        /// <param name="pUser">The pointer to user callback data</param>
        protected static void OnDebugMessage(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr pMessage, IntPtr pUser)
        {
            string message = Marshal.PtrToStringUTF8(pMessage, length);

            if (severity == DebugSeverity.DebugSeverityNotification)
            {
                if (_showGLDebugNotify)
                    Console.WriteLine("[{0} type={1} id={2}] {3}", severity, type, id, message);
            }
            else
                Console.WriteLine("[{0} type={1} id={2}] {3}", severity, type, id, message);
        }

        /// <summary>
        /// OpenGL debug delegate instance
        /// </summary>
        private static DebugProc DebugDelegate = new DebugProc(OnDebugMessage);

        public static void EnableGLDebugNotify()
        {
            _showGLDebugNotify = true;
        }

        public static void DisableGLDebugNotify()
        {
            _showGLDebugNotify = false;
        }

        /// <summary>
        /// Load any data needed for the initial screen
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();

            //OpenGL debug messaging
            GL.DebugMessageCallback(DebugDelegate, 0);
        }

        /// <summary>
        /// Update data before the draw of the next frame
        /// </summary>
        /// <param name="args">Frame event arguments</param>
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        /// <summary>
        /// Render the frame, making draw calls to any objects to be drawn
        /// </summary>
        /// <param name="args">Frame event arguments</param>
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
        }

        /// <summary>
        /// Handle window resize events
        /// </summary>
        /// <param name="e">Resize event arguments</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        /// <summary>
        /// Cleanup and unload items before closing the program
        /// </summary>
        protected override void OnUnload()
        {
            base.OnUnload();
        }
    }

    public struct ViewMatrixes
    {
        public Matrix4 Model { get; set; }
        public Matrix4 View { get; set; }
        public Matrix4 Projection { get; set; }
        public Matrix4 Orthographic { get; set; }
    }
}
