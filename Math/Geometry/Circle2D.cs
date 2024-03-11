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
    /// Defines a 2D circle
    /// </summary>
    public struct Circle2D : IEquatable<Circle2D>, IFormattable
    {
        /// <summary>
        /// The Radius of the circle
        /// </summary>
        public float Radius;
        /// <summary>
        /// The Center of the circle
        /// </summary>
        public Vec2 Center;

        /// <summary>
        /// The Diameter of the circle
        /// </summary>
        public readonly float Diameter { get { return Radius * 2; } }

        /// <summary>
        /// The Circumference of the circle
        /// </summary>
        public readonly float Circumference { get { return Diameter * MathF.PI; } }

        /// <summary>
        /// Basic constructor for a circle
        /// </summary>
        /// <param name="rad">The radius of the circle</param>
        /// <param name="cen">The center of the circle</param>
        public Circle2D(float rad, Vec2 cen)
        {
            Radius = rad;
            Center = cen;
        }

        /// <summary>
        /// Check to see if the given point is inside the circle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the circle, false otherwise</returns>
        public readonly bool IsPointInside(Vec2 point)
        {
            if (point == Center) { return true; } //if the point is at the center

            var dist = Vec2.Distance(Center, point);
            if (MathF.Abs(dist) < Radius) { return true; } //if the point is less than the radius away from center

            return false;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Circle2D cir && Equals(cir);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Circle2D other) 
        {
            return Radius == other.Radius && Center == other.Center;
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Radius, Center.GetHashCode());
        }

        /// <summary>
        /// Define the equivalence operator for a 2D circle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if they are equal false otherwise</returns>
        public static bool operator ==(Circle2D lhs, Circle2D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator for a 2D circle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if they are equal true otherwise</returns>
        public static bool operator !=(Circle2D lhs, Circle2D rhs)
        {
            return !lhs.Equals(rhs);
        }

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
            return string.Format("Center: {0} Radius: {1}", Center.ToString(format, formatProvider), Radius.ToString(format, formatProvider));
        }
    }
}
