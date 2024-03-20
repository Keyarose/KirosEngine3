using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    public struct Triangle3D : IFormattable
    {
        public Vec3[] Vertices = new Vec3[3];
        public Vec3 Normal;

        /// <summary>
        /// Constructor for when only the vertices are provided
        /// </summary>
        /// <param name="verts">The triangle's vertices</param>
        public Triangle3D(Vec3[] verts) 
        {
            Vertices = verts;

            Normal = Vec3.Cross(Edge3, Edge1).NormalizedCopy();

#if DEBUG
            if (!Normal.IsFinite()) //unsure if it's needed so its debug only for now
            {
                Logger.WriteToLog("Triangle normal calculation resulted in a nonfinite value.");
                Logger.WriteToLog(string.Format("Triangle data: {0}, {1}, {2}", verts[0], verts[1], verts[2]));
            }
#endif
        }

        /// <summary>
        /// Constructor for when the normal is provided
        /// </summary>
        /// <param name="verts">The triangle's vertices</param>
        /// <param name="norm">The triangle's surface normal</param>
        public Triangle3D(Vec3[] verts, Vec3 norm)
        {
            Vertices = verts;
            Normal = norm;
        }

        /// <summary>
        /// Construct a 3D triangle from a 2D triangle
        /// </summary>
        /// <param name="tri"></param>
        public Triangle3D(Triangle2D tri) : this(Tri2DToVec3Array(tri)) { }

        /// <summary>
        /// Convert a 2D triangle into an array of 3D vectors to use in a 3D triangle
        /// </summary>
        /// <param name="tri"></param>
        /// <returns></returns>
        private static Vec3[] Tri2DToVec3Array(Triangle2D tri)
        {
            Vec3[] verts = new Vec3[3];
            for (int i = 0; i < 3; i++)
            {
                verts[i] = new Vec3(tri.Vertices[i], 0.0f);
            }
            return verts;
        }

        /// <summary>
        /// The edge of the triangle opposite Vertex 0
        /// </summary>
        public readonly Vec3 Edge1 { get { return Vertices[2] - Vertices[1]; } }
        /// <summary>
        /// The edge of the triangle opposite Vertex 1
        /// </summary>
        public readonly Vec3 Edge2 { get { return Vertices[0] - Vertices[2]; } }
        /// <summary>
        /// The edge of the triangle opposite Vertex 2
        /// </summary>
        public readonly Vec3 Edge3 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// The perimeter of the triangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length + Edge2.Length + Edge3.Length; } }

        /// <summary>
        /// The area of the triangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(Edge1, Edge2).Length / 2; } }

        /// <summary>
        /// The Plane the triangle exists in
        /// </summary>
        public readonly Plane TriPlane { get { return new Plane(Vertices); } }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return ToString(null, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public readonly string ToString(string? format)
        {
            return ToString(format, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public readonly string ToString(IFormatProvider? formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <inheritdoc/>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            return string.Format("{0}, {1}, {2}", Vertices[0].ToString(format, formatProvider),
                Vertices[1].ToString(format, formatProvider),
                Vertices[2].ToString(format, formatProvider));
        }
    }
}
