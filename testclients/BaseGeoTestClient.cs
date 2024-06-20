using KirosEngine3.Camera;
using KirosEngine3.Config;
using KirosEngine3.Input;
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Matrix;
using KirosEngine3.Math.Vector;
using KirosEngine3.Mesh.Primitives;
using KirosEngine3.Scenes;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if DEBUG
namespace KirosEngine3.testclients
{
    /// <summary>
    /// Test client for basic geometry.
    /// </summary>
    internal class BaseGeoTestClient : Client
    {
        Point? testPoint;
        Line? testLine;
        Triangle? testTriangle;
        Quad? testQuad;

        BaseCamera? camera;

        public BaseGeoTestClient(int width, int height) : base (width, height, "Base Geometry Test")
        {
            ConfigManager.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);
            if (!ConfigManager.LoadFromXML("Resources/Config/generalConfig.xml"))
            {
                //config fallback
            }


        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 0.1f);

            //setup system key events
            KeyboardEventManager.CurrentContext = "system";
            KeyboardEventManager.SubscribeKeyboardEvent(KeyboardEventManager.GLOBAL_CONTEXT, Keys.Escape, KeyboardEventType.KeyPressed, (object sender, KeyboardEventArgs args) => { Close(); });

            //load camera
            camera = new BaseCamera(2.0f * Vec3.UnitZ, ClientSize.X, ClientSize.Y);
            camera.LookAt = Vec3.Zero;

            //load geometry
            testPoint = new Point(new(0.5f, 0.5f, -1f), Color4.Red);
            testPoint.Init();

            testLine = new Line(new(-0.7f, 1f, 0f), new(-2.5f, -2.1f, -0.8f), Color4.Black);
            testLine.Init();

            testTriangle = new Triangle([new(0f, 1f, 0f), new(0f, -0.5f, 0f), new(-0.9f, 0.2f, 0f)], Color4.Green);
            testTriangle.Init();

            testQuad = Quad.UnitQuad;
            testQuad.Init();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (!IsFocused) return;//if the window is not in focus do nothing

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
                Orthographic = (camera != null) ? camera.Orthographic : Matrix4.Identity,
                UIOrtho = Matrix4.CreateOrthographicOffCenter(0, ClientSize.X, ClientSize.Y, 0, -1f, 1f)
            };

            //draw point
            testPoint?.DrawGL(viewMatrixes);

            //draw line
            testLine?.DrawGL(viewMatrixes);

            //draw triangle
            testTriangle?.DrawGL(viewMatrixes);

            //draw quad
            viewMatrixes.Model = Matrix4.CreateTranslation(1f, -1f, 0f);
            testQuad?.DrawGL(viewMatrixes);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            testPoint?.Dispose();
            testLine?.Dispose();
            testTriangle?.Dispose();
            testQuad?.Dispose();

            ShaderManager.OnUnload();
            TextureManager.OnUnload();
            SceneManager.OnUnload();

            base.OnUnload();
        }
    }
}
#endif