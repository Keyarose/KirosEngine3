using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Two by two matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2 : IEquatable<Matrix2>, IFormattable
    {
        public Vec2 Row0;
        public Vec2 Row1;

        /// <summary>
        /// The Identity matrix
        /// </summary>
        public static readonly Matrix2 Identity = new Matrix2(Vec2.UnitX, Vec2.UnitY);

        /// <summary>
        /// the zero matrix
        /// </summary>
        public static readonly Matrix2 Zero = new Matrix2(Vec2.Zero, Vec2.Zero);

        /// <summary>
        /// The first column of the matrix
        /// </summary>
        public readonly Vec2 Column0 => new Vec2(Row0.X, Row1.X);

        /// <summary>
        /// The second column of the matrix
        /// </summary>
        public readonly Vec2 Column1 => new Vec2(Row0.Y, Row1.Y);

        /// <summary>
        /// Calculate the matrix's determinant
        /// </summary>
        public readonly float Determinant
        {
            get
            {
                return (Row0.X * Row1.Y) - (Row0.Y * Row1.X);
            }
        }

        #region Cell Accessors
        /// <summary>
        /// Accessor for row 0, column 0
        /// </summary>
        public float M00
        {
            readonly get { return Row0.X; }
            set { Row0.X = value; }
        }

        /// <summary>
        /// Accessor for row 0, column 1
        /// </summary>
        public float M01
        {
            readonly get { return Row0.Y; }
            set { Row0.Y = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 0
        /// </summary>
        public float M10
        {
            readonly get { return Row1.X; }
            set { Row1.X = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 1
        /// </summary>
        public float M11
        {
            readonly get { return Row1.Y; }
            set { Row1.Y = value; }
        }
        #endregion

        /// <summary>
        /// Accessor for the matrix's diagonal
        /// </summary>
        public Vec2 Diagonal
        {
            readonly get
            {
                return new Vec2(Row0.X, Row1.Y);
            }
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
            }
        }

        /// <summary>
        /// The matrix's trace, the sum of it's diagonal values
        /// </summary>
        public readonly float Trace
        {
            get
            {
                return Row0.X + Row1.Y;
            }
        }

        /// <summary>
        /// Array type accessor for the matrix
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>The value at the given indexes</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index values are out of the allowed range</exception>
        public float this[int row, int column]
        {
            readonly get
            {
                if (column < 0 && column > 2)
                {
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix2.", column));
                }

                switch (row)
                {
                    case 0:
                        return Row0[column];
                    case 1:
                        return Row1[column];
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2.", row));
                }
            }
            set
            {
                if (column < 0 && column > 2)
                {
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix2.", column));
                }

                switch (row) 
                {
                    case 0:
                        Row0[column] = value;
                        break;
                    case 1:
                        Row1[column] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2.", column));
                }
            }
        }

        /// <summary>
        /// Basic constructor using Vec2s
        /// </summary>
        /// <param name="r0">First row of the matrix</param>
        /// <param name="r1">Second row of the matrix</param>
        public Matrix2(Vec2 r0, Vec2 r1)
        {
            Row0 = r0;
            Row1 = r1;
        }

        /// <summary>
        /// Basic constructor using indvidual floats
        /// </summary>
        /// <param name="m00">Row 0, Column 0</param>
        /// <param name="m01">Row 0, Column 1</param>
        /// <param name="m10">Row 1, Column 0</param>
        /// <param name="m11">Row 1, Column 1</param>
        public Matrix2(float m00, float m01, float m10, float m11)
        {
            Row0 = new Vec2(m00, m01);
            Row1 = new Vec2(m10, m11);
        }

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Add(Matrix2 m1, Matrix2 m2) 
        {
            var r = new Matrix2
            {
                Row0 = m1.Row0 + m2.Row0,
                Row1 = m1.Row1 + m2.Row1,
            };
            return r;
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <param name="result">The resulting matrix</param>
        public static void Add(Matrix2 m1, Matrix2 m2, out Matrix2 result)
        {
            result = Add(m1, m2);
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix</param>
        /// <param name="rhs">Right matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 operator +(Matrix2 lhs, Matrix2 rhs)
        {
            return Add(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">First matrix</param>
        /// <param name="m2">Second matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Multiply(Matrix2 m1, Matrix2 m2)
        {
            var r = new Matrix2
            {
                Row0 = new Vec2(Vec2.Dot(m1.Row0, m2.Column0), Vec2.Dot(m1.Row0, m2.Column1)),
                Row1 = new Vec2(Vec2.Dot(m1.Row1, m2.Column0), Vec2.Dot(m1.Row1, m2.Column1))
            };
            return r;
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">First matrix</param>
        /// <param name="m2">Second matrix</param>
        /// <param name="result">The resulting matrix</param>
        public static void Multiply(Matrix2 m1, Matrix2 m2, out Matrix2 result)
        {
            result = Multiply(m1, m2);
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix</param>
        /// <param name="rhs">Right matrix</param>
        /// <returns></returns>
        public static Matrix2 operator *(Matrix2 lhs, Matrix2 rhs) 
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        //todo: invert

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <returns>The transpose in a new instance</returns>
        public static Matrix2 Transpose(Matrix2 m)
        {
            var r = new Matrix2
            {
                Row0 = m.Column0,
                Row1 = m.Column1,
            };

            return r;
        }

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <param name="result">The transpose in a new instance</param>
        public static void Transpose(Matrix2 m, out Matrix2 result)
        {
            result = Transpose(m);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix2 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix2 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">Left matrix</param>
        /// <param name="rhs">Right matrix</param>
        /// <returns>True if equal, false if not</returns>
        public static bool operator ==(Matrix2 lhs, Matrix2 rhs) 
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">Left matrix</param>
        /// <param name="rhs">Right matrix</param>
        /// <returns>True if not equal, false if equal</returns>
        public static bool operator !=(Matrix2 lhs, Matrix2 rhs) 
        {
            return ! lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Row0, Row1);
        }

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
            var r0 = Row0.ToString(format, formatProvider);
            var r1 = Row1.ToString(format, formatProvider);

            return string.Format("{0}\n{1}", r0, r1);
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix2 to Matrix2
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator Matrix2(OpenTK.Mathematics.Matrix2 m)
        {
            return new Matrix2
            {
                Row0 = m.Row0,
                Row1 = m.Row1
            };
        }

        /// <summary>
        /// Handle conversion from Matrix2 to OpenTK's Matrix2
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator OpenTK.Mathematics.Matrix2(Matrix2 m)
        {
            return new OpenTK.Mathematics.Matrix2
            {
                Row0 = m.Row0,
                Row1 = m.Row1
            };
        }
        #endregion
#endif
    }
}
