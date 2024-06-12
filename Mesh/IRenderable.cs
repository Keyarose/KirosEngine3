using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Mesh
{
    /// <summary>
    /// Defines an object as being able to be rendered to the screen.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Draw using the OpenGL API.
        /// </summary>
        /// <param name="vm">The view matrices to use.</param>
        public void DrawGL(ViewMatrixes vm);

        /// <summary>
        /// Get the renderable's vertex data as an array of ColorVertex.
        /// </summary>
        /// <returns>The vertex data.</returns>
        public ColorVertex[] GetVertexData();

        /// <summary>
        /// Get the renderable's draw mode.
        /// </summary>
        /// <returns>The draw mode to be used in drawing.</returns>
        public PrimitiveType GetDrawMode();

        /// <summary>
        /// Draw using the DirectX API.
        /// </summary>
        public void DrawDX();
    }
}
