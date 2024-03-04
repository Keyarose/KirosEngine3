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
            RuntimeVars.Instance.AddVar(GRAPHICSMODE_KEY, GRAPHICSMODE_GL_VAL);
        }
    }
}
