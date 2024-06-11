using KirosEngine3.Scenes;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using KirosEngine3.Math.Vector;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Camera;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Math.Data;
using KirosEngine3.Input;
using KirosEngine3.Config;
using KirosEngine3.Mesh;
using KirosEngine3.UI;

namespace KirosEngine3
{
    /// <summary>
    /// Testing client keep the original client class as simple and clean as possible
    /// </summary>
    internal class TestClient : Client
    {
        Text? testText;
        Point? testPoint;
        Line? testLine;
        Triangle? testTriangle;
        Quad? testQuad;
        Cube? testCube;
        Sphere? testSphere;

        TexturedQuad? testTexQ;
        CoordinateGrid? testGrid;
        CoordinateGrid? testGridXZ;
        CoordinateGrid? testGridYZ;
        BaseCamera? camera;

        ScreenButton? testSButton;

        public TestClient(int width, int height) : base (width, height, "Test Client")
        {
            ConfigVars.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);
            if (!ConfigVars.LoadFromXML("Resources/Config/generalConfig.xml"))
            {
                //failed to load general config perform fallback
            }

            TextureManager.TryAddTexture("defaultFont", ConfigVars.Instance[ConfigKeys.D_FONT_FILE_KEY] + "_0.png");//todo: move font texture loading into font
            FontManager.AddFont(ConfigVars.Instance[ConfigKeys.D_FONT_NAME_KEY], new Font(ConfigVars.Instance[ConfigKeys.D_FONT_NAME_KEY],
            ConfigVars.Instance[ConfigKeys.D_FONT_FILE_KEY] + ".xml",
            "defaultFont"));//todo: cleanup method call once config system is implemented
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            //test stuff zone
            TextureManager.TryAddTexture("wall", "Resources/Textures/wall.jpg");//debug texture
            //end test stuff

            GL.ClearColor(0.2f, 0.3f, 0.3f, 0.1f);
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

            _ = FontManager.TryGetFont(ConfigVars.Instance[ConfigKeys.D_FONT_NAME_KEY], out Font? df);//todo: need better configvars access

            //load test text
            {
                testText = new Text(new Vec2(200.0f, 0.0f), df!, "test");
                //testText.Color = Color4.Red;
                testText.Init();
            }

            //load test point
            {
                testPoint = new Point(new Vec3(0.0f, 0.5f, 0.0f), Color4.Yellow);
                testPoint.Init();
            }


            testLine = new Line(Vec3.Zero, new Vec3(0.5f, 0.5f, 0.0f), Color4.Red);
            testLine.Init();

            testTriangle = new Triangle([new Vec3(0.0f, 0.3f, 0.0f), new Vec3(0.2f, -0.2f, 0.0f), new Vec3(-0.2f, -0.2f, 0.0f)], Color4.Aqua);
            testTriangle.Init();

            testQuad = Quad.UnitQuad;
            testQuad.Init();

            testCube = Cube.UnitCube;
            testCube.SetColors([Color4.Red, Color4.Blue, Color4.Green, Color4.Yellow]);
            testCube.Init("color");

            testSphere = new Sphere(Vec3.Zero, 1.0f, SphereType.UVSphere, 12, 22, "pos");
            testSphere.Init("pos");
            testSphere.Color = Color4.Blue;

            testGrid = CoordinateGrid.UnitGridXY;
            testGrid.Init();

            testGridXZ = CoordinateGrid.UnitGridXZ;
            testGridXZ.Init();
            
            //testGridXZ.Rotation = Matrix4.CreateRotationZ(MathF.PI / 2);

            testGridYZ = CoordinateGrid.UnitGridYZ;
            testGridYZ.Init();

            testSButton = new ScreenButton(new Vec2(100.0f, 0.0f), Color4.Yellow, new Vec2(200f, 300f), "color");
            testSButton.Init();

            //testTexQ = new TexturedQuad(Quad.UnitQuad, "wall");
            testTexQ = new TexturedQuad([new(0f, 0f, 0f), new(200f, 0f, 0f), new(200f, 200f, 0f), new(0f, 200f, 0f)], [0, 1, 2, 2, 3, 0], "defaultFont");
            testTexQ.Init();

            //kem testing
            KeyboardEventManager.SubscribeKeyboardEvent("system", Keys.B,
                KeyboardEventType.KeyHeld, (object sender, KeyboardEventArgs args) => { testLine!.End += new Vec3(0.0f, 0.001f, 0.0f); });

            //ToString testing
            //Console.WriteLine(testQuad.ToString());
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if(!IsFocused) { return; }
            //check keyboard state and notify subscribers
            KeyboardEventManager.Update(KeyboardState, args.Time);
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
                Orthographic = (camera != null) ? camera.Orthographic : Matrix4.Identity
            };

            //testTriangle?.DrawGL(viewMatrixes);
            //testPoint?.DrawGL(viewMatrixes);
            //testLine?.DrawGL(viewMatrixes);
            //testQuad?.DrawGL(viewMatrixes);
            testCube?.DrawGL(viewMatrixes);
            //testSphere?.DrawGL(viewMatrixes);

            //testGrid?.DrawGL(viewMatrixes);
            //testGridXZ?.DrawGL(viewMatrixes);
            //testGridYZ?.DrawGL(viewMatrixes);

            //testSButton?.DrawGL(viewMatrixes);
            //testTexQ?.DrawGL(viewMatrixes, TextureUnit.Texture1);
            
            //testText?.DrawGL(viewMatrixes, TextureUnit.Texture0);

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
            testPoint?.Dispose();

            ShaderManager.OnUnload();
            TextureManager.OnUnload();
            SceneManager.OnUnload();

            base.OnUnload();
        }
    }
}
