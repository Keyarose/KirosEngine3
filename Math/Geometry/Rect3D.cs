using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// A mathematical representation of a rectangle in 3D space.
    /// </summary>
    public struct Rect3D : IEquatable<Rect3D>, IFormattable, IGeometrical
    {
        /// <summary>
        /// The rectangle's vertices defined in clockwise order starting from the upper left
        /// </summary>
        public Vec3[] Vertices = new Vec3[4];

        /// <summary>
        /// The plane the rectangle exists in
        /// </summary>
        public readonly Plane RecPlane
        {
            get
            {
                return new Plane(Vertices[..3]);
            }
        }

        /// <summary>
        /// The normal of the rectangle
        /// </summary>
        public Vec3 Normal { get { return RecPlane.Normal; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 0 and 1
        /// </summary>
        public readonly Vec3 Edge1 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 1 and 2
        /// </summary>
        public readonly Vec3 Edge2 { get { return Vertices[2] - Vertices[1]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 2 and 3
        /// </summary>
        public readonly Vec3 Edge3 { get { return Vertices[3] - Vertices[2]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 3 and 0
        /// </summary>
        public readonly Vec3 Edge4 { get { return Vertices[0] - Vertices[3]; } }

        /// <summary>
        /// The perimeter of the rectangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length * 2 + Edge2.Length * 2; } }

        /// <summary>
        /// The area of the rectangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(Edge1, Edge2).Length; } }

        /// <summary>
        /// Is the rectangle a square
        /// </summary>
        public readonly bool IsSquare
        {
            get
            {
                if (Edge1.Length == Edge2.Length) { return true; }

                return false;
            }
        }

        /// <summary>
        /// Is the rectangle a parallelogram
        /// </summary>
        public readonly bool IsParallelogram
        {
            get
            {
                //skew is left right
                if (!Edge2.IsPerpendicular(Edge3)) { return true; }
                //skew is up down
                if(!Edge1.IsPerpendicular(Edge2)) { return true; }

                return false;
            }
        }

        /// <summary>
        /// Basic constructor for Rect3D
        /// </summary>
        /// <param name="verts">The list of vertices to define the rectangle, minimum array size of 4, extra are discarded</param>
        /// <exception cref="ArgumentException">Thrown if the array of vertices is too small</exception>
        public Rect3D(Vec3[] verts)
        {
            if (verts.Length != 4)
                throw new ArgumentException(string.Format("Rect3D requires 4 vertices."));

            Vertices = verts;
        }

        /// <summary>
        /// Construct a 3D rectangle from a 2D rectangle
        /// </summary>
        /// <param name="rect">The 2D rectangle to construct from</param>
        public Rect3D(Rect2D rect)
        {
            //for (int i = 0; i < Vertices.Length; i++)
            //{
            //    Vertices[i] = rect.Vertices[i].AsVec3();
            //}

            //todo: needs testing
            Vertices = Vertices.Zip(rect.Vertices, (v3, v2) => v2.AsVec3()).ToArray();
        }

        /// <summary>
        /// Get the rectangle as an array of 3D triangles
        /// </summary>
        /// <param name="split02">The vertices to split along; true splits along vertex 0 and 2, false 1 and 3</param>
        /// <returns>The rectangle as an array of triangles</returns>
        public readonly Triangle3D[] AsTriangles(bool split02)
        {
            Triangle3D[] result = new Triangle3D[2];

            if (split02) 
            {
                result[0] = new Triangle3D(Vertices[..3]);//verts 0,1,2
                result[1] = new Triangle3D([Vertices[0], Vertices[2], Vertices[3]]);//verts 0,2,3
            }
            else
            {
                result[0] = new Triangle3D([Vertices[0], Vertices[1], Vertices[3]]);//verts 0,1,3
                result[1] = new Triangle3D(Vertices[1..4]);//verts 1,2,3
            }

            return result;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Rect3D rec && Equals(rec);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Rect3D other)
        {
            return Vertices.SequenceEqual(other.Vertices);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Vertices);
        }

        /// <summary>
        /// Define the equivalence operator for a 3D rectangle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if they are equal false otherwise</returns>
        public static bool operator ==(Rect3D lhs, Rect3D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator for a 3D rectangle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if they are equal, true otherwise</returns>
        public static bool operator !=(Rect3D lhs, Rect3D rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public readonly bool IsGeometricallyCorrect([NotNullWhen(false)]out string? message)
        {
            //check that edges are parallel or perp as needed
            if (!Edge1.IsParallel(Edge3) || !Edge2.IsParallel(Edge4))
            {
                message = "Rectangle is not geometrically correct as the edges are not parallel to their opposites.";
                return false;
            }

            if (Edge1.Length != Edge3.Length || Edge2.Length != Edge4.Length)
            {
                message = "Rectangle is not geometrically correct as the opposite edges are different sizes."; ;
                return false;
            }

            message = null;
            return true;
        }

        #region ToString
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
            return string.Format("Rect3D \n\tVertices: {0}, \n\t\t{1}, \n\t\t{2}, \n\t\t{3}", 
                Vertices[0].ToString(format, formatProvider), 
                Vertices[1].ToString(format, formatProvider), 
                Vertices[2].ToString(format, formatProvider), 
                Vertices[3].ToString(format, formatProvider));
        }
        #endregion
    }
}
