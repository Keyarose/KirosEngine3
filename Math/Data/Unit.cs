using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Data
{
    /// <summary>
    /// Defines a Unit of measurement
    /// </summary>
    public struct Unit : IEquatable<Unit>
    {
        /// <summary>
        /// The name of the unit.
        /// </summary>
        public string Name;
        /// <summary>
        /// The symbol of the unit.
        /// </summary>
        public string Symbol;
        /// <summary>
        /// The unit multiplier.
        /// </summary>
        public Prefix Prefix;
        /// <summary>
        /// Conversion definitions between different units.
        /// </summary>
        public Dictionary<string, Func<Measurement, Measurement>> Conversions;

        //todo: singleton index for defined units

        /// <summary>
        /// Define the Meter unit of measurement
        /// </summary>
        public static readonly Unit Meter = new Unit
        {
            Name = "Meter",
            Symbol = "m",
            Prefix = Prefix.None,
            Conversions = new Dictionary<string, Func<Measurement, Measurement>>()
            {
                { "ft", m => { return new Measurement(m.Value * 3.28084, Foot); } }
            }
            
        };

        /// <summary>
        /// Define the Foot unit of measurement
        /// </summary>
        public static readonly Unit Foot = new Unit
        {
            Name = "Foot",
            Symbol = "ft",
            Prefix = Prefix.None,
            Conversions = new Dictionary<string, Func<Measurement, Measurement>>()
            {
                { "m", c => { return new Measurement(c.Value * 0.3048, Meter); } }
            }
        };

        /// <summary>
        /// Basic constructor for a unit of measurement
        /// </summary>
        /// <param name="name">The name of the unit</param>
        /// <param name="sym">The symbol of the unit</param>
        /// <param name="pre">The unit's multiplier prefix.</param>
        /// <param name="con">A list of conversion methods to other units</param>
        public Unit(string name, string sym, Prefix pre, Dictionary<string, Func<Measurement, Measurement>> con) 
        {
            Name = name;
            Symbol = sym;
            Prefix = pre;
            Conversions = con;
        }

        /// <summary>
        /// Add a conversion to another unit type
        /// </summary>
        /// <param name="targetSymbol">The symbol of the target unit type</param>
        /// <param name="converter">The conversion function from the start type to the target type</param>
        public readonly void AddConversion(string targetSymbol, Func<Measurement, Measurement> converter)
        {
            Conversions.Add(targetSymbol, converter);//todo: handle existing keys
        }

        /// <summary>
        /// Removed a conversion method
        /// </summary>
        /// <param name="targetSymbol">The symbol of the conversion to be removed</param>
        public readonly void RemoveConversion(string targetSymbol) 
        {
            Conversions.Remove(targetSymbol);
        }

        /// <summary>
        /// Check if there is a conversion to the given units
        /// </summary>
        /// <param name="unit">The units to be converted to</param>
        /// <returns>True if there is a conversion method, false otherwise</returns>
        public readonly bool HasConversion(Unit unit)
        {
            return Conversions.ContainsKey(unit.Symbol);
        }

        /// <summary>
        /// Check if there is a conversion to the given units
        /// </summary>
        /// <param name="symbol">The symbol of the units to be converted to</param>
        /// <returns>True if there is a conversion method, false otherwise</returns>
        public readonly bool HasConversion(string symbol)
        {
            return Conversions.ContainsKey(symbol);
        }

        /// <summary>
        /// Change the conversion method for the given unit type
        /// </summary>
        /// <param name="targetSymbol">The symbol of the target unit type</param>
        /// <param name="converter">The conversion function</param>
        public readonly void ChangeConversion(string targetSymbol, Func<Measurement, Measurement> converter)
        {
            if (Conversions.ContainsKey(targetSymbol)) 
            {
                Conversions[targetSymbol] = converter;
            }

            Conversions.Add(targetSymbol, converter);
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

        /// <summary>
        /// Change the unit's prefix and provide the conversion factor 
        /// </summary>
        /// <param name="target">The prefix to change to</param>
        /// <returns>The conversion factor to multiply the value by</returns>
        public double PrefixChangeTo(Prefix target)
        {
            if (Prefix == target) { return 1; }

            float pMul = PrefixMultiplier(Prefix);
            float tMul = PrefixMultiplier(target);

            Prefix = target;

            return pMul / tMul;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Unit other)
        {
            return Name.Equals(other.Name) && Symbol.Equals(other.Symbol);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Unit u && Equals(u);
        }

        /// <summary>
        /// Define the equivalence operator between two units
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>True if the name and symbol are the same</returns>
        public static bool operator ==(Unit lhs, Unit rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equivalence operator between two units
        /// </summary>
        /// <param name="lhs">The left value</param>
        /// <param name="rhs">The right value</param>
        /// <returns>False if the name and symbol are the same</returns>
        public static bool operator !=(Unit lhs, Unit rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Name, HashCode.Combine(Symbol, Prefix));
        }

        //todo: implement IFormattable
        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return string.Format("Unit Name: {1}{0}\n\t Symbol: {1}{2}", Name, Prefix, Symbol);
        }
    }

    /// <summary>
    /// Magnitude prefix of the unit used by the measurement
    /// </summary>
    public enum Prefix
    {
        /// <summary>
        /// 10 to the 1st power.
        /// </summary>
        Deca,
        /// <summary>
        /// 10 to the 2nd power.
        /// </summary>
        Hecto,
        /// <summary>
        /// 10 to the 3rd power.
        /// </summary>
        Kilo,
        /// <summary>
        /// 10 to the 6th power.
        /// </summary>
        Mega,
        /// <summary>
        /// 10 to the 9th power.
        /// </summary>
        Giga,
        /// <summary>
        /// 10 to the 12th power.
        /// </summary>
        Tera,
        /// <summary>
        /// 10 to the 15th power.
        /// </summary>
        Peta,
        /// <summary>
        /// 10 to the 18th power.
        /// </summary>
        Exa,
        /// <summary>
        /// 10 to the 21st power.
        /// </summary>
        Zetta,
        /// <summary>
        /// 10 to the 24th power.
        /// </summary>
        Yotta,
        /// <summary>
        /// 10 to the 27th power.
        /// </summary>
        Ronna,
        /// <summary>
        /// 10 to the 30th power.
        /// </summary>
        Quetta,
        /// <summary>
        /// 10 to the 0th power.
        /// </summary>
        None,
        /// <summary>
        /// 10 to the -1st power.
        /// </summary>
        Deci,
        /// <summary>
        /// 10 to the -2nd power.
        /// </summary>
        Centi,
        /// <summary>
        /// 10 to the -3rd power.
        /// </summary>
        Milli,
        /// <summary>
        /// 10 to the -6th power.
        /// </summary>
        Micro,
        /// <summary>
        /// 10 to the -9th power.
        /// </summary>
        Nano,
        /// <summary>
        /// 10 to the -12th power.
        /// </summary>
        Pico,
        /// <summary>
        /// 10 to the -15th power.
        /// </summary>
        Femto,
        /// <summary>
        /// 10 to the -18th power.
        /// </summary>
        Atto,
        /// <summary>
        /// 10 to the -21st power.
        /// </summary>
        Zepto,
        /// <summary>
        /// 10 to the -24th power.
        /// </summary>
        Yocto,
        /// <summary>
        /// 10 to the -27th power.
        /// </summary>
        Ronto,
        /// <summary>
        /// 10 to the -30th power.
        /// </summary>
        Quecto
    }
}
