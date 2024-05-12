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
        public void DrawGL(ViewMatrixes vm);

        /// <summary>
        /// Get the renderable's vertex data as an array of ColorVertex
        /// </summary>
        /// <returns>The vertex data</returns>
        public ColorVertex[] GetVertexData();

        /// <summary>
        /// Get the renderable's draw mode
        /// </summary>
        /// <returns>The draw mode to be used in drawing</returns>
        public PrimitiveType GetDrawMode();

        public void DrawDX();
    }
}
