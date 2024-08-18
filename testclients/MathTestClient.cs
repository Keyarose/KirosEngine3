using KirosEngine3.Config;
using KirosEngine3.Debug;
using KirosEngine3.Input;
using KirosEngine3.Math;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Textures;
using KirosEngine3.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.testclients
{
    internal class MathTestClient : Client
    {
        ChatBox? cb;

        public MathTestClient(int width, int height, string title = "math test") : base(width, height, title) 
        {
            ConfigManager.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);//declare that we're using the OpenGL API
            if (!ConfigManager.LoadFromXML("Resources/Config/generalConfig.xml"))
            {
                //failed to load general config perform fallback
            }
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor((Color)new Color4(ConfigManager.Instance[ConfigKeys.D_CLEAR_COLOR_KEY]));//set clear color from config
            GL.Enable(EnableCap.DepthTest);

            //system control setup
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.Escape,
                KeyboardEventType.KeyPressed, (object sender, KeyboardEventArgs args) => { Close(); });

            cb = new ChatBox(new(0f, 0f), new(ClientSize.X, ClientSize.Y), "calc", FontManager.Default, 500);
            cb.Init();
            cb.AddSubmitAction(Calculator.ParseInput);
            Calculator.Instance.OutputTarget = cb.AddToTextLog;

            KeyboardEventManager.CurrentContext = cb.InputContext;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused) { return; }

            cb?.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ViewMatrixes viewMatrixes = new ViewMatrixes
            {
                Model = Matrix4.Identity,
                Projection = Matrix4.Identity,
                View = Matrix4.Identity,
                Orthographic = Matrix4.Identity,
                UIOrtho = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, ClientSize.Y, 0, -1f, 1f)
            };

            cb?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            DebugConsole.Instance?.DrawGL(viewMatrixes, TextureUnit.Texture0);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            cb?.Dispose();

            base.OnUnload();
        }
    }
}
