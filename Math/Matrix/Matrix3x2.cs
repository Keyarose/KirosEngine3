using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Three by two matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x2 : IEquatable<Matrix3x2>, IFormattable
    {
        public Vec2 Row0;
        public Vec2 Row1;
        public Vec2 Row2;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix3x2 Zero => new Matrix3x2(Vec2.Zero, Vec2.Zero, Vec2.Zero);

        /// <summary>
        /// The first column of the matrix.
        /// </summary>
        public Vec3 Column0
        {
            readonly get => new Vec3(Row0.X, Row1.X, Row2.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
                Row2.X = value.Z;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec3 Column1
        {
            readonly get => new Vec3(Row0.Y, Row1.Y, Row2.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
                Row2.Y = value.Z;
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

        /// <summary>
        /// Accessor for row 2, column 0
        /// </summary>
        public float M20
        {
            readonly get { return Row2.X; }
            set { Row2.X = value; }
        }

        /// <summary>
        /// Accessor for row 2, column 1
        /// </summary>
        public float M21
        {
            readonly get { return Row2.Y; }
            set { Row2.Y = value; }
        }
        #endregion

        /// <summary>
        /// The main diagonal of the matrix.
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
                Row1.Y = value.Y;
            }
        }

        /// <summary>
        /// The matrix's trace, sum of the main diagonal
        /// </summary>
        public readonly float Trace => Row0.X + Row1.Y;

        /// <summary>
        /// Array type accessor for the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns>The value at the given indexes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown in the index values are out of the allowed range.</exception>
        public float this[int row, int column]
        {
            readonly get
            {
                if (column < 0 || column > 1)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix3x2.", column));

                switch (row)
                {
                    case 0:
                        return Row0[column];
                    case 1:
                        return Row1[column];
                    case 2:
                        return Row2[column];
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3x2.", row));
                }
            }
            set
            {
                if (column < 0 || column > 1)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix3x2.", column));

                switch (row)
                {
                    case 0:
                        Row0[column] = value;
                        break;
                    case 1:
                        Row1[column] = value;
                        break;
                    case 2:
                        Row2[column] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3x2.", row));
                }
            }
        }

        /// <summary>
        /// Array type accessor for the rows of the matrix
        /// </summary>
        /// <param name="row">Row index</param>
        /// <returns>The row at the given index</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index value is out of the allowed range.</exception>
        public Vec2 this[int row]
        {
            readonly get
            {
                switch (row)
                {
                    case 0:
                        return Row0;
                    case 1:
                        return Row1;
                    case 2:
                        return Row2;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3x2.", row));
                }
            }
            set
            {
                switch (row)
                {
                    case 0:
                        Row0 = value;
                        break;
                    case 1:
                        Row1 = value;
                        break;
                    case 2:
                        Row2 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3x2.", row));
                }
            }
        }
                
        #region Constructors
        /// <summary>
        /// Basic constructor using Vec2s
        /// </summary>
        /// <param name="r0">Row 0</param>
        /// <param name="r1">Row 1</param>
        /// <param name="r2">Row 2</param>
        public Matrix3x2(Vec2 r0, Vec2 r1, Vec2 r2)
        {
            Row0 = r0; Row1 = r1; Row2 = r2;
        }

        /// <summary>
        /// Basic constructor using individual floats
        /// </summary>
        /// <param name="m00">Row 0, Column 0</param>
        /// <param name="m01">Row 0, Column 1</param>
        /// <param name="m10">Row 1, Column 0</param>
        /// <param name="m11">Row 1, Column 1</param>
        /// <param name="m20">Row 2, Column 0</param>
        /// <param name="m21">Row 2, Column 1</param>
        public Matrix3x2(float m00, float m01, float m10, float m11, float m20, float m21)
        {
            Row0 = new(m00, m01);
            Row1 = new(m10, m11);
            Row2 = new(m20, m21);
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Get a transposed copy of the matrix.
        /// </summary>
        /// <returns>The resulting transpose.</returns>
        public readonly Matrix2x3 TransposedCopy()
        {
            return Transpose(this);
        }

        /// <summary>
        /// Get the transpose of a matrix.
        /// </summary>
        /// <param name="mat">The matrix to transpose.</param>
        /// <returns>The transpose in a new instance.</returns>
        public static Matrix2x3 Transpose(Matrix3x2 mat)
        {
            return new Matrix2x3(mat.Column0, mat.Column1);
        }

        /// <summary>
        /// Get the transpose of the matrix.
        /// </summary>
        /// <param name="mat">The matrix to transpose.</param>
        /// <param name="result">The resulting transpose.</param>
        public static void Transpose(Matrix3x2 mat, out Matrix2x3 result)
        {
            result = Transpose(mat);
        }
        #endregion

        //todo: scale

        //todo: rotate

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 Add(Matrix3x2 lhs, Matrix3x2 rhs)
        {
            return new Matrix3x2(lhs.Row0 + rhs.Row0, lhs.Row1 + rhs.Row1, lhs.Row2 + rhs.Row2);
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Add(Matrix3x2 lhs, Matrix3x2 rhs, out Matrix3x2 result)
        {
            result = Add(lhs, rhs);
        }

        /// <summary>
        /// Addition operator between two Matrix3x2s
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 operator +(Matrix3x2 lhs, Matrix3x2 rhs)
        {
            return Add(lhs, rhs);
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 Subtract(Matrix3x2 lhs, Matrix3x2 rhs)
        {
            return new Matrix3x2(lhs.Row0 - rhs.Row0, lhs.Row1 - rhs.Row1, lhs.Row2 - rhs.Row2);
        }

        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Subtract(Matrix3x2 lhs, Matrix3x2 rhs, out Matrix3x2 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <summary>
        /// Subtraction operator between two Matrix3x2s
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 operator -(Matrix3x2 lhs, Matrix3x2 rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        //todo: multiply

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix3x2 other) 
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2;
        }

        /// <summary>
        /// Indicates whether the current Matrix3x2 is equal to another within the provided tolerance.
        /// </summary>
        /// <param name="other">The other Matrix3x2.</param>
        /// <param name="tolerance">The allowed difference between the values.</param>
        /// <returns>True if the difference between the two Matrix3x2s is less than the tolerance,
        /// false otherwise.</returns>
        public readonly bool Equals(Matrix3x2 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance) && Row2.Equals(other.Row2, tolerance);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix3x2 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator ==(Matrix3x2 lhs, Matrix3x2 rhs) 
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if not equal, false if equal.</returns>
        public static bool operator !=(Matrix3x2 lhs, Matrix3x2 rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Row0, Row1, Row2);
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
            var r0 = Row0.ToString(format, formatProvider);
            var r1 = Row1.ToString(format, formatProvider);
            var r2 = Row2.ToString(format, formatProvider);

            return string.Format("{0}\n{1}\n{2}", r0, r1, r2);
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix3x2 to Matrix3x2.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator Matrix3x2(OpenTK.Mathematics.Matrix3x2 m)
        {
            return new Matrix3x2(m.Row0, m.Row1, m.Row2);
        }

        /// <summary>
        /// Handle conversion from Matrix3x2 to OpenTK's Matrix3x2.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator OpenTK.Mathematics.Matrix3x2(Matrix3x2 m)
        {
            return new OpenTK.Mathematics.Matrix3x2(m.Row0, m.Row1, m.Row2);
        }
        #endregion
#endif
    }
}
