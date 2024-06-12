using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// The mathematical representation of a line in 2D.
    /// </summary>
    public struct Line2D : IEquatable<Line2D>, IFormattable
    {
        /// <summary>
        /// Starting point of the line
        /// </summary>
        public Vec2 Start;

        /// <summary>
        /// Direction of the line
        /// </summary>
        public Vec2 Direction;

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
                if (Finite) { return Vec2.Distance(Start, Direction); }

                return float.PositiveInfinity;
            }
        }

        /// <summary>
        /// Basic constructor for a 2D line
        /// </summary>
        /// <param name="start">A starting point on the line</param>
        /// <param name="dir">The direction the line is in from the start point</param>
        public Line2D(Vec2 start, Vec2 dir)
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
        public Line2D(Vec2 start, Vec2 dir, bool finite)
        {
            Start = start;
            Direction = dir;
            Finite = finite;
        }

        /// <summary>
        /// The point on the line where the X value is the given value if it exists on the line
        /// </summary>
        /// <param name="x">The X value of the point to find</param>
        /// <returns>The point on the line that has the given X value or Start if it doesn't exist</returns>
        public readonly Vec2 PointForX(float x)
        {
            if (!Direction.X.IsZero())
            {
                float t = (x - Start.X) / Direction.X;
                float y = Start.Y + (t * Direction.Y);

                return new Vec2(x, y);
            }

            return Start;
        }

        /// <summary>
        /// The point on the line where the Y value is the given value if it exists on the line
        /// </summary>
        /// <param name="y">The Y value of the point to find</param>
        /// <returns>The point on the line that has the given Y value or Start if it doesn't exist</returns>
        public readonly Vec2 PointForY(float y) 
        {
            if (!Direction.Y.IsZero()) 
            {
                float t = (y - Start.Y) / Direction.Y;
                float x = Start.Y + (t * Direction.Y);

                return new Vec2(x, y);
            }

            return Start;
        }

        /// <summary>
        /// Checks to see if the given point is on the line
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is on the line or irrelevantly close, false otherwise</returns>
        public readonly bool IsOnLine(Vec2 point)
        {
            float d = ((point.X - Direction.X) * (Direction.Y - Start.Y)) -
                ((point.Y - Start.Y) * (Direction.X - Start.X));

            if (d.IsZero())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks which side of the line the given point is on
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="pos">If true returns true if the point is on the positive value side, if false returns true if the point is on the negative value side</param>
        /// <returns>True if the point is on the side with the value sign indicated by pos, false otherwise</returns>
        public readonly bool IsToSide(Vec2 point, bool pos)
        {
            float d = ((point.X - Direction.X) * (Direction.Y - Start.Y)) -
                ((point.Y - Start.Y) * (Direction.X - Start.X));

            if (pos)
            {
                if (d > 0)
                {
                    return true;
                }

                return false;
            }
            else
            {
                if (d < 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public readonly bool Equals(Line2D other)
        {
            if (Finite != other.Finite) { return false; } //if one line is finite and the other isn't they are not equal

            if (Finite) //if both are finite then they are equal if both start and direction are the same
            {
                return Start == other.Start && Direction == other.Direction;
            }

            return IsOnLine(other.Start) && Direction.IsParallel(other.Direction);
        }

        /// <inheritdoc/>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Line2D l && Equals(l);
        }

        /// <summary>
        /// Defines the equivalence operator between lines
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the lines are equivalent, false otherwise</returns>
        public static bool operator ==(Line2D lhs, Line2D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Defines the non-equivalence operator between lines
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the lines are not equivalent, false otherwise</returns>
        public static bool operator !=(Line2D lhs, Line2D rhs)
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
            return string.Format("Line Point: {0} Vector: {1}", Start.ToString(format, formatProvider), Direction.ToString(format, formatProvider));
        }
        #endregion
    }
}
