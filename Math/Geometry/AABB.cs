using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// Defines an Axially Aligned Bounding Box(AABB)
    /// </summary>
    public struct AABB
    {
        public Vec3 Origin;
        public Vec3 Extent;

        /// <summary>
        /// The width or X dimension of the AABB
        /// </summary>
        public float Width
        {
            readonly get { return Extent.X - Origin.X; }
            set { Extent.X = Origin.X + value; }
        }
        /// <summary>
        /// The width or X dimension of the AABB
        /// </summary>
        public float X { readonly get { return Width; } set { Width = value; } }

        /// <summary>
        /// The height or Y dimension of the AABB
        /// </summary>
        public float Height
        {
            readonly get { return Extent.Y - Origin.Y; }
            set { Extent.Y = Origin.Y + value; }
        }
        /// <summary>
        /// The height or Y dimension of the AABB
        /// </summary>
        public float Y { readonly get { return Height; } set { Height = value; } }

        /// <summary>
        /// The Depth or Z dimension of the AABB
        /// </summary>
        public float Depth
        {
            readonly get { return Extent.Z - Origin.Z; }
            set { Extent.Z = Origin.Z + value; }
        }
        /// <summary>
        /// The Depth or Z dimension of the AABB
        /// </summary>
        public float Z { readonly get { return Depth; } set { Depth = value; } }

        /// <summary>
        /// The center of the AABB
        /// </summary>
        public readonly Vec3 Center { get { return (Dimensions / 2.0f) + Origin; } }

        /// <summary>
        /// The dimensions of the AABB
        /// </summary>
        public readonly Vec3 Dimensions { get { return Extent - Origin; } }

        /// <summary>
        /// The radius of the largest sphere that can fit within the AABB
        /// </summary>
        public readonly float LargestFitRadius
        {
            get { return MathF.MinMagnitude(MathF.MinMagnitude(X, Y), Z); }
        }

        /// <summary>
        /// Predefined empty AABB
        /// </summary>
        public static readonly AABB Empty = new AABB(Vec3.MaxVal, -Vec3.MaxVal);

        /// <summary>
        /// Basic constructor for an AABB
        /// </summary>
        /// <param name="ori">The origin of the AABB</param>
        /// <param name="ext">The far point of the AABB, opposite of the origin</param>
        public AABB(Vec3 ori, Vec3 ext)
        {
            Origin = ori;
            Extent = ext;
        }

        /// <summary>
        /// Constructor for an AABB with dimensions given
        /// </summary>
        /// <param name="ori">The origin of the AABB</param>
        /// <param name="width">The width or X dimension</param>
        /// <param name="height">The height or Y dimension</param>
        /// <param name="depth">The depth or Z dimension</param>
        public AABB(Vec3 ori, float width, float height, float depth)
        {
            Origin = ori;
            Extent = ori;
            Width = width;
            Height = height;
            Depth = depth;
        }

        /// <summary>
        /// Construct an AABB that contains the given list of points
        /// </summary>
        /// <param name="points">The points that should be contained within the AABB</param>
        /// <returns>The AABB that fits the given points</returns>
        public static AABB AABBForPoints(Vec3[] points)
        {
            AABB result = Empty;

            foreach (var point in points) 
            {
                result.AddPoint(point);
            }

            return result;
        }

        /// <summary>
        /// Expand the AABB to include the given point
        /// </summary>
        /// <param name="p">The point to be added to the AABB</param>
        public void AddPoint(Vec3 p)
        {
            if (p.X < Origin.X) { Origin.X = p.X; }
            if (p.Y < Origin.Y) { Origin.Y = p.Y; }
            if (p.Z < Origin.Z) { Origin.Z = p.Z; }

            if (p.X > Extent.X) { Extent.X = p.X; }
            if (p.Y > Extent.Y) { Extent.Y = p.Y; }
            if (p.Z > Extent.Z) { Extent.Z = p.Z; }
        }

        /// <summary>
        /// Check to see if the given point is contained within the AABB
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>True if the point is inside the rectangle, false otherwise</returns>
        public readonly bool ContainsPoint(Vec3 point)
        {
            if (point.X >= Origin.X && point.Y >= Origin.Y && point.Z >= Origin.Z) 
            {
                if (point.X <= Extent.X && point.Y <= Extent.Y && point.Z <= Extent.Z)
                { return true; }
            }

            return false;
        }
    }
}
