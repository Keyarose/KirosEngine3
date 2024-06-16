using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math
{
    /// <summary>
    /// Helper functions for various bits of math.
    /// </summary>
    public static class MathHelpers
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

        #region Float Extentions
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

        /// <summary>
        /// Compare one float to another to see if they are within the given margin of difference
        /// </summary>
        /// <param name="a">The first float to compare</param>
        /// <param name="b">The second float</param>
        /// <param name="margin">The margin of difference to look for, defaults to Epsilon</param>
        /// <returns>True if the difference is less than the margin, false otherwise</returns>
        public static bool CloseTo(this float a, float b, float margin = float.Epsilon)
        {
            return (a - b).Abs() <= margin;
        }

        /// <summary>
        /// Shortcut method for getting the absolute value of a float
        /// </summary>
        /// <param name="a">The float to get the absolute value</param>
        /// <returns>The absolute value</returns>
        public static float Abs(this float a)
        {
            return MathF.Abs(a);
        }
        #endregion

        /// <summary>
        /// Shortcut method for getting the absolute value of a double.
        /// </summary>
        /// <param name="a">The float to get the absolute value.</param>
        /// <returns>The absolute value.</returns>
        public static double Abs(this double a)
        {
            return System.Math.Abs(a);
        }

        /// <summary>
        /// Absolute value of a generic number type.
        /// </summary>
        /// <typeparam name="T">A generic that conforms to INumber.</typeparam>
        /// <param name="a">The value to get the absolute of.</param>
        /// <returns>The absolute value.</returns>
        public static T Abs<T>(this T a) where T : INumber<T>
        {
            if (a < T.Zero)
                a = -a;

            return a;
        }

        /// <summary>
        /// Compare one generic value to another to see if they are within the tolerance of each other.
        /// </summary>
        /// <typeparam name="T">A generic type that conforms to INumber.</typeparam>
        /// <param name="a">The first value to compare.</param>
        /// <param name="b">The second value to compare.</param>
        /// <param name="tolerance">The allowed tolerance in the difference between the two values.</param>
        /// <returns>True if the difference is less than the tolerance, false otherwise.</returns>
        public static bool Equals<T>(this T a, T b, T tolerance) where T : INumber<T> 
        {
            T diff = a - b;
            if (diff.Abs() < tolerance)
                return true;

            return false;
        }

        /// <summary>
        /// Helper method to populate an array of generic type with the given value in all entries
        /// </summary>
        /// <typeparam name="T">The type of value in the array, must implement INumber</typeparam>
        /// <param name="array">The array to populate</param>
        /// <param name="value">The value to fill with</param>
        /// <returns>The resulting array</returns>
        /// <remarks>Based on https://stackoverflow.com/a/1014015</remarks>
        public static T[] Populate<T>(this T[] array, T value) where T : INumber<T>
        {
            for (int i = 0; i < array.Length; i++) 
            {
                array[i] = value;
            }

            return array;
        }
    }
}
