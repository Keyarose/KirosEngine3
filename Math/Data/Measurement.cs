using KirosEngine3.Exceptions;
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
        public int SigDigits;

        public Measurement(double value, Unit unit)
        {
            Value = value;
            Units = unit;
        }

        /// <summary>
        /// Change the prefix of the measurement's units
        /// </summary>
        /// <param name="target">The prefix to change to</param>
        public void ChangeUnitPrefix(Prefix target)
        {
            double conv = Units.PrefixChangeTo(target);

            Value *= conv; //adjust the value for the prefix change
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
            if (lhs.Units.Equals(rhs.Units))
            {
                //if the two measurements use different prefixes change the rhs to match the left
                if (lhs.Units.Prefix != rhs.Units.Prefix)
                {
                    rhs.ChangeUnitPrefix(lhs.Units.Prefix);
                }

                return new Measurement(lhs.Value + rhs.Value, lhs.Units);
            }
            
            //units are not the same so try to convert rhs to the same units as lhs
            try
            {
                rhs = rhs.ConvertUnits(lhs.Units);
            }
            catch (Exception)
            {
                throw;
            }

            //if the two measurements use different prefixes change the rhs to match the left
            if (lhs.Units.Prefix != rhs.Units.Prefix)
            {
                rhs.ChangeUnitPrefix(lhs.Units.Prefix);
            }

            return new Measurement(lhs.Value + rhs.Value, lhs.Units);
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
            if (lhs.Units.Equals(rhs.Units))
            {
                //if the two measurements use different prefixes change the rhs to match the left
                if (lhs.Units.Prefix != rhs.Units.Prefix)
                {
                    rhs.ChangeUnitPrefix(lhs.Units.Prefix);
                }

                return new Measurement(lhs.Value - rhs.Value, lhs.Units);
            }

            //units are not the same so try to convert rhs to the same units as lhs
            try
            {
                rhs = rhs.ConvertUnits(lhs.Units);
            }
            catch (Exception)
            {
                throw;
            }

            //if the two measurements use different prefixes change the rhs to match the left
            if (lhs.Units.Prefix != rhs.Units.Prefix)
            {
                rhs.ChangeUnitPrefix(lhs.Units.Prefix);
            }

            return new Measurement(lhs.Value - rhs.Value, lhs.Units);
        }

        /// <summary>
        /// Converts the measurement to use the given units
        /// </summary>
        /// <param name="targetUnits">The units to convert to</param>
        /// <returns>The measurement using the given units</returns>
        /// <exception cref="ConversionNotDefinedException">Thrown if conversion from the starting units to the target units is not defined</exception>
        public Measurement ConvertUnits(Unit targetUnits)
        {
            if (Units.Equals(targetUnits))
                return this;

            if (!Units.Conversions.ContainsKey(targetUnits.Symbol))
                throw new ConversionNotDefinedException(string.Format("Conversion of Measurements between Unit type {0}, and {1} is not defined.", Units, targetUnits));

            return Units.Conversions[targetUnits.Symbol].Invoke(this);
        }
    }
}
