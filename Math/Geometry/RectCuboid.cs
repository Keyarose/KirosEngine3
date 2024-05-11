using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// Defines a 3D Rectangular Cuboid that can be a Rectangular Prism, Cube, or Parallelepiped
    /// </summary>
    public struct RectCuboid : IEquatable<RectCuboid>, IFormattable, IGeometrical
    {
        /// <summary>
        /// The Cuboid's vertices defined in clockwise order from the front face perspective, front face then back face
        /// </summary>
        public Vec3[] Vertices = new Vec3[8];

        #region Edges
        /// <summary>
        /// Edge of the Cuboid formed by Vertex 0 and 1, Front face and Top face joining edge
        /// </summary>
        public readonly Vec3 Edge1 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 1 and 2, Front face and Right face joining edge
        /// </summary>
        public readonly Vec3 Edge2 { get { return Vertices[2] - Vertices[1]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 2 and 3, Front face and Bottom face joining edge
        /// </summary>
        public readonly Vec3 Edge3 { get { return Vertices[3] - Vertices[2]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 3 and 0, Front face and Left face joining edge
        /// </summary>
        public readonly Vec3 Edge4 { get { return Vertices[0] - Vertices[3]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 4 and 5, Back face and Top face joining edge
        /// </summary>
        public readonly Vec3 Edge5 { get { return Vertices[4] - Vertices[5]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 4 and 7, Back face and Left face joining edge
        /// </summary>
        public readonly Vec3 Edge6 { get { return Vertices[7] - Vertices[4]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 6 and 7, Back face and Bottom face joining edge
        /// </summary>
        public readonly Vec3 Edge7 { get { return Vertices[6] - Vertices[7]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 5 and 6, Back face and Right face joining edge
        /// </summary>
        public readonly Vec3 Edge8 { get { return Vertices[5] - Vertices[6]; } }
        
        /// <summary>
        /// Edge of the Cuboid formed by Vertex 1 and 5, Right face and Top face joining edge
        /// </summary>
        public readonly Vec3 Edge9 { get { return Vertices[5] - Vertices[1]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 2 and 6, Right face and Bottom face joining edge
        /// </summary>
        public readonly Vec3 Edge10 { get { return Vertices[2] - Vertices[6]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 0 and 4, Left face and Top face joining edge
        /// </summary>
        public readonly Vec3 Edge11 { get { return Vertices[0] - Vertices[4]; } }

        /// <summary>
        /// Edge of the Cuboid formed by Vertex 3 and 7, Left face and Bottom face joining edge
        /// </summary>
        public readonly Vec3 Edge12 { get { return Vertices[7] - Vertices[3]; } }
        #endregion

        #region Faces
        /// <summary>
        /// The front face of the Cuboid
        /// </summary>
        public readonly Rect3D FrontFace { get { return new Rect3D(Vertices[..4]); } }

        /// <summary>
        /// The back face of the Cuboid
        /// </summary>
        public readonly Rect3D BackFace { get { return new Rect3D(Vertices[4..]); } }

        /// <summary>
        /// The Right face of the Cuboid
        /// </summary>
        public readonly Rect3D RightFace { get { return new Rect3D([Vertices[1], Vertices[5], Vertices[6], Vertices[2]]); } }

        /// <summary>
        /// The Left face of the Cuboid
        /// </summary>
        public readonly Rect3D LeftFace { get { return new Rect3D([Vertices[4], Vertices[0], Vertices[3], Vertices[7]]); } }

        /// <summary>
        /// The Top face of the Cuboid
        /// </summary>
        public readonly Rect3D TopFace { get { return new Rect3D([Vertices[4], Vertices[5], Vertices[1], Vertices[0]]); } }

        /// <summary>
        /// The Bottom face of the Cuboid
        /// </summary>
        public readonly Rect3D BottomFace { get { return new Rect3D([Vertices[3], Vertices[2], Vertices[6], Vertices[7]]); } }
        #endregion

        /// <summary>
        /// The volume of the Cuboid
        /// </summary>
        public readonly float Volume { get { return Edge1.Length * Edge2.Length * Edge3.Length; } }

        /// <summary>
        /// The surface area of the Cuboid
        /// </summary>
        public readonly float SurfaceArea { get { return FrontFace.Area + BackFace.Area + RightFace.Area + LeftFace.Area + TopFace.Area + BottomFace.Area; } }

        /// <summary>
        /// Is the Cuboid a Cube
        /// </summary>
        public readonly bool IsCube
        {
            get
            {
                if (Edge1.Length == Edge2.Length && Edge1.Length == Edge9.Length)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Construct a Cuboid from a set of 8 vertices
        /// </summary>
        /// <param name="verts">The eight vertices to use</param>
        /// <exception cref="ArgumentException">Thrown if the number of vertices is not 8.</exception>
        public RectCuboid(Vec3[] verts)
        {
            if (verts.Length != 8)
                throw new ArgumentException(string.Format("RectCuboid requires 8 vertices."));

            Vertices = verts;
        }

        /// <summary>
        /// Construct a Cuboid from a set of faces
        /// </summary>
        /// <param name="faces">The set of faces to use</param>
        /// <exception cref="NotImplementedException"></exception>
        public RectCuboid(Rect3D[] faces)
        {
            throw new NotImplementedException();//todo: face constructor
        }

        /// <inheritdoc/>
        public readonly bool IsGeometricallyCorrect([NotNullWhen(false)]out string? message)
        {
            if (FrontFace.IsGeometricallyCorrect(out string? fm))
            {
                message = "RectCuboid is not geometrically correct as the front face is not: ";
                message += fm;
                return false;
            }

            if (BackFace.IsGeometricallyCorrect(out fm))
            {
                message = "RectCuboid is not geometrically correct as the back face is not: ";
                message += fm;
                return false;
            }

            if (RightFace.IsGeometricallyCorrect(out fm))
            {
                message = "RectCuboid is not geometrically correct as the right face is not: ";
                message += fm;
                return false;
            }

            if (LeftFace.IsGeometricallyCorrect(out fm))
            {
                message = "RectCuboid is not geometrically correct as the left face is not: ";
                message += fm;
                return false;
            }

            if (TopFace.IsGeometricallyCorrect(out fm))
            {
                message = "RectCuboid is not geometrically correct as the top face is not: ";
                message += fm;
                return false;
            }

            if (BottomFace.IsGeometricallyCorrect(out fm))
            {
                message = "RectCuboid is not geometrically correct as the bottom face is not: ";
                message += fm;
                return false;
            }

            message = null;
            return true;
        }

        #region Operators
        /// <inheritdoc/>
        public readonly bool Equals(RectCuboid other)
        {
            return Vertices.SequenceEqual(other.Vertices);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is RectCuboid cube && Equals(cube);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Vertices);
        }

        /// <summary>
        /// Define the equivalence operator for a Cuboid
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public static bool operator ==(RectCuboid lhs, RectCuboid rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator for a Cuboid
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if they are equal, true otherwise</returns>
        public static bool operator !=(RectCuboid lhs, RectCuboid rhs)
        {
            return !lhs.Equals(rhs);
        }
        #endregion

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
            string result = "Cuboid \n\tVertices: ";
            //todo: explore using format to choose between writing verts, and faces
            foreach (var vert in Vertices)
            {
                result += string.Format("{0}, \n\t\t", vert.ToString(format, formatProvider));
            }

            return result;
        }
        #endregion
    }
}
