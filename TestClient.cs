using KirosEngine3.Scenes;
using KirosEngine3.Shaders;
using KirosEngine3.Textures;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3
{
    /// <summary>
    /// Testing client keep the original client class as simple and clean as possible
    /// </summary>
    internal class TestClient : Client
    {
        public TestClient(int width, int height) : base (width, height, "Test Client")
        {
            RuntimeVars.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            ShaderManager.OnUnload();
            TextureManager.OnUnload();
            SceneManager.OnUnload();
        }
    }
}
