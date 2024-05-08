using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Mesh
{
    /// <summary>
    /// Defines an object as being able to be rendered to the screen
    /// </summary>
    public interface IRenderable
    {
        public void DrawGL();

        public ColorVertex[] GetVertexData();

        public PrimitiveType GetDrawMode();

        public void DrawDX();
    }
}
