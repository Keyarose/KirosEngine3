using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Geometry
{
    /// <summary>
    /// Define a 2D rectangle
    /// </summary>
    public struct Rect2D
    {
        /// <summary>
        /// The rectangle's vertices defined in clockwise order starting from the upper left
        /// </summary>
        public Vec2[] Vertices = new Vec2[4];

        public Rect2D(Vec2[] verts)
        {
            Vertices = verts;
        }

        /// <summary>
        /// Top edge of the rectangle
        /// </summary>
        public readonly Vec2 Edge1 { get { return Vertices[1] - Vertices[0]; } }

        /// <summary>
        /// Right edge of the rectangle
        /// </summary>
        public readonly Vec2 Edge2 { get { return Vertices[2] - Vertices[1]; } }

        /// <summary>
        /// Bottom edge of the rectangle
        /// </summary>
        public readonly Vec2 Edge3 { get { return Vertices[2] - Vertices[3]; } }

        /// <summary>
        /// Left edge of the rectangle
        /// </summary>
        public readonly Vec2 Edge4 { get { return Vertices[3] - Vertices[0]; } }

        /// <summary>
        /// The perimeter of the rectangle
        /// </summary>
        public readonly float Perimeter { get { return Edge1.Length + Edge2.Length + Edge3.Length + Edge4.Length; } }

        /// <summary>
        /// The area of the rectangle
        /// </summary>
        public readonly float Area { get { return Vec3.Cross(Edge1.AsVec3(), Edge2.AsVec3()).Length; } }

        /// <summary>
        /// Is the rectangle also a square
        /// </summary>
        public readonly bool IsSquare
        {
            get
            {
                if (Edge1.Length == Edge2.Length) { return true; }

                return false;
            }
        }

        /// <summary>
        /// Is the rectangle also a parallelogram
        /// </summary>
        public readonly bool IsParallelogram
        {
            get
            {
                //skew is left right
                if (!Vertices[0].X.CloseTo(Vertices[3].X)) { return true; }
                //skew is up down
                if (!Vertices[0].Y.CloseTo(Vertices[1].Y)) { return true; }

                return false;
            }
        }

        /// <summary>
        /// Check to see if the given point is contained within the rectangle
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <remarks>Based on https://math.stackexchange.com/questions/1805724</remarks>
        /// <returns>True if the point is inside the rectangle, false otherwise</returns>
        public readonly bool ContainsPoint(Vec2 point)
        {
            float limit = Area;

            float test1;
            float test2;

            if (limit < 0)
            {
                test1 = (point.Y - Vertices[0].Y) * Edge4.X + (point.X - Vertices[0].X) * Edge4.Y;
                test2 = (point.X - Vertices[0].X) * Edge1.Y + (point.Y - Vertices[0].Y) * Edge1.X;

                if ((0 <= test1 && test1 <= -limit) && (0 <= test2 && test2 <= -limit))
                    return true;
            }
            else
            {
                test1 = (point.X - Vertices[0].X) * Edge4.Y + (point.Y - Vertices[0].Y) * Edge4.X;
                test2 = (point.Y - Vertices[0].Y) * Edge1.X + (point.X - Vertices[0].X) * Edge1.Y;

                if ((0 <= test1 && test1 <= limit) && (0 <= test2 && test2 <= limit))
                    return true;
            }

            return false;
        }
    }
}
