using KirosEngine3.Camera;
using KirosEngine3.Config;
using KirosEngine3.Input;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Scenes;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using KirosEngine3.UI;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace KirosEngine3
{
    /// <summary>
    /// Testing client keep the original client class as simple and clean as possible
    /// </summary>
    internal class TestClient : Client
    {
        Text? testText;
        Cube? testCube;
        Sphere? testSphere;
        Label? testLabel;

        TexturedQuad? testTexQ;
        CoordinateGrid? testGrid;
        CoordinateGrid? testGridXZ;
        CoordinateGrid? testGridYZ;
        BaseCamera? camera;

        ScreenButton? testSButton;
        TextBox? testTextBox;

        public TestClient(int width, int height) : base(width, height, "Test Client")
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

            //test stuff zone
            TextureManager.TryAddTexture("wall", "Resources/Textures/wall.jpg");//debug texture
            //end test stuff


            GL.ClearColor((Color)new Color4(ConfigManager.Instance[ConfigKeys.D_CLEAR_COLOR_KEY]));//set clear color from config
            GL.Enable(EnableCap.DepthTest);
            //GL.Enable(EnableCap.DebugOutput);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);//wireframe drawing

            //system control setup
            KeyboardEventManager.CurrentContext = "system";
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.Escape,
                KeyboardEventType.KeyPressed, (object sender, KeyboardEventArgs args) => { Close(); });

            Vec3 moveC = new Vec3(1.0f, 1.0f, 0.0f);//todo: manual camera movement remove later
            camera = new BaseCamera(2.0f * Vec3.UnitZ + moveC, ClientSize.X, ClientSize.Y, .5f);
            camera.LookAt = Vec3.Zero;

            //load test text
            testText = new Text(new Vec2(0.0f, 0.0f), "test wrap");
            testText.Size = new Vec2(80f, 30f);
            //testText.Color = Color4.Red;
            testText.Init();

            testCube = Cube.UnitCube;
            testCube.SetColors([Color4.Red, Color4.Blue, Color4.Green, Color4.Yellow]);
            testCube.Init("color");

            testSphere = new Sphere(Vec3.Zero, 1.0f, SphereType.UVSphere, 12, 22, "pos");
            testSphere.Init("pos");
            testSphere.Color = Color4.Blue;

            testLabel = new Label(new(400f, 300f), "Label Test.");
            testLabel.Init();

            testGrid = CoordinateGrid.UnitGridXY;
            testGrid.Init();

            testGridXZ = CoordinateGrid.UnitGridXZ;
            testGridXZ.Init();

            //testGridXZ.Rotation = Matrix4.CreateRotationZ(MathF.PI / 2);

            testGridYZ = CoordinateGrid.UnitGridYZ;
            testGridYZ.Init();

            testSButton = new ScreenButton(new Vec2(100.0f, 0.0f), Color4.Yellow, new Vec2(200f, 300f), "color");
            testSButton.Init();

            testTextBox = new TextBox(new Vec2(2.0f, 0f), new Vec2(798f, 300f), "inputbox", FontManager.Default, 50);
            testTextBox.Init();

            KeyboardEventManager.CurrentContext = testTextBox.InputContext;

            //testTexQ = new TexturedQuad(Quad.UnitQuad, "wall");
            testTexQ = new TexturedQuad([new(0f, 0f, 0f), new(200f, 0f, 0f), new(200f, 200f, 0f), new(0f, 200f, 0f)], [0, 1, 2, 2, 3, 0], "defaultFont");
            testTexQ.Init();

            //kem testing
            // KeyboardEventManager.SubscribeKeyboardEvent("system", Keys.B, KeyboardEventType.KeyHeld, (object sender, KeyboardEventArgs args) => { testLine!.End += new Vec3(0.0f, 0.001f, 0.0f); });

            //ToString testing
            //Console.WriteLine(testQuad.ToString());
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused) { return; }
            //check keyboard state and notify subscribers
            KeyboardEventManager.Update(KeyboardState, args.Time);

            testTextBox?.Update();
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

            testCube?.DrawGL(viewMatrixes);
            //testSphere?.DrawGL(viewMatrixes);

            //testGrid?.DrawGL(viewMatrixes);
            //testGridXZ?.DrawGL(viewMatrixes);
            //testGridYZ?.DrawGL(viewMatrixes);

            //testSButton?.DrawGL(viewMatrixes);
            //testTexQ?.DrawGL(viewMatrixes, TextureUnit.Texture1);

            //hud and 2d
            testText?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testLabel?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            testTextBox?.DrawGL(viewMatrixes, TextureUnit.Texture0);
            //end hud and 2d

            SwapBuffers();
        }

        /// <summary>
        /// Handle window resize events
        /// </summary>
        /// <param name="e">The resize event args</param>
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);//todo: explore usages, or ignore in favor of an agnostic method?
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
