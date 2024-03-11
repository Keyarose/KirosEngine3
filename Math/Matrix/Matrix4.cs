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
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 : IEquatable<Matrix4>, IFormattable
    {
        public Vec4 Row0;
        public Vec4 Row1; 
        public Vec4 Row2; 
        public Vec4 Row3;

        public static readonly Matrix4 Identity = new Matrix4(Vec4.UnitX, Vec4.UnitY, Vec4.UnitZ, Vec4.UnitW);

        public static readonly Matrix4 Zero = new Matrix4(Vec4.Zero, Vec4.Zero, Vec4.Zero, Vec4.Zero);

        /// <summary>
        /// The first column of the matrix
        /// </summary>
        public readonly Vec4 Column0 => new Vec4(Row0.X, Row1.X, Row2.X, Row3.X);

        /// <summary>
        /// The second column of the matrix
        /// </summary>
        public readonly Vec4 Column1 => new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y);

        /// <summary>
        /// The third column of the matrix
        /// </summary>
        public readonly Vec4 Column2 => new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z);

        /// <summary>
        /// The fourth column of the matrix
        /// </summary>
        public readonly Vec4 Column3 => new Vec4(Row0.W, Row1.W, Row2.W, Row3.W);

        /// <summary>
        /// Calculate the matrix's determinant
        /// </summary>
        public readonly float Determinant
        {
            get
            {
                float result = 0;

                result += Row0.X * new Matrix3(M11, M12, M13, M21, M22, M23, M31, M32, M33).Determinant;
                result -= Row0.Y * new Matrix3(M10, M12, M13, M20, M22, M23, M30, M32, M33).Determinant;
                result += Row0.Z * new Matrix3(M10, M11, M13, M20, M21, M23, M30, M31, M33).Determinant;
                result -= Row0.W * new Matrix3(M10, M11, M12, M20, M21, M22, M30, M31, M32).Determinant;

                return result;
            }
        }

        #region Cell Accessors
        public float M00
        {
            readonly get { return Row0.X; }
            set { Row0.X = value; }
        }

        public float M01
        {
            readonly get { return Row0.Y; }
            set { Row0.Y = value; }
        }

        public float M02
        {
            readonly get { return Row0.Z; }
            set { Row0.Z = value; }
        }

        public float M03
        {
            readonly get { return Row0.W; }
            set { Row0.W = value; }
        }

        public float M10
        {
            readonly get { return Row1.X; }
            set { Row1.X = value; }
        }

        public float M11
        {
            readonly get { return Row1.Y; }
            set { Row1.Y = value; }
        }

        public float M12
        {
            readonly get { return Row1.Z; }
            set { Row1.Z = value; }
        }

        public float M13
        {
            readonly get { return Row1.W; }
            set { Row1.W = value; }
        }

        public float M20
        {
            readonly get { return Row2.X; }
            set { Row2.X = value; }
        }

        public float M21
        {
            readonly get { return Row2.Y; }
            set { Row2.Y = value; }
        }

        public float M22
        {
            readonly get { return Row2.Z; }
            set { Row2.Z = value; }
        }

        public float M23
        {
            readonly get { return Row2.W; }
            set { Row2.W = value; }
        }

        public float M30
        {
            readonly get { return Row3.X; }
            set { Row3.X = value; }
        }

        public float M31
        {
            readonly get { return Row3.Y; }
            set { Row3.Y = value; }
        }

        public float M32
        {
            readonly get { return Row3.Z; }
            set { Row3.Z = value; }
        }

        public float M33
        {
            readonly get { return Row3.W; }
            set { Row3.W = value; }
        }
        #endregion

        /// <summary>
        /// Accessor for the matrix's diagonal
        /// </summary>
        public Vec4 Diagonal
        {
            readonly get
            {
                return new Vec4(Row0.X, Row1.Y, Row2.Z, Row3.W);
            }
            set
            {
                Row0.X = value.X;
                Row1.Y = value.Y;
                Row2.Z = value.Z;
                Row3.W = value.W;
            }
        }

        /// <summary>
        /// The matrix's trace, the sum of it's diagonal values
        /// </summary>
        public readonly float Trace
        {
            get
            {
                return Row0.X + Row1.Y + Row2.Z + Row3.W;
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
                if (column < 0 && column > 3)
                {
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix4.", column));
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
                else if (row == 3)
                {
                    return Row3[column];
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix4.", row));
                }
            }
            set
            {
                if (column < 0 && column > 3)
                {
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} out of range for Matrix4.", column));
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
                else if (row == 3)
                {
                    Row3[column] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix4.", row));
                }
            }
        }

        /// <summary>
        /// Basic constructor using Vec4s
        /// </summary>
        /// <param name="r0">First row of the matrix</param>
        /// <param name="r1">Second row of the matrix</param>
        /// <param name="r2">Third row of the matrix</param>
        /// <param name="r3">Fourth row of the matrix</param>
        public Matrix4(Vec4 r0, Vec4 r1, Vec4 r2, Vec4 r3) 
        {
            Row0 = r0;
            Row1 = r1;
            Row2 = r2;
            Row3 = r3;
        }

        /// <summary>
        /// Basic constructor using indvidual floats
        /// </summary>
        /// <param name="m00">Row 0, Column 0</param>
        /// <param name="m01">Row 0, Column 1</param>
        /// <param name="m02">Row 0, Column 2</param>
        /// <param name="m03">Row 0, Column 3</param>
        /// <param name="m10">Row 1, Column 0</param>
        /// <param name="m11">Row 1, Column 1</param>
        /// <param name="m12">Row 1, Column 2</param>
        /// <param name="m13">Row 1, Column 3</param>
        /// <param name="m20">Row 2, Column 0</param>
        /// <param name="m21">Row 2, Column 1</param>
        /// <param name="m22">Row 2, Column 2</param>
        /// <param name="m23">Row 2, Column 3</param>
        /// <param name="m30">Row 3, Column 0</param>
        /// <param name="m31">Row 3, Column 1</param>
        /// <param name="m32">Row 3, Column 2</param>
        /// <param name="m33">Row 3, Column 3</param>
        public Matrix4(float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
            Row3 = new Vec4(m30, m31, m32, m33);
        }

        /// <summary>
        /// Normalize the matrix by dividing by the determinant, should be checked for nan and infinites
        /// </summary>
        public void Normalize()
        {
            var det = Determinant;
            Row0 /= det;
            Row1 /= det;
            Row2 /= det; 
            Row3 /= det;
            //todo: 0 division handling
        }

        /// <summary>
        /// Create a normalized copy of the matrix
        /// </summary>
        /// <returns>A copy of the matrix that has been normalized</returns>
        public readonly Matrix4 NormalizedCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }

        //todo:Invert(), InvertedCopy()

        /// <summary>
        /// Convert a matrix into it's transpose
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

        #region Translation
        /// <summary>
        /// Create a copy of the matrix without translation
        /// </summary>
        /// <returns>The matrix without translation</returns>
        public readonly Matrix4 ClearTranslation()
        {
            var c = this;
            c.Row3.Xyz = Vec3.Zero;
            return c;
        }
        #endregion

        #region Scale
        /// <summary>
        /// Create a copy of the matrix without any scaling
        /// </summary>
        /// <returns>The matrix without scaling</returns>
        public readonly Matrix4 ClearScale()
        {
            var c = this;
            c.Row0.Xyz = c.Row0.Xyz.NormalizedCopy();
            c.Row1.Xyz = c.Row1.Xyz.NormalizedCopy();
            c.Row2.Xyz = c.Row2.Xyz.NormalizedCopy();
            return c;
        }

        /// <summary>
        /// Get the scale components of the matrix
        /// </summary>
        /// <returns>The 3D vector representing the scale components</returns>
        public readonly Vec3 GetScale()
        {
            return new Vec3(Row0.Xyz.Length, Row1.Xyz.Length, Row2.Xyz.Length);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <param name="z">The Z dimension scale factor</param>
        /// <returns>The matrix that represents the 3 scale factors</returns>
        public static Matrix4 CreateScale(float x, float y, float z)
        {
            var r = Identity;
            r.Row0.X = x;
            r.Row0.Y = y;
            r.Row0.Z = z;
            return r;
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <param name="z">The Z dimension scale factor</param>
        /// <param name="result">The matrix that represents the 3 scale factors</param>
        public static void CreateScale(float x, float y, float z, out Matrix4 result)
        {
            result = CreateScale(x, y, z);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in all 3 dimensions</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix4 CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in all 3 dimensions</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(float scale, out Matrix4 result)
        {
            result = CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factors for each dimension</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix4 CreateScale(Vec3 scale)
        {
            return CreateScale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factors for each dimension</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(Vec3 scale, out Matrix4 result)
        {
            result = CreateScale(scale.X, scale.Y, scale.Z);
        }
        #endregion

        #region Rotation
        public readonly Matrix4 ClearRotation()
        {
            var c = this;
            c.Row0.Xyz = new Vec3(Row0.Xyz.Length, 0.0f, 0.0f);
            c.Row1.Xyz = new Vec3(0.0f, Row1.Xyz.Length, 0.0f);
            c.Row2.Xyz = new Vec3(0.0f, 0.0f, Row2.Xyz.Length);
            return c;
        }

        //todo: getRotation, createRotation
        #endregion

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 Add(Matrix4 m1, Matrix4 m2)
        {
            var r = new Matrix4
            {
                Row0 = m1.Row0 + m2.Row0,
                Row1 = m1.Row1 + m2.Row1,
                Row2 = m1.Row2 + m2.Row2,
                Row3 = m1.Row3 + m2.Row3,
            };
            return r;
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="m1">First matrix to add</param>
        /// <param name="m2">Second matrix to add</param>
        /// <param name="result">The resulting matrix</param>
        public static void Add(Matrix4 m1, Matrix4 m2, out Matrix4 result)
        {
            result = Add(m1, m2);
        }

        /// <summary>
        /// Define the addition operator for two matrices
        /// </summary>
        /// <param name="lhs">Left matrix to add</param>
        /// <param name="rhs">Right matrix to add</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 operator +(Matrix4 lhs, Matrix4 rhs)
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
        public static Matrix4 Multiply(Matrix4 m1, Matrix4 m2)
        {
            var r = new Matrix4
            {
                Row0 = new Vec4(Vec4.Dot(m1.Row0, m2.Column0), Vec4.Dot(m1.Row0, m2.Column1), Vec4.Dot(m1.Row0, m2.Column2), Vec4.Dot(m1.Row0, m2.Column3)),
                Row1 = new Vec4(Vec4.Dot(m1.Row1, m2.Column0), Vec4.Dot(m1.Row1, m2.Column1), Vec4.Dot(m1.Row1, m2.Column2), Vec4.Dot(m1.Row1, m2.Column3)),
                Row2 = new Vec4(Vec4.Dot(m1.Row2, m2.Column0), Vec4.Dot(m1.Row2, m2.Column1), Vec4.Dot(m1.Row2, m2.Column2), Vec4.Dot(m1.Row2, m2.Column3)),
                Row3 = new Vec4(Vec4.Dot(m1.Row3, m2.Column0), Vec4.Dot(m1.Row3, m2.Column1), Vec4.Dot(m1.Row3, m2.Column2), Vec4.Dot(m1.Row3, m2.Column3))
            };

            return r;
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="m1">First matrix</param>
        /// <param name="m2">Second matrix</param>
        /// <param name="result">The resulting matrix</param>
        public static void Multiply(Matrix4 m1, Matrix4 m2, out Matrix4 result)
        {
            result = Multiply(m1, m2);
        }

        /// <summary>
        /// Define the multiplication of two matrices
        /// </summary>
        /// <param name="lhs">The left matrix</param>
        /// <param name="rhs">The right matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 operator *(Matrix4 lhs, Matrix4 rhs) 
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        //todo: invert

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 Transpose(Matrix4 m)
        {
            return new Matrix4(m.Column0, m.Column1, m.Column2, m.Column3);
        }

        /// <summary>
        /// Find the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <param name="result">The resulting matrix</param>
        public static void Transpose(Matrix4 m, out Matrix4 result)
        {
            result = Transpose(m);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix4 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2 && Row3 == other.Row3;
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix4 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left matrix</param>
        /// <param name="rhs">The right matrix</param>
        /// <returns>True if equal, false otherwise</returns>
        public static bool operator ==(Matrix4 lhs, Matrix4 rhs) 
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">Left matrix</param>
        /// <param name="rhs">Right matrix</param>
        /// <returns>False if equal, true otherwise</returns>
        public static bool operator !=(Matrix4 lhs, Matrix4 rhs) 
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Row0, Row1, Row2, Row3);
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
            var r2 = Row2.ToString(format, formatProvider);
            var r3 = Row3.ToString(format, formatProvider);

            return string.Format("{0}\n{1}\n{2}\n{3}", r0, r1, r2, r3);
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix4 to Matrix4
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator Matrix4(OpenTK.Mathematics.Matrix4 m)
        {
            return new Matrix4
            {
                Row0 = m.Row0,
                Row1 = m.Row1,
                Row2 = m.Row2,
                Row3 = m.Row3
            };
        }

        /// <summary>
        /// Handle conversion from Matrix4 to OpenTK's Matrix4
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator OpenTK.Mathematics.Matrix4(Matrix4 m)
        {
            return new OpenTK.Mathematics.Matrix4
            {
                Row0 = m.Row0,
                Row1 = m.Row1,
                Row2 = m.Row2,
                Row3 = m.Row3
            };
        }
        #endregion
#endif
    }
}
