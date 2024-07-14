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

        /// <summary>
        /// Flag to allow or disallow the output of OpenGL debug messages that are only notifications.
        /// </summary>
        private static bool _showGLDebugNotify = false;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="width">The width of the client window.</param>
        /// <param name="height">The height of the client window.</param>
        /// <param name="title">The title for the window.</param>
        public Client(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) { }

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

        /// <summary>
        /// Enable the output of notification level OpenGL debug messages.
        /// </summary>
        public static void EnableGLDebugNotify()
        {
            _showGLDebugNotify = true;
        }

        /// <summary>
        /// Disable the output of notification level OpenGL debug messages.
        /// </summary>
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

    /// <summary>
    /// Container struct for the view matrices.
    /// </summary>
    public struct ViewMatrixes
    {
        /// <summary>
        /// The Model matrix, commonly the identity matrix and modified inside each renderable.
        /// </summary>
        public Matrix4 Model { get; set; }

        /// <summary>
        /// The View matrix, constructed by the active camera object.
        /// </summary>
        public Matrix4 View { get; set; }

        /// <summary>
        /// The Projection matrix, constructed by the active camera object.
        /// </summary>
        public Matrix4 Projection { get; set; }

        /// <summary>
        /// The Orthographic matrix, constructed by the active camera object and used mainly for UI/HUD.
        /// </summary>
        public Matrix4 Orthographic { get; set; }

        /// <summary>
        /// The Orthographic matrix for UI elements.
        /// </summary>
        public Matrix4 UIOrtho { get; set; }
    }
}
