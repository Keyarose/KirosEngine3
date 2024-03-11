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
    /// Defines a circle in 3D space
    /// </summary>
    public struct Circle3D : IEquatable<Circle3D>, IFormattable
    {
        /// <summary>
        /// The Radius of the circle
        /// </summary>
        public float Radius;
        /// <summary>
        /// The Center of the circle
        /// </summary>
        public Vec3 Center;

        /// <summary>
        /// One of two points that define the circle's plane in conjunction with the center
        /// </summary>
        Vec3 Plane1, Plane2;

        /// <summary>
        /// The Diameter of the circle
        /// </summary>
        public readonly float Diameter { get { return Radius * 2; } }

        /// <summary>
        /// The Circumference of the circle
        /// </summary>
        public readonly float Circumference { get { return Diameter * MathF.PI; } }

        //todo: public Plane CirPlane { get { return new Plane(Center, Plane1, Plane2); } }

        /// <summary>
        /// Basic constructor for a circle
        /// </summary>
        /// <param name="rad">The radius of the circle</param>
        /// <param name="cen">The center of the circle</param>
        /// <param name="p1">The first point used to define the plane</param>
        /// <param name="p2">The second point used to define the plane</param>
        public Circle3D(float rad, Vec3 cen, Vec3 p1,  Vec3 p2) 
        {
            Radius = rad;
            Center = cen;
            Plane1 = p1;
            Plane2 = p2;
        }

        /// <summary>
        /// Construct a 3D circle from a 2D circle
        /// </summary>
        /// <param name="cir">The 2D circle to use, the Z co-ord of the center is set to 0</param>
        /// <param name="p1">The first point used to define the plane</param>
        /// <param name="p2">The second point used to define the plane</param>
        public Circle3D(Circle2D cir, Vec3 p1, Vec3 p2)
        {
            Radius = cir.Radius;
            Center = new Vec3(cir.Center, 0);
            Plane1 = p1;
            Plane2 = p2;
        }

        /// <summary>
        /// Check to see if the given point is inside the circle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the circle, false otherwise</returns>
        public readonly bool IsPointInside(Vec3 point)
        {
            if (point == Center) { return true; }

            var dist = Vec3.Distance(Center, point);
            //if (CirPlane.IsOnPlane(point) && MathF.Abs(dist) < Radius) { return true; }

            return false;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Circle3D cir && Equals(cir);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Circle3D other)
        {
            return Radius == other.Radius && Center == other.Center; //todo: && CirPlane == other.CirPlane;
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(HashCode.Combine(Radius, Center.GetHashCode()), HashCode.Combine(Plane1, Plane2));
        }

        /// <summary>
        /// Define the equivalence operator for a 3D circle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if they are equal false otherwise</returns>
        public static bool operator ==(Circle3D lhs, Circle3D rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator for a 3D circle
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if they are equal, true otherwise</returns>
        public static bool operator !=(Circle3D lhs, Circle3D rhs)
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
            return string.Format("Center: {0} Radius: {1} Plane Point 1: {2} Plane Point 2: {3}",
                Center.ToString(format, formatProvider),
                Radius.ToString(format, formatProvider),
                Plane1.ToString(format, formatProvider),
                Plane2.ToString(format, formatProvider));
        }
    }
}
