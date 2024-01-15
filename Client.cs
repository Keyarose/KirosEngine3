using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    /// <summary>
    /// Abstract class to be inherited by indivdual works using the engine
    /// </summary>
    public abstract class Client : GameWindow
    {
        public Client(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
        {

        }

        /// <summary>
        /// Load any data needed for the inital screen
        /// </summary>
        protected override void OnLoad()
        {
            base.OnLoad();
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
        /// <param name="args">Frame event arguemnts</param>
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
}
