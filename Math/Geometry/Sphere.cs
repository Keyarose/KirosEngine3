using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    public struct Sphere : IEquatable<Sphere>, IFormattable
    {
        /// <summary>
        /// The center of the sphere
        /// </summary>
        public Vec3 Center;

        /// <summary>
        /// The radius of the sphere
        /// </summary>
        public float Radius;

        /// <summary>
        /// Basic constructor for a Sphere
        /// </summary>
        /// <param name="center">The center of the sphere</param>
        /// <param name="radius">The radius of the sphere</param>
        public Sphere(Vec3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Construct a Sphere from a circle
        /// </summary>
        /// <param name="circle">The circle to build the sphere from</param>
        public Sphere(Circle3D circle)
        {
            Center = circle.Center;
            Radius = circle.Radius;
        }

        /// <summary>
        /// Check if the given point is inside the sphere
        /// </summary>
        /// <param name="p">The given point</param>
        /// <returns>True if the point is inside the sphere, false otherwise</returns>
        public readonly bool IsPointInside(Vec3 p)
        {
            return Vec3.Distance(Center, p) < Radius;
        }

        /// <summary>
        /// Create a circle from the sphere using the given plane
        /// </summary>
        /// <param name="plane">The plane to use</param>
        /// <returns>The resulting circle</returns>
        /// <exception cref="ArgumentException">Thrown if the plane doesn't intersect the center</exception>
        public readonly Circle3D GetCircleOnPlane(Plane plane)
        {
            if (!plane.IsOnPlane(Center)) 
            { throw new ArgumentException(string.Format("Cannot generate a circle from the plane as it doesn't intersect the sphere's center. {0}", plane)); }

            return new Circle3D(Radius, Center, plane);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Sphere other)
        {
            return Center == other.Center && Radius == other.Radius;
        }

        /// <inheritdoc/>
        public readonly override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Sphere sp && Equals(sp);
        }

        /// <summary>
        /// Defines the equivalence operator for Spheres
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the spheres are equivalent, false otherwise</returns>
        public static bool operator ==(Sphere lhs, Sphere rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Defines the non-equivalence operator for Spheres
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the spheres are not equivalent, false otherwise</returns>
        public static bool operator !=(Sphere lhs, Sphere rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(Center, Radius);
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
            return ToString(null , formatProvider);
        }

        /// <inheritdoc/>
        public readonly string ToString(string? format, IFormatProvider? formatProvider) 
        {
            return string.Format("Sphere \n\t Center: {0}\n\t Radius: {1}", Center.ToString(format, formatProvider), Radius.ToString(format, formatProvider));
        }
        #endregion
    }
}
