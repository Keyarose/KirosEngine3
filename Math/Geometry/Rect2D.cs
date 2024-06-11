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
    /// Define a 2D rectangle
    /// </summary>
    public struct Rect2D : IEquatable<Rect2D>, IFormattable, IGeometrical
    {
        /// <summary>
        /// The rectangle's vertices defined in clockwise order starting from the upper left
        /// </summary>
        public Vec2[] Vertices = new Vec2[4];

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="verts">The vertices that define the rectangle.</param>
        public Rect2D(Vec2[] verts)
        {//todo: array size check.
            Vertices = verts;
        }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 0 and 1
        /// </summary>
        public readonly Vec2 Edge1 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 1 and 2
        /// </summary>
        public readonly Vec2 Edge2 { get { return Vertices[2] - Vertices[1]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 2 and 3
        /// </summary>
        public readonly Vec2 Edge3 { get { return Vertices[3] - Vertices[2]; } }

        /// <summary>
        /// Edge of the rectangle formed by Vertex 3 and 0
        /// </summary>
        public readonly Vec2 Edge4 { get { return Vertices[0] - Vertices[3]; } }

        /// <summary>
        /// The perimeter of the rectangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length * 2 + Edge2.Length * 2; } }

        /// <summary>
        /// The area of the rectangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(Edge1.AsVec3(), Edge2.AsVec3()).Length; } }

        /// <summary>
        /// Is the rectangle also a square
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
        /// Is the rectangle also a parallelogram
        /// </summary>
        public readonly bool IsParallelogram
        {
            get
            {
                //skew is left right
                if (!Vertices[0].X.CloseTo(Vertices[3].X)) { return true; }
                //skew is up down
                if (!Vertices[0].Y.CloseTo(Vertices[1].Y)) { return true; }

                return false;
            }
        }

        /// <summary>
        /// Check to see if the given point is contained within the rectangle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <remarks>Based on https://math.stackexchange.com/questions/1805724</remarks>
        /// <returns>True if the point is inside the rectangle, false otherwise</returns>
        public readonly bool ContainsPoint(Vec2 point)
        {
            float limit = Area;

            float test1;
            float test2;

            if (limit < 0)
            {
                test1 = (point.Y - Vertices[0].Y) * Edge4.X + (point.X - Vertices[0].X) * Edge4.Y;
                test2 = (point.X - Vertices[0].X) * Edge1.Y + (point.Y - Vertices[0].Y) * Edge1.X;

                if ((0 <= test1 && test1 <= -limit) && (0 <= test2 && test2 <= -limit))
                    return true;
            }
            else
            {
                test1 = (point.X - Vertices[0].X) * Edge4.Y + (point.Y - Vertices[0].Y) * Edge4.X;
                test2 = (point.Y - Vertices[0].Y) * Edge1.X + (point.X - Vertices[0].X) * Edge1.Y;

                if ((0 <= test1 && test1 <= limit) && (0 <= test2 && test2 <= limit))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get the rectangle as an array of 2D triangles
        /// </summary>
        /// <param name="split02">The vertices to split along; True splits along vertex 0 and 2, false 1 and 3</param>
        /// <returns>The rectangle as an array of triangles</returns>
        public readonly Triangle2D[] AsTriangles(bool split02)
        {
            Triangle2D[] result = new Triangle2D[2];

            if (split02) 
            {
                result[0] = new Triangle2D(Vertices[..3]);//verts 0,1,2
                result[1] = new Triangle2D([Vertices[0], Vertices[2], Vertices[3]]);//verts 0,2,3
            }
            else
            {
                result[0] = new Triangle2D([Vertices[0], Vertices[1], Vertices[3]]);//verts 0,1,3
                result[1] = new Triangle2D(Vertices[1..4]);//verts 1,2,3
            }

            return result;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Rect2D rec && Equals(rec);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Rect2D other)
        {
            return Vertices.SequenceEqual(other.Vertices);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Vertices);
        }

        /// <summary>
        /// Define the equivalence operator for a 2D rectangle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if they are equal false otherwise</returns>
        public static bool operator ==(Rect2D lhs, Rect2D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator for a 2D rectangle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if they are equal, true otherwise</returns>
        public static bool operator !=(Rect2D lhs, Rect2D rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public readonly bool IsGeometricallyCorrect([NotNullWhen(false)] out string? message)
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
