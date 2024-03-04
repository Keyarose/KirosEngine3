using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math
{
    internal static class MathHelpers
    {
        internal static readonly float PiOver2 = MathF.PI / 2;

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="deg">The value in degrees</param>
        /// <returns>The value in radians</returns>
        public static float DegToRad(float deg)
        {
            return deg * MathF.PI / 180.0f;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="rad">The value in radians</param>
        /// <returns>The value in degrees</returns>
        public static float RadToDeg(float rad)
        {
            return rad * 180.0f / MathF.PI;
        }

        /// <summary>
        /// Checks if a float is close enough to zero to be zero
        /// </summary>
        /// <param name="a">The float to check</param>
        /// <returns>True if the value is zero or smaller than <see cref="float.Epsilon"/></returns>
        public static bool IsZero(this float a)
        {
            return MathF.Abs(a) < float.Epsilon;
        }

        /// <summary>
        /// Clamp a floating point value between the given minimum and maximum
        /// </summary>
        /// <param name="a">The float to clamp</param>
        /// <param name="min">The minimum allowed value for the float</param>
        /// <param name="max">The maximum allowed value for the float</param>
        /// <returns>The clamped value</returns>
        public static float Clamp(this float a, float min, float max)
        {
            return a < min ? min : (a > max ? max : a);
        }
    }
}
