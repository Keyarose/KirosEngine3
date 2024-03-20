using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    public struct Plane : IEquatable<Plane>, IFormattable
    {
        /// <summary>
        /// The points that define the plane listed in clockwise order
        /// </summary>
        public Vec3[] Points = new Vec3[3];

        /// <summary>
        /// The normal vector of the plane, also the a, b, and c values of the Plane's equation
        /// </summary>
        public Vec3 Normal;

        /// <summary>
        /// The D Value of the Plane's equation
        /// </summary>
        /// <remarks>Using the definition equation in the form of ax + by + cz = d</remarks>
        public float DVal;

        /// <summary>
        /// The plane's angle from the X axis in radians
        /// </summary>
        public readonly float AngleFromXAxis 
        { 
            get 
            {
                Vec3 yParallel = new Vec3(1, 0, 0) - PointForXZ(1, 0);
                Vec3 xParallel = PointForYZ(0, 0) - new Vec3(1, 0, 0);

                return MathF.Atan2(yParallel.Length, xParallel.Length);//todo: signs?, ask bryn
            } 
        }

        /// <summary>
        /// The plane's angle from the Y axis in radians
        /// </summary>
        public readonly float AngleFromYAxis
        {
            get
            {
                Vec3 yParallel = new Vec3(0, 1, 0) - PointForXZ(0, 0);
                Vec3 xParallel = PointForYZ(1, 0) - new Vec3(0, 1, 0);

                return MathF.Atan2(xParallel.Length, yParallel.Length);
            }
        }

        /// <summary>
        /// The plane's angle from the Z axis in radians
        /// </summary>
        public readonly float AngleFromZAxis
        {
            get
            {
                Vec3 yParallel = new Vec3(0, 0, 1) - PointForXZ(0, 0);
                Vec3 zParallel = PointForXY(0, 1) - new Vec3(0, 0, 1);

                return MathF.Atan2(yParallel.Length, zParallel.Length);
            }
        }

        /// <summary>
        /// Construct a plane using the provided points, normal, and D value
        /// </summary>
        /// <param name="ps">The points that define the plane in clockwise order</param>
        /// <param name="norm">The plane's normal</param>
        /// <param name="dv">The plane's D value</param>
        public Plane(Vec3[] ps, Vec3 norm, float dv)
        {
            Points = ps;
            Normal = norm;
            DVal = dv;
        }

        /// <summary>
        /// Construct a plane using the provided points and normal, and calculate the D value
        /// </summary>
        /// <param name="ps">The points that define the plane in clockwise order</param>
        /// <param name="norm">The plane's normal</param>
        public Plane(Vec3[] ps, Vec3 norm)
        {
            Points = ps;
            Normal = norm;
            DVal = Vec3.Dot(ps[0], norm);
        }

        /// <summary>
        /// Construct a plane using the provided points, and calculate the normal and D value
        /// </summary>
        /// <param name="ps">The points that define the plane in clockwise order</param>
        public Plane(Vec3[] ps)
        {
            Points = ps;
            Normal = Vec3.Cross(ps[1] - ps[0], ps[2] - ps[0]).NormalizedCopy();

#if DEBUG
            if (!Normal.IsFinite()) //unsure if it's needed so its debug only for now
            {
                Logger.WriteToLog("Plane normal calculation resulted in a nonfinite value.");
                Logger.WriteToLog(string.Format("Plane data: {0}, {1}, {2}", Points[0], Points[1], Points[2]));
            }
#endif
            DVal = Vec3.Dot(ps[0], Normal);
        }

        /// <summary>
        /// Construct a plane using the provided points, and calculate the normal and D value
        /// </summary>
        /// <param name="p1">Point one that defines the plane</param>
        /// <param name="p2">Point two that defines the plane</param>
        /// <param name="p3">Point three that defines the plane</param>
        public Plane(Vec3 p1, Vec3 p2, Vec3 p3) : this([p1, p2, p3]) { }

        #region PointOnPlane
        /// <summary>
        /// Find the point on the plane that fits the given x and y coords
        /// </summary>
        /// <param name="x">The X coordinate of the point</param>
        /// <param name="y">The Y coordinate of the point</param>
        /// <returns>The point on the plane that fits the X and Y coordinates</returns>
        public readonly Vec3 PointForXY(float x, float y)
        {
            //calculate the z value of the point using the plane's equation
            float z = (DVal - Normal.X * x - Normal.Y * y) / Normal.Z;

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Find the point on the plane that fits the given X and Z coords
        /// </summary>
        /// <param name="x">The X coordinate of the point</param>
        /// <param name="z">The Y coordinate of the point</param>
        /// <returns>The point on the plane that fits the X and Z coordinates</returns>
        public readonly Vec3 PointForXZ(float x, float z)
        {
            float y = (DVal - Normal.X * x - Normal.Z * z) / Normal.Y;

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Find the point on the plane that fits the given Y and Z coords
        /// </summary>
        /// <param name="y">The Y coordinate of the point</param>
        /// <param name="z">The Z coordinate of the point</param>
        /// <returns>The point on the plane that fits the Y and Z coordinates</returns>
        public readonly Vec3 PointForYZ(float y, float z)
        {
            float x = (DVal - Normal.Y * y - Normal.Z * z) / Normal.X;

            return new Vec3(x, y, z);
        }
        #endregion


        /// <summary>
        /// Find the distance from the plane for the given point
        /// </summary>
        /// <remarks>pg. 317, equation 9.14</remarks>
        /// <param name="point">The point to find the distance of</param>
        /// <returns>The distance from the plane</returns>
        public readonly float DistanceFromPlane(Vec3 point)
        {
            return Vec3.Dot(point, Normal) - DVal;
        }

        /// <summary>
        /// Check to see if the given point is on the plane
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if it is on the plane, false otherwise</returns>
        public readonly bool IsOnPlane(Vec3 point)
        {
            if (Vec3.Dot(point, Normal) == DVal) { return true; }
            
            return false;
        }

        /// <summary>
        /// Calculate the best fit plane normal vector for a given set of points
        /// </summary>
        /// <param name="points">The collection of points to find the best fit for. Must contain 3 or more points.</param>
        /// <returns>The best fit normal vector</returns>
        /// <exception cref="ArgumentException">Thrown if the list of points contains less than 3 points</exception>
        public static Vec3 BestFitNormal(Vec3[] points)
        {
            int pCount = points.Length;

            if (pCount < 3)
            {
                throw new ArgumentException("Two or fewer points were provided for a best fit Plane Normal, Three or more is required.");
            }

            Vec3 norm = Vec3.Zero;

            Vec3 prev = points[^1];

            for (int i = 0; i < pCount; i++) 
            {
                norm.X += (prev.Z + points[i].Z) * (prev.Y - points[i].Y);
                norm.Y += (prev.X + points[i].X) * (prev.Z - points[i].Z);
                norm.Z += (prev.Y + points[i].Y) * (prev.X - points[i].X);

                prev = points[i];
            }

            return norm.NormalizedCopy();
        }

        /// <summary>
        /// Calculate the best fit D value for a set of points
        /// </summary>
        /// <param name="points">The set of points to find the D value for. Must contain 3 or more points.</param>
        /// <returns>The best fit D value</returns>
        /// <exception cref="ArgumentException">Thrown if the list of points contains less than 3 points</exception>
        public static float BestFitDVal(Vec3[] points) 
        {
            int pCount = points.Length;

            if (pCount < 3)
            {
                throw new ArgumentException("Two or fewer points were provided for a best fit Plane D Value, Three or more is required.");
            }

            Vec3 pSum = Vec3.Zero;

            //sum the points
            foreach (Vec3 point in points) 
            {
                pSum += point;
            }

            //average the D value by the number of points
            return Vec3.Dot(pSum, BestFitNormal(points)) / pCount;
        }

        /// <summary>
        /// Calculate the best fit D value for a set of points, provide the best fit normal to prevent recalculating it.
        /// </summary>
        /// <param name="points">The set of points to find the D value for. Must contain 3 or more points.</param>
        /// <param name="bfNorm">The best fit normal for the set of points</param>
        /// <returns>The best fit D value</returns>
        /// <exception cref="ArgumentException">Thrown if the list of points contains less than 3 points</exception>
        public static float BestFitDVal(Vec3[] points, Vec3 bfNorm)
        {
            int pCount = points.Length;

            if (pCount < 3) 
            {
                throw new ArgumentException("Two or fewer points were provided for a best fit Plane D Value, Three or more is required.");
            }

            Vec3 pSum = Vec3.Zero;

            foreach (Vec3 point in points)
            {
                pSum += point;
            }

            return Vec3.Dot(pSum, bfNorm) / pCount;
        }

        /// <summary>
        /// Find the line defining the intersection between two planes
        /// </summary>
        /// <param name="other">The second plane in the intersection</param>
        /// <param name="intersect">The line defining the intersection</param>
        /// <returns>True if there is an intersection, false otherwise</returns>
        /// <remarks>based on Graphics gems 1 pg 305</remarks>
        public readonly bool Intersection(Plane other, out Line intersect)
        {
            Vec3 p3n = Vec3.Cross(Normal, other.Normal);
            float det = p3n.LengthSqr;

            if (det.IsZero())//the planes are parallel and don't intersect
            { 
                intersect = new Line(new Vec3(0, 0, 0), new Vec3(0, 0, 0));
                return false;
            }

            Vec3 point = ((Vec3.Cross(p3n, other.Normal) * DVal) + (Vec3.Cross(p3n, Normal) * other.DVal)) / det; //a point on the line of intersection

            intersect = new Line(point, p3n);
            return true;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Plane other)
        {
            //if the normals are parallel the planes are identical or parallel, if any of the defining points of a plane is on the other plane then they are identical
            return Normal.IsParallel(other.Normal) && IsOnPlane(other.Points[0]);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Plane pl && Equals(pl);
        }

        /// <summary>
        /// Defines the equivalence operator between two Planes
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the two planes are equivalent, false otherwise</returns>
        public static bool operator ==(Plane lhs, Plane rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Defines the non-equivalence operator between two Planes
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the two planes are not equivalent, false otherwise</returns>
        public static bool operator !=(Plane lhs, Plane rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Points, HashCode.Combine(Normal, DVal));
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
            return string.Format("Plane Points: ({0}, {1}, {2}) Normal: {3} D Value: {4}",
                Points[0].ToString(format, formatProvider),
                Points[1].ToString(format, formatProvider),
                Points[2].ToString(format, formatProvider),
                Normal.ToString(format, formatProvider),
                DVal.ToString(format, formatProvider));
        }
        #endregion
    }
}
