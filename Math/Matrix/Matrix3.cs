
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
    /// Three by three matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3 : IEquatable<Matrix3>, IFormattable
    {
        public Vec3 Row0;
        public Vec3 Row1;
        public Vec3 Row2;

        /// <summary>
        /// The Identity matrix
        /// </summary>
        public static readonly Matrix3 Identity = new Matrix3(Vec3.UnitX, Vec3.UnitY, Vec3.UnitZ);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static readonly Matrix3 Zero = new Matrix3(Vec3.Zero, Vec3.Zero, Vec3.Zero);

        /// <summary>
        /// The first column of the matrix
        /// </summary>
        public readonly Vec3 Column0 => new Vec3(Row0.X, Row1.X, Row2.X);

        /// <summary>
        /// The second column of the matrix
        /// </summary>
        public readonly Vec3 Column1 => new Vec3(Row0.Y, Row1.Y, Row2.Y);

        /// <summary>
        /// The third column of the matrix
        /// </summary>
        public readonly Vec3 Column2 => new Vec3(Row0.Z, Row1.Z, Row2.Z);

        /// <summary>
        /// Calculate the matrix's determinant
        /// </summary>
        public readonly float Determinant
        {
            get
            {
                float result = 0;

                result += Row0.X * new Matrix2(M11, M12, M21, M22).Determinant;
                result -= Row0.Y * new Matrix2(M10, M12, M20, M22).Determinant;
                result += Row0.Z * new Matrix2(M10, M11, M20, M21).Determinant;

                return result;
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
        /// Accessor for row 0, column 2
        /// </summary>
        public float M02
        {
            readonly get { return Row0.Z; }
            set { Row0.Z = value; }
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
        /// Accessor for row 1, column 2
        /// </summary>
        public float M12
        {
            readonly get { return Row1.Z; }
            set { Row1.Z = value; }
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

        /// <summary>
        /// Accessor for row 2, column 2
        /// </summary>
        public float M22
        {
            readonly get { return Row2.Z; }
            set { Row2.Z = value; }
        }
        #endregion

        /// <summary>
        /// Accessor for the matrix's diagonal
        /// </summary>
        public Vec3 Diagonal
        {
            readonly get
            {
                return new Vec3(Row0.X, Row1.Y, Row2.Z);
            }
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
            }
        }

        /// <summary>
        /// The matrix's trace, the sum of it's diagonal values
        /// </summary>
        public readonly float Trace
        {
            get
            {
                return Row0.X + Row1.Y + Row2.Z;
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
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix3.", column));
                }

                if (row == 0)
                {
                    return Row0[column];
                }
                else if (row == 1)
                {
                    return Row1[column];
                }
                else if (row == 2)
                {
                    return Row2[column];
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3.", row));
                }
            }
            set
            {
                if (column < 0 && column > 2)
                {
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix3.", column));
                }

                if (row == 0)
                {
                    Row0[column] = value;
                }
                else if (row == 1)
                {
                    Row1[column] = value;
                }
                else if (row == 2)
                {
                    Row2[column] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3.", row));
                }
            }
        }

        /// <summary>
        /// Basic constructor using Vec3s
        /// </summary>
        /// <param name="r0">First row of the matrix</param>
        /// <param name="r1">Second row of the matrix</param>
        /// <param name="r2">Third row of the matrix</param>
        public Matrix3(Vec3 r0, Vec3 r1, Vec3 r2)
        {
            Row0 = r0;
            Row1 = r1;
            Row2 = r2;
        }

        /// <summary>
        /// Basic constructor using indvidual floats
        /// </summary>
        /// <param name="m00">Row 0, Column 0</param>
        /// <param name="m01">Row 0, Column 1</param>
        /// <param name="m02">Row 0, Column 2</param>
        /// <param name="m10">Row 1, Column 0</param>
        /// <param name="m11">Row 1, Column 1</param>
        /// <param name="m12">Row 1, Column 2</param>
        /// <param name="m20">Row 2, Column 0</param>
        /// <param name="m21">Row 2, Column 1</param>
        /// <param name="m22">Row 2, Column 2</param>
        public Matrix3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            Row0 = new Vec3(m00, m01, m02);
            Row1 = new Vec3(m10, m11, m12);
            Row2 = new Vec3(m20, m21, m22);
        }

        /// <summary>
        /// Normalize the matrix by dividing by the determinant, should be checked for nan and infinites
        /// </summary>
        public void Normalize()
        {
            var determinant = Determinant;
            Row0 /= determinant;
            Row1 /= determinant;
            Row2 /= determinant;
            //todo: 0 division handling
        }

        /// <summary>
        /// Create a normalized copy of the matrix, should be checked for nan and infinites
        /// </summary>
        /// <returns>A copy of the matrix that has been normalized</returns>
        public readonly Matrix3 NormalizedCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }

        /// <summary>
        /// Converts a matrix into it's inverse
        /// </summary>
        public void Invert()
        {
            this = Invert(this);
        }

        /// <summary>
        /// Create an inverted copy of the matrix
        /// </summary>
        /// <returns>An inverted copy of the matrix or a copy of the matrix if it is singular</returns>
        public readonly Matrix3 InvertedCopy()
        {
            var c = this;
            if(c.Determinant != 0)
            {
                c.Invert();
            }
            return c;
        }

        /// <summary>
        /// Converts a matrix into it's transpose
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

        #region Scale
        /// <summary>
        /// Create a copy of the matrix without any scaling
        /// </summary>
        /// <returns>The matrix without scaling</returns>
        public readonly Matrix3 ClearScale()
        {
            var c = this;
            c.Row0 = c.Row0.NormalizedCopy();
            c.Row1 = c.Row1.NormalizedCopy();
            c.Row2 = c.Row2.NormalizedCopy();
            return c;
        }

        /// <summary>
        /// Gets the scale components of the matrix
        /// </summary>
        /// <returns>The 3d vector representing the scale components</returns>
        public readonly Vec3 GetScale()
        {
            return new Vec3(Row0.Length, Row1.Length, Row2.Length);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in all 3 dimensions</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix3 CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in all 3 dimensions</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(float scale, out Matrix3 result)
        {
            result = CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <param name="z">The Z dimension scale factor</param>
        /// <returns>The matrix that represents the 3 scale factors</returns>
        public static Matrix3 CreateScale(float x, float y, float z)
        {
            var r = Identity;
            r.Row0.X = x;
            r.Row1.Y = y;
            r.Row2.Z = z;
            return r;
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <param name="z">The Z dimension scale factor</param>
        /// <param name="result">The matrix that represents the 3 scale factors</param>
        public static void CreateScale(float x, float y, float z, out Matrix3 result)
        {
            result = CreateScale(x, y, z);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factors for each dimension</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix3 CreateScale(Vec3 scale)
        {
            return CreateScale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factors for each dimension</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(Vec3 scale, out Matrix3 result)
        {
            result = CreateScale(scale.X, scale.Y, scale.Z);
        }
        #endregion

        /// <summary>
        /// Create a copy of the matrix with the rotation removed
        /// </summary>
        /// <returns>The matrix without rotation</returns>
        public readonly Matrix3 ClearRotation()
        {
            var c = this;
            c.Row0 = new Vec3(Row0.Length, 0.0f, 0.0f);
            c.Row1 = new Vec3(0.0f, Row1.Length, 0.0f);
            c.Row2 = new Vec3(0.0f, 0.0f, Row2.Length);
            return c;
        }

        //todo: getRotation, createRotation

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <returns>The resulting matrix in a new instance</returns>
        public static Matrix3 Add(Matrix3 m1, Matrix3 m2)
        {
            var r = new Matrix3
            {
                Row0 = m1.Row0 + m2.Row0,
                Row1 = m1.Row1 + m2.Row1,
                Row2 = m1.Row2 + m2.Row2
            };
            return r;
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <param name="result">The resulting matrix in a new instance</param>
        public static void Add(Matrix3 m1, Matrix3 m2, out Matrix3 result)
        {
            result = Add(m1, m2);
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="left">First matrix to add</param>
        /// <param name="right">Second matrix to add</param>
        /// <returns>The resulting matrix in a new instance</returns>
        public static Matrix3 operator +(Matrix3 left, Matrix3 right)
        {
            return Add(left, right);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">First matrix</param>
        /// <param name="m2">Second matrix</param>
        /// <returns>A new matrix containing the result</returns>
        public static Matrix3 Multiply(Matrix3 m1, Matrix3 m2)
        {
            //todo: rework as a series of dot products
            var r = new Matrix3
            {
                Row0 = new Vec3((m1[0, 0] * m2[0, 0]) + (m1[0, 1] * m2[1, 0]) + (m1[0, 2] * m2[2, 0]),
                    (m1[0, 0] * m2[0, 1]) + (m1[0, 1] * m2[1, 1]) + (m1[0, 2] * m2[2, 1]),
                    (m1[0, 0] * m2[0, 2]) + (m1[0, 1] * m2[1, 2]) + (m1[0, 2] * m2[2, 2])),

                Row1 = new Vec3((m1[1, 0] * m2[0, 0]) + (m1[1, 1] * m2[1, 0]) + (m1[1, 2] * m2[2, 0]),
                    (m1[1, 0] * m2[0, 1]) + (m1[1, 1] * m2[1, 1]) + (m1[1, 2] * m2[2, 1]),
                    (m1[1, 0] * m2[0, 2]) + (m1[1, 1] * m2[1, 2]) + (m1[1, 2] * m2[2, 2])),

                Row2 = new Vec3((m1[2, 0] * m2[0, 0]) + (m1[2, 1] * m2[1, 0]) + (m1[2, 2] * m2[2, 0]),
                    (m1[2, 0] * m2[0, 1]) + (m1[2, 1] * m2[1, 1]) + (m1[2, 2] * m2[2, 1]),
                    (m1[2, 0] * m2[0, 2]) + (m1[2, 1] * m2[1, 2]) + (m1[2, 2] * m2[2, 2]))
            };

            return r;
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">First matrix</param>
        /// <param name="m2">Second matrix</param>
        /// <param name="result">A new matrix containing the result</param>
        public static void Multiply(Matrix3 m1, Matrix3 m2, out Matrix3 result)
        {
            result = Multiply(m1, m2);
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">Left matrix</param>
        /// <param name="m2">Right matrix</param>
        /// <returns>A new matrix containing the result</returns>
        public static Matrix3 operator *(Matrix3 m1, Matrix3 m2)
        {
            return Multiply(m1, m2);
        }
        #endregion

        /// <summary>
        /// Invert the given matrix
        /// </summary>
        /// <param name="m">The matrix to invert</param>
        /// <returns>A new instance containing the inverted matrix</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular</exception>
        public static Matrix3 Invert(Matrix3 m)
        {
            var r = new Matrix3();

            float row0x = m.Row0.X, row0y = m.Row0.Y, row0z = m.Row0.Z;
            float row1x = m.Row1.X, row1y = m.Row1.Y, row1z = m.Row1.Z;
            float row2x = m.Row2.X, row2y = m.Row2.Y, row2z = m.Row2.Z;

            float inRow0X = (+row1y * row2z) - (row1z * row2y);
            float inRow1X = (-row1x * row2z) + (row1z * row2x);
            float inRow2X = (+row1x * row2y) - (row1y * row2x);

            //calculate the determinant here since we have to some of the work anyway
            float determ = (row0x * inRow0X) + (row0y * inRow1X) + (row0z * inRow2X);

            //check that the determinant isn't zero
            if(determ == 0f)
            {
                throw new InvalidOperationException("Matrix cannot be inverted as it is singular.");
            }

            //find matrix adjugate
            r.Row0.X = inRow0X;
            r.Row0.Y = (-row0y * row2z) + (row0z * row2y);
            r.Row0.Z = (+row0y * row1z) - (row0z * row1y);

            r.Row1.X = inRow1X;
            r.Row1.Y = (+row0x * row2z) - (row0z * row2x);
            r.Row1.Z = (-row0x * row1z) + (row0z * row1x);

            r.Row2.X = inRow2X;
            r.Row2.Y = (-row0x * row2y) + (row0y * row2x);
            r.Row2.Z = (+row0x * row1y) - (row0y * row1x);

            determ = 1.0f / determ;

            r.Row0.X *= determ;
            r.Row0.Y *= determ;
            r.Row0.Z *= determ;
            r.Row1.X *= determ;
            r.Row1.Y *= determ;
            r.Row1.Z *= determ;
            r.Row2.X *= determ;
            r.Row2.Y *= determ;
            r.Row2.Z *= determ;

            return r;
        }

        /// <summary>
        /// Inverts the given matrix
        /// </summary>
        /// <param name="m">The matrix to invert</param>
        /// <param name="result">A new instance containing the inverted matrix</param>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular</exception>
        public static void Invert(Matrix3 m, out Matrix3 result)
        {
            result = Invert(m);
        }

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <returns>A new instance containing the transposed matrix</returns>
        public static Matrix3 Transpose(Matrix3 m)
        {
            var r = new Matrix3
            {
                Row0 = m.Column0,
                Row1 = m.Column1,
                Row2 = m.Column2
            };

            return r;
        }

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <param name="result">A new instance containing the transposed matrix</param>
        public static void Transpose(Matrix3 m, out Matrix3 result)
        {
            result = Transpose(m);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix3 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix3 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="left">Left matrix</param>
        /// <param name="right">Right matrix</param>
        /// <returns>True if equal, false if not</returns>
        public static bool operator ==(Matrix3 left, Matrix3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="left">Left matrix</param>
        /// <param name="right">Right matrix</param>
        /// <returns>True if not equal, false if equal</returns>
        public static bool operator !=(Matrix3 left, Matrix3 right)
        {
            return !left.Equals(right);
        }

        ///<inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Row0, Row1, Row2);
        }

        ///<inheritdoc/>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            var r0 = Row0.ToString(format, formatProvider);
            var r1 = Row1.ToString(format, formatProvider);
            var r2 = Row2.ToString(format, formatProvider);

            return string.Format(@"{0}\n{1}\n{2}", r0, r1, r2);
        }
    }
}
