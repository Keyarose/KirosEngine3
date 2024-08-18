using KirosEngine3.Camera;
using KirosEngine3.Config;
using KirosEngine3.Debug;
using KirosEngine3.Input;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Scenes;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using KirosEngine3.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.testclients
{
    /// <summary>
    /// Testing space for UI items.
    /// </summary>
    internal class UITestClient : Client
    {
        BaseCamera? camera;

        Text? testText;
        Label? testLabel;

        TextBox? testTextBox;
        ScrollableTextLog? testScrollLog;

        public UITestClient(int width, int height) : base(width, height, "UITestClient")
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

            GL.ClearColor((Color)new Color4(ConfigManager.Instance[ConfigKeys.D_CLEAR_COLOR_KEY]));
            GL.Enable(EnableCap.DepthTest);

            KeyboardEventManager.CurrentContext = "system";
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.Escape,
                KeyboardEventType.KeyPressed, (object sender, KeyboardEventArgs args) => { Close(); });

            camera = new BaseCamera(2.0f * Vec3.UnitZ, ClientSize.X, ClientSize.Y, .5f);
            camera.LookAt = Vec3.Zero;

            //load the test elements
            testText = new Text(new Vec2(), "test text.");
            testText.Size = new Vec2(50f, 20f);
            testText.Init();

            testLabel = new Label(new(600f, 50f), "test Label.");
            testLabel.Init();

            testTextBox = new TextBox(new(50f, 100f), new(400f, 21f), "txtBox", null);
            testTextBox.Init();

            testScrollLog = new ScrollableTextLog(new(50f, 200f), new(300f, 200f), "scrollLog", null, 20);
            testScrollLog.Init();
            testScrollLog.BackgroundColor = Color4.Green;
            testScrollLog.Border = true;
            testScrollLog.BorderColor = Color4.Red;

            testScrollLog.AddLine("test 1 test2 test #, test4 te5t %, ; test 77a");
            testScrollLog.AddLine("For score and two tons.");
            testScrollLog.AddLine("Random RNG newt home[oops]");

            KeyboardEventManager.CurrentContext = testTextBox.InputContext;
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused) { return; }

            testTextBox?.Update();
            testScrollLog?.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            ViewMatrixes viewMatrixes = new ViewMatrixes
            {
                Model = Matrix4.Identity,
                Projection = (camera != null) ? camera.Projection : Matrix4.Identity,
                View = (camera != null) ? camera.View : Matrix4.Identity,
                Orthographic = (camera != null) ? camera.Orthographic : Matrix4.Identity,
                UIOrtho = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, ClientSize.Y, 0, -1f, 1f)
            };

            DebugConsole.Instance?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testLabel?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testText?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testTextBox?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testScrollLog?.DrawGL(viewMatrixes, TextureUnit.Texture0);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            ShaderManager.OnUnload();
            TextureManager.OnUnload();
            SceneManager.OnUnload();

            base.OnUnload();
        }
    }
}
