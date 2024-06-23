using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Data
{
    /// <summary>
    /// A four channel color object made up of red, green, blue and alpha.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4 : IEquatable<Color4>
    {
        /// <summary>
        /// The color components
        /// </summary>
        public float R, G, B, A;

        /// <summary>
        /// Alias of the R component
        /// </summary>
        public float X
        {
            readonly get { return R; }
            set { R = value; }
        }

        /// <summary>
        /// Alias of the G component
        /// </summary>
        public float Y
        {
            readonly get { return G; }
            set { G = value; }
        }

        /// <summary>
        /// Alias of the B component
        /// </summary>
        public float Z
        {
            readonly get { return B; }
            set { B = value; }
        }

        /// <summary>
        /// Alias of the A component
        /// </summary>
        public float W
        {
            readonly get { return A; }
            set { A = value; }
        }

        /// <summary>
        /// Index accessor for the color
        /// </summary>
        /// <param name="index">Index that corresponds to the R, G, B, or A component</param>
        /// <returns>The value of the index specified component</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is outside of 0-3 inclusive.</exception>
        public float this[int index]
        {
            readonly get
            {
                switch (index) 
                {
                    case 0: 
                        return R;
                    case 1: 
                        return G;
                    case 2: 
                        return B;
                    case 3: 
                        return A;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Color4", index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        R = value;
                        break;
                    case 1:
                        G = value;
                        break;
                    case 2:
                        B = value;
                        break;
                    case 3:
                        A = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Color4", index));
                }
            }
        }

        /// <summary>
        /// The size of a Color4 in bytes.
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<Color4>();

        #region Constructors
        /// <summary>
        /// Construct a Color vector with identical components
        /// </summary>
        /// <param name="f">The value for each component</param>
        public Color4(float f)
        {
            R = f;  G = f;  B = f; A = f;
        }

        /// <summary>
        /// Construct a Color vector
        /// </summary>
        /// <param name="r">The red component</param>
        /// <param name="g">The green component</param>
        /// <param name="b">The blue component</param>
        /// <param name="a">The alpha component</param>
        public Color4(float r, float g, float b, float a)
        {
            R = r; G = g; B = b; A = a;
        }

        /// <summary>
        /// Construct a Color vector from byte values
        /// </summary>
        /// <param name="r">The red component</param>
        /// <param name="g">The green component</param>
        /// <param name="b">The blue component</param>
        /// <param name="a">The alpha component</param>
        public Color4(byte r, byte g, byte b, byte a)
        {
            R = (float)r / byte.MaxValue;
            G = (float)g / byte.MaxValue;
            B = (float)b / byte.MaxValue;
            A = (float)a / byte.MaxValue;
        }

        /// <summary>
        /// Construct a Color vector as a copy of another
        /// </summary>
        /// <param name="color">The color to copy</param>
        public Color4(Color4 color) 
        {
            R = color.R; G = color.G; B = color.B; A = color.A;
        }

        /// <summary>
        /// Construct a Color vector from a string containing all four components, comma delimitated.
        /// </summary>
        /// <param name="colorVals">The string containing the component values.</param>
        /// <exception cref="ArgumentException">Thrown if there are too few components in the string. Or if there are issues parsing the string into float values.</exception>
        public Color4(string colorVals)
        {
            string[] comp = colorVals.Replace(" ", string.Empty).Split(',');

            if (comp.Length < 4) 
            {
                throw new ArgumentException(string.Format("Too few components in the provided string: {0}, number of components: {1}", colorVals, comp.Length));
            }

            try 
            {
                R = float.Parse(comp[0]);
                G = float.Parse(comp[1]);
                B = float.Parse(comp[2]);
                A = float.Parse(comp[3]);
            }
            catch (ArgumentNullException exn)
            {
                throw new ArgumentException(string.Format("One or more substrings from {0} resulted in a null argument when attempting to parse to float.", colorVals), exn);
            }
            catch (OverflowException exo)
            {
                throw new ArgumentException(string.Format("One or more components from {0} resulted in an overflow when attempting to parse to float.", colorVals), exo);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException(string.Format("One or more components from {0} is not formatted correctly to parse to float.", colorVals), ex);
            }
        }
        #endregion

        #region Color Definitions
        /// <summary>
        /// A predefined color: Transparent.
        /// </summary>
        public static readonly Color4 Transparent = new Color4(255, 255, 255, 0);

        /// <summary>
        /// A predefined color: Aqua.
        /// </summary>
        public static readonly Color4 Aqua = new Color4(0, 255, 255, 255);

        /// <summary>
        /// A predefined color: Black.
        /// </summary>
        public static readonly Color4 Black = new Color4(0, 0, 0, 255);

        /// <summary>
        /// A predefined color: Blue.
        /// </summary>
        public static readonly Color4 Blue = new Color4(0, 0, 255, 255);

        /// <summary>
        /// A predefined color: Gray.
        /// </summary>
        public static readonly Color4 Gray = new Color4(192, 192, 192, 255);

        /// <summary>
        /// A predefined color: Green.
        /// </summary>
        public static readonly Color4 Green = new Color4(0, 255, 0, 255);

        /// <summary>
        /// A predefined color: Red.
        /// </summary>
        public static readonly Color4 Red = new Color4(255, 0, 0, 255);

        /// <summary>
        /// A predefined color: White.
        /// </summary>
        public static readonly Color4 White = new Color4(255, 255, 255, 255);

        /// <summary>
        /// A predefined color: Yellow.
        /// </summary>
        public static readonly Color4 Yellow = new Color4(255, 255, 0, 255);
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Color4 c && Equals(c);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Color4 other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left operand</param>
        /// <param name="rhs">The right operand</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(Color4 lhs, Color4 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left operand</param>
        /// <param name="rhs">The right operand</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(Color4 lhs, Color4 rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }
        #endregion

        #region Conversions
        //todo: color space conversions

        /// <summary>
        /// Handle conversion from Vec4 to Color4
        /// </summary>
        /// <param name="v">The vector to be converted</param>
        public static explicit operator Color4(Vec4 v)
        {
            return new Color4(v.X, v.Y, v.Z, v.W);
        }

        /// <summary>
        /// Handle conversion from System.Drawing.Color to Color4
        /// </summary>
        /// <param name="c">The system color to be converted.</param>
        public static implicit operator Color4(Color c)
        {
            return new Color4(c.R, c.G, c.B, c.A);
        }

        /// <summary>
        /// Handle conversion from Color4 to System.Drawing.Color
        /// </summary>
        /// <param name="c">The Color4 to be converted.</param>
        public static explicit operator Color(Color4 c)
        {
            return Color.FromArgb((int)(c.A * 255), (int)(c.R * 255), (int)(c.G * 255), (int)(c.B * 255));
        }
        #endregion

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
            return string.Format("Color4: ({0},{1},{2},{3})",
                R.ToString(format, formatProvider),
                G.ToString(format, formatProvider),
                B.ToString(format, formatProvider),
                A.ToString(format, formatProvider));
        }
        #endregion
    }
}
