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
        /// Checks if a float is close enough to zero to be zero
        /// </summary>
        /// <param name="a">The float to check</param>
        /// <returns>True if the value is zero or smaller than <see cref="float.Epsilon"/></returns>
        public static bool IsZero(this float a)
        {
            return MathF.Abs(a) < float.Epsilon;
        }

        public static float Clamp(this float a, float min, float max)
        {
            return a < min ? min : (a > max ? max : a);
        }
    }
}
