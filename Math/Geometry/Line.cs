using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace KirosEngine3.Math.Geometry
{
    public struct Line : IEquatable<Line>, IFormattable
    {
        /// <summary>
        /// Start point of the line
        /// </summary>
        public Vec3 Start;//(x0, y0, z0)

        /// <summary>
        /// Direction of the line
        /// </summary>
        public Vec3 Direction;//(a, b, c)

        /// <summary>
        /// Flag to define the line as finite or not
        /// </summary>
        public bool Finite;

        /// <summary>
        /// The length of the line, infinite if the line is not finite
        /// </summary>
        public readonly float Length
        {
            get
            {
                if (Finite) { return Vec3.Distance(Start, Direction); }

                return float.PositiveInfinity;
            }
        }

        /// <summary>
        /// Basic constructor for a line
        /// </summary>
        /// <param name="start">A starting point on the line</param>
        /// <param name="dir">A directional vector to define the line's direction</param>
        /// <remarks>Line formula: [x, y, z] = [x0, y0, z0] + t[a, b, c]</remarks>
        public Line(Vec3 start, Vec3 dir)
        {
            Start = start;
            Direction = dir;
            Finite = false;
        }

        /// <summary>
        /// Constructor for defining a line that can be finite or infinite
        /// </summary>
        /// <param name="start">A starting point on the line</param>
        /// <param name="dir">A directional vector or end point</param>
        /// <param name="finite">True marks the line as finite, false an infinite line</param>
        public Line(Vec3 start, Vec3 dir, bool finite)
        {
            Start = start;
            Direction = dir;
            Finite = finite;
        }

        /// <summary>
        /// The point on the line where the X value is the given value
        /// </summary>
        /// <param name="x">The X value of the point to find</param>
        /// <returns>The point on the line that has the given X value</returns>
        public readonly Vec3 PointForX(float x)
        {
            float t = (x - Start.X) / Direction.X; //x = x0 + ta
            float y = Start.Y + t * Direction.Y;
            float z = Start.Z + t * Direction.Z;

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// The point on the line where the Y value is the given value
        /// </summary>
        /// <param name="y">The Y value of the point to find</param>
        /// <returns>The point on the line that has the given Y value</returns>
        public readonly Vec3 PointForY(float y) 
        {
            float t = (y - Start.Y) / Direction.Y;
            float x = Start.X + t * Direction.X;
            float z = Start.Z + t * Direction.Z;

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// The point on the line where the Z value is the given value
        /// </summary>
        /// <param name="z">The Z value of the point to find</param>
        /// <returns>The point on the line that has the given Z value</returns>
        public readonly Vec3 PointForZ(float z) 
        {
            float t = (z - Start.Z) / Direction.Z;
            float x = Start.X + t * Direction.X;
            float y = Start.Y + t * Direction.Y;

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Checks to see if the given point is on the line
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is on the line or irrelevantly close, false otherwise</returns>
        public readonly bool IsOnLine(Vec3 point)
        {
            float tx = (point.X - Start.X) / Direction.X;
            float ty = (point.Y - Start.Y) / Direction.Y;
            float tz = (point.Z - Start.Z) / Direction.Z;

            return tx.CloseTo(ty) && tx.CloseTo(tz);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Line other)
        {
            if (Finite != other.Finite) { return false; } //if one line is finite and the other isn't then they are not equal

            if (Finite) //if both are finite then they are equal if both start and direction are the same
            {
                return Start == other.Start && Direction == other.Direction;
            }

            return IsOnLine(other.Start) && Direction.IsParallel(other.Direction);
        }

        /// <inheritdoc/>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Line l && Equals(l);
        }

        /// <summary>
        /// Defines the equivalence operator between lines
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the lines are equivalent, false otherwise</returns>
        public static bool operator ==(Line lhs, Line rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Defines the non-equivalence operator between lines
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the lines are not equivalent, false otherwise</returns>
        public static bool operator !=(Line lhs, Line rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Start, HashCode.Combine(Direction, Finite));
        }

        #region ToString
        /// <inheritdoc/>
        public readonly override string ToString()
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
            return string.Format("Line Point: {0} Vector: {1}", Start.ToString(format, formatProvider), Direction.ToString(format, formatProvider));
        }
        #endregion
    }
}