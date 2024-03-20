using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Data
{
    public struct Measurement
    {
        public double Value;
        public Unit Units;
        public Prefix Prefix;

        public Measurement(double value, Unit unit, Prefix pre)
        {
            Value = value;
            Units = unit;
            Prefix = pre;
        }

        /// <summary>
        /// Define the addition operator between two measurements
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>A measurement in the units of the left Measurement who's value is the sum of the two inputs</returns>
        /// <exception cref="ArgumentException">Thrown if the unit types are different and thus cannot be added</exception>
        public static Measurement operator +(Measurement lhs, Measurement rhs) 
        {
            //units are the same
            if (lhs.Units == rhs.Units)
            {
                if (lhs.Prefix == rhs.Prefix) { return new Measurement(lhs.Value + rhs.Value, lhs.Units, lhs.Prefix); } //prefix is the same
                else //prefix is different
                {
                    float diff = PrefixDifference(lhs.Prefix, rhs.Prefix);
                    return new Measurement(lhs.Value + rhs.Value * diff, lhs.Units, lhs.Prefix); 
                }
            }

            throw new ArgumentException(string.Format("Addition of Measurements of Unit type {0}, and {1} is not defined.", lhs.Units, rhs.Units));
        }

        /// <summary>
        /// Define the subtraction operator between two measurements
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>A measurement in the units of the left measurement who's value is the minuend of the two inputs</returns>
        /// <exception cref="ArgumentException">Thrown if the unit types are different and thus cannot be subtracted</exception>
        public static Measurement operator -(Measurement lhs, Measurement rhs)
        {
            //units are the same
            if (lhs.Units == rhs.Units)
            {
                if (lhs.Prefix == rhs.Prefix) { return new Measurement(lhs.Value - rhs.Value, lhs.Units, lhs.Prefix); } //prefix is the same
                else //prefix is different
                {
                    float diff = PrefixDifference(lhs.Prefix, rhs.Prefix);
                    return new Measurement(lhs.Value - rhs.Value * diff, lhs.Units, lhs.Prefix);
                }
            }

            throw new ArgumentException(string.Format("Subtraction of Measurements of Unit type {0}, and {1} is not defined.", lhs.Units, rhs.Units));
        }

        /// <summary>
        /// Check to see if the provided units can be combined into a derived type
        /// </summary>
        /// <param name="a">The first unit type</param>
        /// <param name="b">The second unit type</param>
        /// <returns>True if the units can become a derived type, false otherwise</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static bool UnitsCanCompound(Unit a, Unit b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The multiplier to convert between the two prefixes
        /// </summary>
        /// <param name="a">The prefix to convert to</param>
        /// <param name="b">The prefix to convert from</param>
        /// <returns>The multiplier to convert from Prefix b to a</returns>
        private static float PrefixDifference(Prefix a, Prefix b)
        {
            float aMult = PrefixMultiplier(a);
            float bMult = PrefixMultiplier(b);
            
            return aMult * bMult;
        }

        /// <summary>
        /// Convert the prefix into its multiplier
        /// </summary>
        /// <param name="p">The prefix to convert</param>
        /// <returns>The multiplier represented by the prefix</returns>
        private static float PrefixMultiplier(Prefix p)
        {
            switch (p)
            {
                case Prefix.Deca:
                    return 10f;
                case Prefix.Hecto:
                    return 100f;
                case Prefix.Kilo:
                    return 1000f;
                case Prefix.Mega:
                    return 1000000f;
                case Prefix.Giga:
                    return 1000000000f;
                case Prefix.Tera:
                    return 1000000000000f;
                case Prefix.Peta:
                    return 1000000000000000f;
                case Prefix.Exa:
                    return 1000000000000000000f;
                case Prefix.Zetta:
                    return 1000000000000000000000f;
                case Prefix.Yotta:
                    return 1000000000000000000000000f;
                case Prefix.Ronna:
                    return 1000000000000000000000000000f;
                case Prefix.Quetta:
                    return 1000000000000000000000000000000f;
                case Prefix.None:
                    return 1;
                case Prefix.Deci:
                    return 0.1f;
                case Prefix.Centi:
                    return 0.01f;
                case Prefix.Milli:
                    return 0.001f;
                case Prefix.Micro:
                    return 0.000001f;
                case Prefix.Nano:
                    return 0.000000001f;
                case Prefix.Pico:
                    return 0.000000000001f;
                case Prefix.Femto:
                    return 0.000000000000001f;
                case Prefix.Atto:
                    return 0.000000000000000001f;
                case Prefix.Zepto:
                    return 0.000000000000000000001f;
                case Prefix.Yocto:
                    return 0.000000000000000000000001f;
                case Prefix.Ronto:
                    return 0.000000000000000000000000001f;
                case Prefix.Quecto:
                    return 0.000000000000000000000000000001f;
                default:
                    return 1;
            }
        }
    }

    /// <summary>
    /// Defines the unit the measurement is in
    /// </summary>
    public enum Unit
    {
        Meter,
        Second,
        Gram,
        Ampere,
        Kelvin,
        Mole,
        Candela,
        Minute,
        Hour,
        Day,
        Week,
        Month,
        Year
    }

    /// <summary>
    /// Magnitude prefix of the unit used by the measurement
    /// </summary>
    public enum Prefix
    {
        Deca, //10^1
        Hecto, //10^2
        Kilo, //10^3
        Mega, //10^6
        Giga, //10^9
        Tera, //10^12
        Peta, //10^15
        Exa, //10^18
        Zetta, //10^21
        Yotta, //10^24
        Ronna, //10^27
        Quetta, //10^30
        None, //10^0
        Deci, //10^-1
        Centi, //10^-2
        Milli, //10^-3
        Micro, //10^-6
        Nano, //10^-9
        Pico, //10^-12
        Femto, //10^-15
        Atto, //10^-18
        Zepto, //10^-21
        Yocto, //10^-24
        Ronto, //10^-27
        Quecto //10^-30
    }
}
