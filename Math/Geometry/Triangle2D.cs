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
    /// Define a 2D triangle
    /// </summary>
    public struct Triangle2D : IEquatable<Triangle2D>, IFormattable
    {
        /// <summary>
        /// The vertices of the triangle.
        /// </summary>
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
        /// The angle of the triangle at Vertex 0
        /// </summary>
        public readonly float Angle1 { get { return MathF.Acos(Vec2.Dot(Edge2, Edge3) / (Edge2.Length * Edge3.Length)); } }
        /// <summary>
        /// The angle of the triangle at Vertex 1
        /// </summary>
        public readonly float Angle2 { get { return MathF.Acos(Vec2.Dot(Edge1, Edge3) / (Edge1.Length * Edge3.Length)); } }
        /// <summary>
        /// The angle of the triangle at Vertex 2
        /// </summary>
        public readonly float Angle3 { get { return MathF.Acos(Vec2.Dot(Edge1, Edge2) / (Edge1.Length * Edge2.Length)); } }

        /// <summary>
        /// The perimeter of the triangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length + Edge2.Length + Edge3.Length; } }

        /// <summary>
        /// The area of the triangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(new Vec3(Edge1, 0.0f), new Vec3(Edge2, 0.0f)).Length / 2; } }

        /// <inheritdoc/>
        public readonly bool Equals(Triangle2D other)
        {
            if (other.Vertices.Contains(Vertices[0]) && other.Vertices.Contains(Vertices[1]) && other.Vertices.Contains(Vertices[2]))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Triangle2D t && Equals(t);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Vertices);
        }

        /// <summary>
        /// Define the equivalence operator between two 2D triangles
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the triangles are equal, false otherwise</returns>
        public static bool operator ==(Triangle2D lhs, Triangle2D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator between two 2D triangles
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if the triangles are equal, true otherwise</returns>
        public static bool operator !=(Triangle2D lhs, Triangle2D rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Check if the given point is inside the triangle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the triangle, false otherwise</returns>
        public readonly bool ContainsPoint(Vec2 point)
        {
            Vec3 baryCoords = BarycentricCoords(point);

            if (baryCoords.X < 0.0f || baryCoords.Y < 0.0f || baryCoords.Z < 0.0f)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calculate the barycentric coordinates for the given point
        /// </summary>
        /// <param name="point">The point to calculate for</param>
        /// <returns>The barycentric version of the point</returns>
        public readonly Vec3 BarycentricCoords(Vec2 point) 
        {
            Triangle2D subT1 = new Triangle2D([Vertices[1], Vertices[2], point]);//sub-triangle on edge 1
            Triangle2D subT2 = new Triangle2D([Vertices[2], Vertices[0], point]);//sub-triangle on edge 2

            float area = Area;//cache locally so it is only calculated once

            float x = subT1.Area / area;
            float y = subT2.Area / area;

            return new Vec3(x, y, 1.0f - x - y);
        }

        /// <summary>
        /// The center of gravity for the triangle
        /// </summary>
        /// <returns>A 2D point representing the center of gravity</returns>
        public readonly Vec2 CenterOfGrav()
        {
            return (Vertices[0] + Vertices[1] + Vertices[2]) / 3.0f;
        }

        /// <summary>
        /// The incenter of the triangle
        /// </summary>
        /// <returns>A 2D point representing the triangle's incenter</returns>
        public readonly Vec2 Incenter()
        {
            return (Vertices.Length * Vertices[0] + Vertices.Length * Vertices[1] + Vertices.Length * Vertices[2]) / Perimeter;
        }

        /// <summary>
        /// The circle that fits within the triangle at the incenter
        /// </summary>
        /// <returns>A 2D circle</returns>
        public readonly Circle2D IncenterCircle()
        {
            return new Circle2D(Area / Perimeter, Incenter());
        }

        /// <summary>
        /// The circumcenter, the center of the circle that fits the vertices of the triangle
        /// </summary>
        /// <param name="circumradius">The radius of the circle</param>
        /// <returns>A 2D point representing the circumcenter</returns>
        public readonly Vec2 Circumcenter(out float circumradius)
        {
            float d1 = Vec2.Dot(-Edge2, Edge3);
            float d2 = Vec2.Dot(-Edge3, Edge1);
            float d3 = Vec2.Dot(-Edge1, Edge2);

            float c1 = d2 * d3;
            float c2 = d1 * d3;
            float c3 = d2 * d1;
            float c = c1 + c2 + c3;

            circumradius = MathF.Sqrt((d1 + d2) * (d2 + d3) * (d3 + d1) / c) / 2.0f;

            return (((c2 + c3) * Vertices[0]) + ((c3 + c1) * Vertices[1]) + ((c1 + c2) * Vertices[2])) / (2.0f * c);
        }

        /// <summary>
        /// The circle that fits all three of the triangle's vertices
        /// </summary>
        /// <returns>A 2D circle</returns>
        public readonly Circle2D CircumcenterCircle()
        {
            Vec2 center = Circumcenter(out float radius);
            return new Circle2D(radius, center);
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
            return string.Format("Triangle2D \n\tVertices: {0}, \n\t\t{1}, \n\t\t{2}",
                Vertices[0].ToString(format, formatProvider),
                Vertices[1].ToString(format, formatProvider),
                Vertices[2].ToString(format, formatProvider));
        }
        #endregion
    }
}
