using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    public struct Triangle2D
    {
        public Vec2[] Vertices = new Vec2[3];
        
        /// <summary>
        /// Constructor for a 2D triangle
        /// </summary>
        /// <param name="verts">The triangle's vertices</param>
        public Triangle2D(Vec2[] verts)
        {
            Vertices = verts;
        }

        /// <summary>
        /// The edge of the triangle opposite Vertex 0
        /// </summary>
        public readonly Vec2 Edge1 { get { return Vertices[2] - Vertices[1]; } }
        /// <summary>
        /// The edge of the triangle opposite Vertex 1
        /// </summary>
        public readonly Vec2 Edge2 { get { return Vertices[0] - Vertices[2]; } }
        /// <summary>
        /// The edge of the triangle opposite Vertex 2
        /// </summary>
        public readonly Vec2 Edge3 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// The perimeter of the triangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length + Edge2.Length + Edge3.Length; } }

        /// <summary>
        /// The area of the triangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(new Vec3(Edge1, 0.0f), new Vec3(Edge2, 0.0f)).Length / 2; } }
    }
}
