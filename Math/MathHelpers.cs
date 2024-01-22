using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math
{
    internal static class MathHelpers
    {
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
