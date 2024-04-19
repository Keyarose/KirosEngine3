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
        BaseCamera? camera;

        public TestClient(int width, int height) : base (width, height, "Test Client")
        {
            ConfigVars.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);
            ConfigVars.AddVar(DEFAULT_FONT_NAME_KEY, "default");
            ConfigVars.AddVar(DEFAULT_FONT_FILE_KEY, "Resources/Fonts/latin_sas_math_16pt");

            FontManager.AddFont((string)ConfigVars.Instance[DEFAULT_FONT_NAME_KEY], new Font((string)ConfigVars.Instance[DEFAULT_FONT_NAME_KEY],
            (string)ConfigVars.Instance[DEFAULT_FONT_FILE_KEY] + ".xml",
            (string)ConfigVars.Instance[DEFAULT_FONT_FILE_KEY] + "_0.png"));//todo: cleanup call once config system is implemented
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 0.1f);

            camera = new BaseCamera(-Vec3.UnitZ, ClientSize.X, ClientSize.Y);

            ShaderManager.CreateShader("color", "Resources/Shaders/ColorShader.vert", "Resources/Shaders/ColorShader.frag");

            ShaderManager.CreateShader("text", "Resources/Shaders/FontShader_default.vert", "Resources/Shaders/FontShader_default.frag");

            _ = FontManager.TryGetFont(DEFAULT_FONT_NAME_KEY, out Font? df);

            testText = new Text(new Vec2(0.0f), df!, "t");
            testPoint = new Point(new Vec3(0.0f, 0.5f, 0.0f), Color4.Yellow);

            testLine = new Line(Vec3.Zero, new Vec3(0.5f, 0.5f, 0.0f), Color4.Red);
            testLine.Init();

            testTriangle = new Triangle([new Vec3(0.0f, 0.3f, 0.0f), new Vec3(0.2f, -0.2f, 0.0f), new Vec3(-0.2f, -0.2f, 0.0f)], Color4.Aqua);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if(!IsFocused) { return; }
            
            if(KeyboardState.IsKeyDown(Keys.Escape)) { Close(); }

            if(KeyboardState.IsKeyDown(Keys.Up)) { testLine!.End += new Vec3(0.0f, 0.001f, 0.0f); }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            ViewMatrixes viewMatrixes = new ViewMatrixes
            {
                Model = Matrix4.Identity,
                Projection = (camera != null) ? camera.Projection : Matrix4.Identity,
                View = (camera != null) ? camera.View : Matrix4.Identity,
                Orthographic = (camera != null) ? camera.Orthographic : Matrix4.Identity
            };

            testTriangle?.DrawGL();
            testPoint?.DrawGL();
            testLine?.DrawGL();
            

            //testText?.Draw(viewMatrixes, TextureUnit.Texture0);

            SwapBuffers();
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
