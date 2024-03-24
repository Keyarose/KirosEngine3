using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// Defines a circle in 3D space
    /// </summary>
    public struct Circle3D : IEquatable<Circle3D>, IFormattable, IGeometrical
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
        public Plane CirPlane;

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
        /// <param name="p1">The first point used to define the plane</param>
        /// <param name="p2">The second point used to define the plane</param>
        public Circle3D(float rad, Vec3 cen, Vec3 p1,  Vec3 p2) 
        {
            Radius = rad;
            Center = cen;
            CirPlane = new Plane(cen, p1, p2);
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
            CirPlane = new Plane(Center, p1, p2);
        }
        
        /// <summary>
        /// Construct a 3D circle using a plane
        /// </summary>
        /// <param name="rad">The radius of the circle</param>
        /// <param name="cen">The center of the circle, must be a point on the provided plane.</param>
        /// <param name="p">The plane the circle is on</param>
        public Circle3D(float rad, Vec3 cen, Plane p)
        {
            Radius = rad;
            Center = cen;
            CirPlane = p;
        }

        /// <summary>
        /// Check to see if the given point is inside the circle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the circle, false otherwise</returns>
        public readonly bool IsPointInside(Vec3 point)
        {
            if (point == Center) { return true; } //if the point is the center then it's true

            //if the point is on the same plane as the circle and closer than the radius to the center then it's true
            var dist = Vec3.Distance(Center, point);
            return CirPlane.IsOnPlane(point) && MathF.Abs(dist) < Radius;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Circle3D cir && Equals(cir);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Circle3D other)
        {
            return Radius == other.Radius && Center == other.Center && CirPlane == other.CirPlane;
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(HashCode.Combine(Radius, Center), CirPlane);
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
        public bool IsGeometricallyCorrect(out string? message)
        {
            if (CirPlane.IsOnPlane(Center)) 
            {
                message = null;
                return true; 
            }

            message = string.Format("Circle is not geometrically correct as it's center point is not on it's defining plane.");
            return false;
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
            return string.Format("Circle3D \n\t Center: {0}\n\t Radius: {1}\n\t Plane: {2}",
                Center.ToString(format, formatProvider),
                Radius.ToString(format, formatProvider),
                CirPlane.ToString(format, formatProvider));
        }
        #endregion
    }
}
