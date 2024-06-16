using KirosEngine3.Math.Vector;
using KirosEngine3.Math.Data;
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
    /// Four by four matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4 : IEquatable<Matrix4>, IFormattable, IMatrix<Matrix4, Vec4, Vec4, Vec4, Matrix4>
    {
        /// <summary>
        /// The first row of the matrix.
        /// </summary>
        public Vec4 Row0;
        /// <summary>
        /// The second row of the matrix.
        /// </summary>
        public Vec4 Row1;
        /// <summary>
        /// The third row of the matrix.
        /// </summary>
        public Vec4 Row2;
        /// <summary>
        /// The fourth row of the matrix.
        /// </summary>
        public Vec4 Row3;

        /// <summary>
        /// The Identity matrix.
        /// </summary>
        public static Matrix4 Identity => new Matrix4(Vec4.UnitX, Vec4.UnitY, Vec4.UnitZ, Vec4.UnitW);

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix4 Zero => new Matrix4(Vec4.Zero, Vec4.Zero, Vec4.Zero, Vec4.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix
        /// </summary>
        public Vec4 Column0
        {
            readonly get => new Vec4(Row0.X, Row1.X, Row2.X, Row3.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
                Row2.X = value.Z;
                Row3.X = value.W;
            }
        }

        /// <summary>
        /// The second column of the matrix
        /// </summary>
        public Vec4 Column1
        {
            readonly get => new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
                Row2.Y = value.Z;
                Row3.Y = value.W;
            }
        }

        /// <summary>
        /// The third column of the matrix
        /// </summary>
        public Vec4 Column2
        {
            readonly get => new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
                Row2.Z = value.Z;
                Row3.Z = value.W;
            }
        }

        /// <summary>
        /// The fourth column of the matrix
        /// </summary>
        public Vec4 Column3
        {
            readonly get => new Vec4(Row0.W, Row1.W, Row2.W, Row3.W);
            set
            {
                Row0.W = value.X;
                Row1.W = value.Y;
                Row2.W = value.Z;
                Row3.W = value.W;
            }
        }
        #endregion

        /// <inheritdoc/>
        public readonly Vec4[] GetColumns()
        {
            return [Column0, Column1, Column2, Column3];
        }

        /// <inheritdoc/>
        public readonly Vec4[] GetRows()
        {
            return [Row0, Row1, Row2, Row3];
        }

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
        /// <summary>
        /// Accessor for row 0, column 0.
        /// </summary>
        public float M00
        {
            readonly get { return Row0.X; }
            set { Row0.X = value; }
        }

        /// <summary>
        /// Accessor for row 0, column 1.
        /// </summary>
        public float M01
        {
            readonly get { return Row0.Y; }
            set { Row0.Y = value; }
        }

        /// <summary>
        /// Accessor for row 0, column 2.
        /// </summary>
        public float M02
        {
            readonly get { return Row0.Z; }
            set { Row0.Z = value; }
        }

        /// <summary>
        /// Accessor for row 0, column 3.
        /// </summary>
        public float M03
        {
            readonly get { return Row0.W; }
            set { Row0.W = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 0.
        /// </summary>
        public float M10
        {
            readonly get { return Row1.X; }
            set { Row1.X = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 1.
        /// </summary>
        public float M11
        {
            readonly get { return Row1.Y; }
            set { Row1.Y = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 2.
        /// </summary>
        public float M12
        {
            readonly get { return Row1.Z; }
            set { Row1.Z = value; }
        }

        /// <summary>
        /// Accessor for row 1, column 3.
        /// </summary>
        public float M13
        {
            readonly get { return Row1.W; }
            set { Row1.W = value; }
        }

        /// <summary>
        /// Accessor for row 2, column 0.
        /// </summary>
        public float M20
        {
            readonly get { return Row2.X; }
            set { Row2.X = value; }
        }

        /// <summary>
        /// Accessor for row 2, column 1.
        /// </summary>
        public float M21
        {
            readonly get { return Row2.Y; }
            set { Row2.Y = value; }
        }

        /// <summary>
        /// Accessor for row 2, column 2.
        /// </summary>
        public float M22
        {
            readonly get { return Row2.Z; }
            set { Row2.Z = value; }
        }

        /// <summary>
        /// Accessor for row 2, column 3.
        /// </summary>
        public float M23
        {
            readonly get { return Row2.W; }
            set { Row2.W = value; }
        }

        /// <summary>
        /// Accessor for row 3, column 0.
        /// </summary>
        public float M30
        {
            readonly get { return Row3.X; }
            set { Row3.X = value; }
        }

        /// <summary>
        /// Accessor for row 3, column 1.
        /// </summary>
        public float M31
        {
            readonly get { return Row3.Y; }
            set { Row3.Y = value; }
        }

        /// <summary>
        /// Accessor for row 3, column 2.
        /// </summary>
        public float M32
        {
            readonly get { return Row3.Z; }
            set { Row3.Z = value; }
        }

        /// <summary>
        /// Accessor for row 3, column 3.
        /// </summary>
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
                if (column < 0 || column > 3)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix4", column));

                switch (row)
                {
                    case 0:
                        return Row0[column];
                    case 1:
                        return Row1[column];
                    case 2:
                        return Row2[column];
                    case 3:
                        return Row3[column];
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4", row));
                }
            }
            set
            {
                if (column < 0 || column > 3)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix4", column));

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
                    case 3:
                        Row3[column] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4", row));
                }
            }
        }

        /// <summary>
        /// Array type accessor for the rows of the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>The row at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is outside the range of 0-3</exception>
        public Vec4 this[int row]
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
                    case 3:
                        return Row3;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix4.", row));
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
                    case 3:
                        Row3 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix4.", row));
                }
            }
        }

        #region Constructors
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
        /// Construct a Matrix4 from a Matrix3.
        /// </summary>
        /// <param name="m">The Matrix3 to use as a basis.</param>
        /// <param name="w0">The W component for row 0, default 0.</param>
        /// <param name="w1">The W component for row 1, default 0.</param>
        /// <param name="w2">The W component for row 2, default 0.</param>
        /// <param name="m30">Row 3, Column 0, default 0.</param>
        /// <param name="m31">Row 3, Column 1, default 0.</param>
        /// <param name="m32">Row 3, Column 2, default 0.</param>
        /// <param name="m33">Row 3, Column 3, default 1.</param>
        public Matrix4(Matrix3 m, float w0 = 0, float w1 = 0, float w2 = 0,
                        float m30 = 0, float m31 = 0, float m32 = 0, float m33 = 1)
        {
            Row0 = new Vec4(m.Row0, w0);
            Row1 = new Vec4(m.Row1, w1);
            Row2 = new Vec4(m.Row2, w2);
            Row3 = new Vec4(m30, m31, m32, m33);
        }

        /// <summary>
        /// Construct a Matrix4 from a Matrix3.
        /// </summary>
        /// <param name="m">The Matrix3 to use as a basis.</param>
        /// <param name="wVals">The W component values for rows 0-2.</param>
        /// <param name="r3">Row 3 of the matrix.</param>
        public Matrix4(Matrix3 m, Vec3 wVals, Vec4 r3)
        {
            Row0 = new Vec4(m.Row0, wVals.X);
            Row1 = new Vec4(m.Row1, wVals.Y);
            Row2 = new Vec4(m.Row2, wVals.Z);
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
        #endregion

        #region ElementaryMatrices
        /// <summary>
        /// Produce an elementary matrix for row interchange between the two specified rows.
        /// </summary>
        /// <param name="r1">The index of the first row to interchange.</param>
        /// <param name="r2">The index of the second row to interchange.</param>
        /// <returns>The 3D elementary matrix that performs row interchange.</returns>
        /// <exception cref="InvalidOperationException">Thrown if both indexes are the same value.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if one of the row indexes is out of the allowed range.</exception>
        public static Matrix4 RowInterchangeElemMat(int r1, int r2)
        {
            if (r1 == r2)
                throw new InvalidOperationException(string.Format("Cannot interchange a row with itself."));

            if (r1 < 0 || r1 > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r1));
            if (r2 < 0 || r2 > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r2));

            if (r1 == 0 || r2 == 0)
            {
                if (r1 == 1 || r2 == 1)//switch row 0 and row 1
                {
                    return new Matrix4(0.0f, 1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                }
                else if (r1 == 2 || r2 == 2)//switch row 0 and row 2
                {
                    return new Matrix4(0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                }
                //switch row 0 and row 3
                return new Matrix4(0.0f, 0.0f, 0.0f, 1.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f, 0.0f);
            }
            else if (r1 == 1 || r2 == 1)
            {
                if (r1 == 2 || r2 == 2)//switch row 1 and row 2
                {
                    return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                }
                //switch row 1 and row 3
                return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f);
            }

            //switch row 2 and row 3
            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                0.0f, 1.0f, 0.0f, 0.0f,
                                0.0f, 0.0f, 0.0f, 1.0f,
                                0.0f, 0.0f, 1.0f, 0.0f);
        }

        /// <summary>
        /// Produce an elementary matrix for the scalar multiplication row operation.
        /// </summary>
        /// <param name="row">The row to be multiplied.</param>
        /// <param name="scalar">The scalar to multiply by, cannot be zero.</param>
        /// <returns>The 3D elementary matrix that performs the scalar multiplication.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the scalar is zero.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if the row index is out of range.</exception>
        public static Matrix4 RowMultiplyElemMat(int row, float scalar)
        {
            if (scalar.IsZero())
                throw new InvalidOperationException(string.Format("Multiplying a row by zero is not allowed."));

            switch (row)
            {
                case 0:
                    return new Matrix4(scalar, 0.0f, 0.0f, 0.0f, 
                                    0.0f, 1.0f, 0.0f, 0.0f, 
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                case 1:
                    return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f, 
                                    0.0f, scalar, 0.0f, 0.0f, 
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                case 2:
                    return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f, 
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, scalar, 0.0f,
                                    0.0f, 0.0f, 0.0f, 1.0f);
                case 3:
                    return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f, scalar);
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", row));
            }
        }

        /// <summary>
        /// Produce an elementary matrix for adding one row to another.
        /// </summary>
        /// <param name="r1">The index of the row to add.</param>
        /// <param name="r2">The index of the row to add to.</param>
        /// <param name="scalar">The number of times to add the first row, cannot be zero.</param>
        /// <returns>The 3D elementary matrix that performs the row operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the indexes are the same, or if the scalar is zero.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if the indexes are out of range for Matrix4.</exception>
        public static Matrix4 RowAddElemMat(int r1, int r2, float scalar)
        {
            if (r1 == r2)
                throw new InvalidOperationException(string.Format("Adding a row to itself is not a valid operation."));
            if (scalar.IsZero())
                throw new InvalidOperationException(string.Format("Multiplying a row by zero is not allowed."));

            switch (r1)
            {
                case 0:
                    {
                        if (r2 == 1)//add row 0 to row 1
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            scalar, 1.0f, 0.0f, 0.0f, 
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 2)//add row 0 to row 2
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            scalar, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 3)//add row 0 to row 3
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            scalar, 0.0f, 0.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r2));
                    }
                case 1:
                    {
                        if (r2 == 0)//add row 1 to row 0
                        {
                            return new Matrix4(1.0f, scalar, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 2)//add row 1 to row 2
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, scalar, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 3)//add row 1 to row 3
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, scalar, 0.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r2));
                    }
                case 2:
                    {
                        if (r2 == 0)//add row 2 to row 0
                        {
                            return new Matrix4(1.0f, 0.0f, scalar, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 1)//add row 2 to row 1
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, scalar, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 3)//add row 2 to row 3
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, scalar, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r2));
                    }
                case 3:
                    {
                        if (r2 == 0)//add row 3 to row 0
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, scalar,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 1)//add row 3 to row 1
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, scalar,
                                            0.0f, 0.0f, 1.0f, 0.0f,
                                            0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 2)//add row 3 to row 2
                        {
                            return new Matrix4(1.0f, 0.0f, 0.0f, 0.0f,
                                            0.0f, 1.0f, 0.0f, 0.0f,
                                            0.0f, 0.0f, 1.0f, scalar,
                                            0.0f, 0.0f, 1.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r2));
                    }
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix4.", r1));
            }
        }
        #endregion

        #region RowOperations
        /// <summary>
        /// Perform the row interchange operation on this matrix.
        /// </summary>
        /// <param name="r1">Index of the first row to interchange.</param>
        /// <param name="r2">Index of the second row to interchange.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix4 RowInterchange(int r1, int r2)
        {
            return RowInterchangeElemMat(r1, r2) * this;
        }

        /// <summary>
        /// Perform the row multiplication operation on this matrix.
        /// </summary>
        /// <param name="row">Index of the row to multiply.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix4 RowMultiplication(int row, float scalar)
        {
            return RowMultiplyElemMat(row, scalar) * this;
        }

        /// <summary>
        /// Perform the row addition operation on this matrix.
        /// </summary>
        /// <param name="r1">Index of the row to add.</param>
        /// <param name="r2">Index of the row to add to.</param>
        /// <param name="scalar">The number of times to add the first row.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix4 RowAddition(int r1, int r2, float scalar)
        {
            return RowAddElemMat(r1, r2, scalar) * this;
        }
        #endregion

        #region View&Proj
        /// <summary>
        /// Construct a world space to a camera space matrix (row major)
        /// </summary>
        /// <param name="pos">The camera's position</param>
        /// <param name="target">The target to look at</param>
        /// <param name="up">The up direction of the camera space</param>
        /// <returns>A conversion matrix between world space and camera space</returns>
        public static Matrix4 LookAt(Vec3 pos, Vec3 target, Vec3 up)
        {
            Vec3 zAxis = Vec3.Normalize(pos - target);
            Vec3 xAxis = Vec3.Normalize(Vec3.Cross(up, zAxis));
            Vec3 yAxis = Vec3.Normalize(Vec3.Cross(zAxis, xAxis));
            Matrix4 result = new Matrix4
            {
                Row0 = new Vec4(xAxis.X, yAxis.X, zAxis.X, 0.0f),
                Row1 = new Vec4(xAxis.Y, yAxis.Y, zAxis.Y, 0.0f),
                Row2 = new Vec4(xAxis.Z, yAxis.Z, zAxis.Z, 0.0f),
                Row3 = new Vec4(-Vec3.Dot(xAxis, pos), -Vec3.Dot(yAxis, pos), -Vec3.Dot(zAxis, pos), 1.0f)
            };

            return result;
        }//todo: deal with nan cases

        /// <summary>
        /// Construct a perspective projection matrix (row major)
        /// </summary>
        /// <param name="fovy">Angle of the field of view in y axis</param>
        /// <param name="aspect">The ratio of the view width/height</param>
        /// <param name="depthNear">Distance to the near clip plane</param>
        /// <param name="depthFar">Distance to the far clip plane</param>
        /// <returns>A perspective projection matrix</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if depthNear, depthFar, or aspect are zero or less, or if fovy is negative or greater than PI</exception>
        public static Matrix4 CreatePerspectiveFOV(float fovy, float aspect, float depthNear, float depthFar)
        {
            if (fovy <= 0.0f || fovy > MathF.PI)
            {
                throw new ArgumentOutOfRangeException(nameof(fovy));
            }
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(aspect, 0.0f);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(depthNear, 0.0f);
            ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(depthFar, 0.0f);

            Matrix4 result = new Matrix4();
            float top = depthNear * MathF.Tan(0.5f * fovy);
            float bottom = 0.0f - top;
            float left = bottom * aspect;
            float right = top * aspect;

            float x = 2.0f * depthNear / (right - left);
            float y = 2.0f * depthNear / (top - bottom);
            float x2 = (right + left) / (right - left);
            float y2 = (top + bottom) / (top - bottom);
            float z = (0.0f - (depthFar + depthNear)) / (depthFar - depthNear);
            float z2 = (0.0f - (2.0f * depthFar * depthNear)) / (depthFar - depthNear);

            result.Row0 = new Vec4(x, 0.0f, 0.0f, 0.0f);
            result.Row1 = new Vec4(0.0f, y, 0.0f, 0.0f);
            result.Row2 = new Vec4(x2, y2, z, -1.0f);
            result.Row3 = new Vec4(0.0f, 0.0f, z2, 0.0f);

            return result;
        }

        /// <summary>
        /// Construct an orthographic projection matrix (row major)
        /// </summary>
        /// <param name="width">The width of the projection volume</param>
        /// <param name="height">The height of the projection volume</param>
        /// <param name="depthNear">The near clip plane distance</param>
        /// <param name="depthFar">The far clip plane distance</param>
        /// <returns>An orthographic projection matrix</returns>
        public static Matrix4 CreateOrthographic(float width, float height, float depthNear, float depthFar)
        {
            return CreateOrthographicOffCenter(-width / 2.0f, width / 2.0f, -height / 2.0f, height / 2.0f, depthNear, depthFar);
        }

        /// <summary>
        /// Construct an off center orthographic projection matrix (row major)
        /// </summary>
        /// <param name="left">Coordinate value for the left extent</param>
        /// <param name="right">Coordinate value for the right extent</param>
        /// <param name="bottom">Coordinate value for the bottom extent</param>
        /// <param name="top">Coordinate value for the top extent</param>
        /// <param name="depthNear">The near clip</param>
        /// <param name="depthFar">The far clip</param>
        /// <returns>The orthographic projection matrix (row major)</returns>
        public static Matrix4 CreateOrthographicOffCenter(float left, float right, float bottom, float top, float depthNear, float depthFar)
        {
            Matrix4 result = Identity;

            var invertRightLeft = 1.0f / (right - left);
            var invertTopBottom = 1.0f / (top - bottom);
            var invertFarNear = 1.0f / (depthFar - depthNear);

            //scale by the view
            result.Row0.X = 2.0f * invertRightLeft;
            result.Row1.Y = 2.0f * invertTopBottom;
            result.Row2.Z = -2.0f * invertFarNear;

            //translate by view size
            result.Row3.X = -(right + left) * invertRightLeft;
            result.Row3.Y = -(top + bottom) * invertTopBottom;
            result.Row3.Z = -(depthFar + depthNear) * invertFarNear;

            return result;
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Convert a matrix into it's transpose
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

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

        /// <summary>
        /// Get a copy of the matrix's transpose.
        /// </summary>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix4 TransposedCopy()
        {
            return Transpose(this);
        }
        #endregion

        #region Invert
        /// <summary>
        /// Invert the given matrix.
        /// </summary>
        /// <param name="m">The matrix to invert.</param>
        /// <returns>The inverted matrix.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        public static Matrix4 Invert(Matrix4 m)
        {
            float n00 = m.M00, n01 = m.M01, n02 = m.M02, n03 = m.M03;
            float n10 = m.M10, n11 = m.M11, n12 = m.M12, n13 = m.M13;
            float n20 = m.M20, n21 = m.M21, n22 = m.M22, n23 = m.M23;
            float n30 = m.M30, n31 = m.M31, n32 = m.M32, n33 = m.M33;

            //2x2 sub matrix determinants
            float detM22_23_32_33 = Matrix2.CalcDeterminant(n22, n23, n32, n33);
            float detM21_23_31_33 = Matrix2.CalcDeterminant(n21, n23, n31, n33);
            float detM21_22_31_32 = Matrix2.CalcDeterminant(n21, n22, n31, n32);
            float detM20_23_30_33 = Matrix2.CalcDeterminant(n20, n23, n30, n33);
            float detM20_22_30_32 = Matrix2.CalcDeterminant(n20, n22, n30, n32);
            float detM20_21_30_31 = Matrix2.CalcDeterminant(n20, n21, n30, n31);

            //first row, determinants of 3x3 sub matrixes, first column result
            float detM00 = +(n11 * detM22_23_32_33 - n12 * detM21_23_31_33 + n13 * detM21_22_31_32);
            float detM01 = -(n10 * detM22_23_32_33 - n12 * detM20_23_30_33 + n13 * detM20_22_30_32);
            float detM02 = +(n10 * detM21_23_31_33 - n11 * detM20_23_30_33 + n13 * detM20_21_30_31);
            float detM03 = -(n10 * detM21_22_31_32 - n11 * detM20_22_30_32 + n12 * detM20_21_30_31);

            //calc the determ here since we have to do some work anyway
            float determ = n00 * detM00 + n01 * detM01 + n02 * detM02 + n03 * detM03;

            if (determ.IsZero())
                throw new InvalidOperationException("Matrix cannot be inverted as it is singular.");

            float invertDet = 1.0f / determ;

            //second row, second column result
            float detM10 = -(n01 * detM22_23_32_33 - n02 * detM21_23_31_33 + n03 * detM21_22_31_32);
            float detM11 = +(n00 * detM22_23_32_33 - n02 * detM20_23_30_33 + n03 * detM20_22_30_32);
            float detM12 = -(n00 * detM21_23_31_33 - n01 * detM20_23_30_33 + n03 * detM20_21_30_31);
            float detM13 = +(n00 * detM21_22_31_32 - n01 * detM20_22_30_32 + n02 * detM20_21_30_31);

            //2x2 sub matrix determinants
            float detM12_13_32_33 = Matrix2.CalcDeterminant(n12, n13, n32, n33);
            float detM11_13_31_33 = Matrix2.CalcDeterminant(n11, n13, n31, n33);
            float detM11_12_31_32 = Matrix2.CalcDeterminant(n11, n12, n31, n32);
            float detM10_13_30_33 = Matrix2.CalcDeterminant(n10, n13, n30, n33);
            float detM10_12_30_32 = Matrix2.CalcDeterminant(n10, n12, n30, n32);
            float detM10_11_30_31 = Matrix2.CalcDeterminant(n10, n11, n30, n31);

            //third row, third column result
            float detM20 = +(n01 * detM12_13_32_33 - n02 * detM11_13_31_33 + n03 * detM11_12_31_32);
            float detM21 = -(n00 * detM12_13_32_33 - n02 * detM10_13_30_33 + n03 * detM10_12_30_32);
            float detM22 = +(n00 * detM11_13_31_33 - n01 * detM10_13_30_33 + n03 * detM10_11_30_31);
            float detM23 = -(n00 * detM11_12_31_32 - n01 * detM10_12_30_32 + n02 * detM10_11_30_31);

            //2x2 sub matrix det
            float detM12_13_22_23 = Matrix2.CalcDeterminant(n12, n13, n22, n23);
            float detM11_13_21_23 = Matrix2.CalcDeterminant(n11, n13, n21, n23);
            float detM11_12_21_22 = Matrix2.CalcDeterminant(n11, n12, n21, n22);
            float detM10_13_20_23 = Matrix2.CalcDeterminant(n10, n13, n20, n23);
            float detM10_12_20_22 = Matrix2.CalcDeterminant(n10, n12, n20, n22);
            float detM10_11_20_21 = Matrix2.CalcDeterminant(n10, n11, n20, n21);

            //fourth row, fourth column result
            float detM30 = -(n01 * detM12_13_22_23 - n02 * detM11_13_21_23 + n03 * detM11_12_21_22);
            float detM31 = +(n00 * detM12_13_22_23 - n02 * detM10_13_20_23 + n03 * detM10_12_20_22);
            float detM32 = -(n00 * detM11_13_21_23 - n01 * detM10_13_20_23 + n03 * detM10_11_20_21);
            float detM33 = +(n00 * detM11_12_21_22 - n01 * detM10_12_20_22 + n02 * detM10_11_20_21);

            var r = new Matrix4
            {
                Row0 = new Vec4(detM00, detM10, detM20, detM30) * invertDet,
                Row1 = new Vec4(detM01, detM11, detM21, detM31) * invertDet,
                Row2 = new Vec4(detM02, detM12, detM22, detM32) * invertDet,
                Row3 = new Vec4(detM03, detM13, detM23, detM33) * invertDet
            };

            return r;
        }

        /// <summary>
        /// Invert the given matrix.
        /// </summary>
        /// <param name="m">The matrix to invert.</param>
        /// <param name="result">The inverted matrix.</param>
        /// <exception cref="InvalidOperationException">Thrown if the matrix is singular.</exception>
        public static void Invert(Matrix4 m, out Matrix4 result)
        {
            result = Invert(m);
        }

        /// <summary>
        /// Convert the matrix into it's inverse.
        /// </summary>
        public void Invert()
        {
            this = Invert(this);
        }
        #endregion

        #region Normalize
        /// <summary>
        /// Normalize the matrix by dividing by the determinant.
        /// </summary>
        public void Normalize()
        {
            var det = Determinant;
            if (det.IsZero())
            {
                Console.WriteLine("Matrix4: {0} has a determinant of 0. Thus normalize is undefined.", this);
                Logger.WriteToLog("Matrix4: {0} has a determinant of 0. Thus normalize is undefined.", this);
                //todo: write debug
            }
            else
            {
                Row0 /= det;
                Row1 /= det;
                Row2 /= det;
                Row3 /= det;
            }
        }

        /// <summary>
        /// Create a normalized copy of the matrix
        /// </summary>
        /// <returns>A copy of the matrix that has been normalized, or the original matrix
        /// if it is undefined.</returns>
        public readonly Matrix4 NormalizedCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }
        #endregion

        #region Swizzle
        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="mat">The matrix to swizzle.</param>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <param name="row3Row">The index of the row to be moved to row 3.</param>
        /// <returns>The resulting matrix.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if any of the indexes are out of range.</exception>
        public static Matrix4 Swizzle(Matrix4 mat, int row0Row, int row1Row, int row2Row, int row3Row)
        {
            if (row0Row < 0 || row0Row > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix4.", row0Row));
            if (row1Row < 0 || row1Row > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix4.", row1Row));
            if (row2Row < 0 || row2Row > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix4.", row2Row));
            if (row3Row < 0 || row3Row > 3)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix4.", row3Row));

            var result = new Matrix4
            {
                Row0 = mat[row0Row],
                Row1 = mat[row1Row],
                Row2 = mat[row2Row],
                Row3 = mat[row3Row]
            };

            return result;
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="mat">The matrix to swizzle.</param>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <param name="row3Row">The index of the row to be moved to row 3.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Swizzle(Matrix4 mat, int row0Row, int row1Row, int row2Row, int row3Row, out Matrix4 result)
        {
            result = Swizzle(mat, row0Row, row1Row, row2Row, row3Row);
        }

        /// <summary>
        /// Create a swizzled copy of the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <param name="row3Row">The index of the row to be moved to row 3.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix4 SwizzleCopy(int row0Row, int row1Row, int row2Row, int row3Row)
        {
            return Swizzle(this, row0Row, row1Row, row2Row, row3Row);
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <param name="row3Row">The index of the row to be moved to row 3.</param>
        public void Swizzle(int row0Row, int row1Row, int row2Row, int row3Row)
        {
            this = Swizzle(this, row0Row, row1Row, row2Row, row3Row);
        }
        #endregion

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

        /// <summary>
        /// Returns the translation component of the matrix.
        /// </summary>
        /// <returns>The translation.</returns>
        public readonly Vec3 GetTranslation()
        {
            return Row3.Xyz;
        }

        /// <summary>
        /// Create a matrix with translation values.
        /// </summary>
        /// <param name="x">The X translation.</param>
        /// <param name="y">The Y translation.</param>
        /// <param name="z">The Z translation.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 CreateTranslation(float x, float y, float z)
        {
            var result = Identity;

            result.Row3 = new Vec4(x, y, z, 1.0f);

            return result;
        }

        /// <summary>
        /// Create a matrix with translation values.
        /// </summary>
        /// <param name="x">The X translation.</param>
        /// <param name="y">The Y translation.</param>
        /// <param name="z">The Z translation.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateTranslation(float x, float y, float z, out Matrix4 result)
        {
            result = CreateTranslation(x, y, z);
        }

        /// <summary>
        /// Create a matrix with translation values.
        /// </summary>
        /// <param name="t">The translation as a vector.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 CreateTranslation(Vec3 t)
        {
            return CreateTranslation(t.X, t.Y, t.Z);
        }

        /// <summary>
        /// Create a matrix with translation values.
        /// </summary>
        /// <param name="t">The translation as a vector.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateTranslation(Vec3 t, out Matrix4 result)
        {
            result = CreateTranslation(t.X, t.Y, t.Z);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Create a copy of the matrix without any scaling.
        /// </summary>
        /// <returns>The matrix without scaling.</returns>
        public readonly Matrix4 ClearScale()
        {
            var c = this;
            c.Row0.Xyz = c.Row0.Xyz.NormalizedCopy();
            c.Row1.Xyz = c.Row1.Xyz.NormalizedCopy();
            c.Row2.Xyz = c.Row2.Xyz.NormalizedCopy();
            return c;
        }

        /// <summary>
        /// Get the scale components of the matrix.
        /// </summary>
        /// <returns>The 3D vector representing the scale components.</returns>
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
        /// <summary>
        /// Clear the rotation from a copy of the matrix.
        /// </summary>
        /// <returns>The copy without rotation.</returns>
        public readonly Matrix4 ClearRotation()
        {
            var c = this;
            c.Row0.Xyz = new Vec3(Row0.Xyz.Length, 0.0f, 0.0f);
            c.Row1.Xyz = new Vec3(0.0f, Row1.Xyz.Length, 0.0f);
            c.Row2.Xyz = new Vec3(0.0f, 0.0f, Row2.Xyz.Length);
            return c;
        }

        /// <summary>
        /// Create a matrix for rotation around the X axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 CreateRotationX(float angle)//todo: row/column major methods
        {
            var r = Identity;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            r.M11 = cos;
            r.M12 = sin;
            r.M21 = -sin;
            r.M22 = cos;

            return r;
        }

        /// <summary>
        /// Create a matrix for rotation around the X axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <param name="result">The resulting matrix</param>
        public static void CreateRotationX(float angle, out Matrix4 result)
        {
            result = CreateRotationX(angle);
        }

        /// <summary>
        /// Create a matrix for rotation around the Y axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 CreateRotationY(float angle)
        {
            var r = Identity;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            r.M00 = cos;
            r.M20 = -sin;
            r.M02 = sin;
            r.M22 = cos;

            return r;
        }

        /// <summary>
        /// Create a matrix for rotation around the Y axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <param name="result">The resulting matrix</param>
        public static void CreateRotationY(float angle, out Matrix4 result)
        {
            result = CreateRotationY(angle);
        }

        /// <summary>
        /// Create a matrix for rotation around the Z axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix4 CreateRotationZ(float angle)
        {
            var r = Identity;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);

            r.M00 = cos;
            r.M01 = sin;
            r.M10 = -sin;
            r.M11 = cos;

            return r;
        }

        /// <summary>
        /// Create a matrix for rotation around the Z axis (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by</param>
        /// <param name="result">The resulting matrix</param>
        public static void CreateRotationZ(float angle, out Matrix4 result)
        {
            result = CreateRotationZ(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 CreateRotationOnAxis(Vec3 axis, float angle)
        {
            Matrix3 rotMat = Matrix3.CreateRotationOnAxis(axis, angle);

            return new Matrix4(rotMat);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationOnAxis(Vec3 axis, float angle, out Matrix4 result)
        {
            result = CreateRotationOnAxis(axis, angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 CreateRotFromQuaternion(Quaternion quat)
        {
            Matrix3 rotMat = Matrix3.CreateRotFromQuaternion(quat);

            return new Matrix4(rotMat);
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotFromQuaternion(Quaternion quat, out Matrix4 result)
        {
            result = CreateRotFromQuaternion(quat);
        }

        /// <summary>
        /// Returns the rotation component of the matrix.
        /// </summary>
        /// <param name="preNormalized">Set true if the rows of the matrix are normalized, false otherwise.</param>
        /// <returns>The rotation as a Quaternion.</returns>
        public readonly Quaternion GetRotation(bool preNormalized = false)
        {
            //working copy of rows
            var r0 = Row0.Xyz;
            var r1 = Row1.Xyz;
            var r2 = Row2.Xyz;

            //if the rows are not normalized do so
            if (!preNormalized)
            {
                r0.Normalize();
                r1.Normalize();
                r2.Normalize();
            }

            var result = new Quaternion();

            float trace = Trace;

            if (trace > 0)
            {
                float sqrt = MathF.Sqrt(trace);

                result.W = sqrt;
                sqrt = 1.0f / (4.0f * sqrt);
                result.X = (r1.Z - r2.Y) * sqrt;
                result.Y = (r2.X - r0.Z) * sqrt;
                result.Z = (r0.Y - r1.X) * sqrt;
            }
            else if (r0.X > r1.Y && r0.X > r2.Z)//if r0.X greater than both other diagonal components 
            {
                float sqrt = 2.0f * MathF.Sqrt(1.0f + r0.X - r1.Y - r2.Z);

                result.X = 0.25f * sqrt;
                sqrt = 1.0f / sqrt;
                result.W = (r2.Y - r1.Z) * sqrt;
                result.Y = (r1.X + r0.Y) * sqrt;
                result.Z = (r2.X + r0.Y) * sqrt;
            }
            else if (r1.Y > r2.Z)//if Y of the diagonal is greater than the Z component
            {
                float sqrt = 2.0f * MathF.Sqrt(1.0f + r1.Y - r0.X - r2.Z);

                result.Y = 0.25f * sqrt;
                sqrt = 1.0f / sqrt;
                result.W = (r2.X - r0.Z) * sqrt;
                result.X = (r1.X + r0.Y) * sqrt;
                result.Z = (r2.Y + r1.Z) * sqrt;
            }
            else
            {
                float sqrt = 2.0f * MathF.Sqrt(1.0f + r2.Z - r0.X - r1.Y);

                result.Z = 0.25f * sqrt;
                sqrt = 1.0f / sqrt;
                result.W = (r1.X - r0.Y) * sqrt;
                result.X = (r2.X + r0.Z) * sqrt;
                result.Y = (r2.Y + r1.Z) * sqrt;
            }

            result.Normalize();
            return result;
        }
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

        /// <inheritdoc/>
        public readonly Matrix4 Add(Matrix4 rhs)
        {
            return Add(this, rhs);
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

        #region Subtract
        /// <summary>
        /// Subtract one matrix from another.
        /// </summary>
        /// <param name="lhs">The matrix to subtract from.</param>
        /// <param name="rhs">The matrix to subtract.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 Subtract(Matrix4 lhs, Matrix4 rhs)
        {
            var r = new Matrix4
            {
                Row0 = lhs.Row0 - rhs.Row0,
                Row1 = lhs.Row1 - rhs.Row1,
                Row2 = lhs.Row2 - rhs.Row2,
                Row3 = lhs.Row3 - rhs.Row3
            };

            return r;
        }

        /// <summary>
        /// Subtract one matrix from another.
        /// </summary>
        /// <param name="lhs">The matrix to subtract from.</param>
        /// <param name="rhs">The matrix to subtract.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Subtract(Matrix4 lhs, Matrix4 rhs, out Matrix4 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix4 Subtract(Matrix4 rhs)
        {
            return Subtract(this, rhs);
        }

        /// <summary>
        /// Define the subtraction operator between two matrices.
        /// </summary>
        /// <param name="lhs">The left matrix operand.</param>
        /// <param name="rhs">The right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 operator -(Matrix4 lhs, Matrix4 rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply the matrix by a scalar value.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 Multiply(Matrix4 lhs, float rhs)
        {
            var r = new Matrix4
            {
                Row0 = lhs.Row0 * rhs,
                Row1 = lhs.Row1 * rhs,
                Row2 = lhs.Row2 * rhs,
                Row3 = lhs.Row3 * rhs
            };

            return r;
        }

        /// <summary>
        /// Multiply the matrix by a scalar value.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4 lhs, float rhs, out Matrix4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vec4 Multiply(Matrix4 lhs, Vec4 rhs)
        {
            var r = new Vec4
            {
                X = Vec4.Dot(lhs.Row0, rhs),
                Y = Vec4.Dot(lhs.Row1, rhs),
                Z = Vec4.Dot(lhs.Row2, rhs),
                W = Vec4.Dot(lhs.Row3, rhs)
            };

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="result"></param>
        public static void Multiply(Matrix4 lhs, Vec4 rhs, out Vec4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vec4 Multiply(Vec4 lhs, Matrix4 rhs)
        {
            var r = new Vec4
            {
                X = Vec4.Dot(lhs, rhs.Column0),
                Y = Vec4.Dot(lhs, rhs.Column1),
                Z = Vec4.Dot(lhs, rhs.Column2),
                W = Vec4.Dot(lhs, rhs.Column3)
            };

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="result"></param>
        public static void Multiply(Vec4 lhs, Matrix4 rhs, out Vec4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4x2 Multiply(Matrix4 lhs, Matrix4x2 rhs)
        {
            var r = new Matrix4x2
            {
                Row0 = new Vec2(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1)),
                Row1 = new Vec2(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1)),
                Row2 = new Vec2(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1)),
                Row3 = new Vec2(Vec4.Dot(lhs.Row3, rhs.Column0), Vec4.Dot(lhs.Row3, rhs.Column1))
            };

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="result"></param>
        public static void Multiply(Matrix4 lhs, Matrix4x2 rhs, out Matrix4x2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4x3 Multiply(Matrix4 lhs, Matrix4x3 rhs)
        {
            var r = new Matrix4x3
            {
                Row0 = new Vec3(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1), Vec4.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1), Vec4.Dot(lhs.Row1, rhs.Column2)),
                Row2 = new Vec3(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1), Vec4.Dot(lhs.Row2, rhs.Column2)),
                Row3 = new Vec3(Vec4.Dot(lhs.Row3, rhs.Column0), Vec4.Dot(lhs.Row3, rhs.Column1), Vec4.Dot(lhs.Row3, rhs.Column2))
            };

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <param name="result"></param>
        public static void Multiply(Matrix4 lhs, Matrix4x3 rhs, out Matrix4x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply two matrices together.
        /// </summary>
        /// <param name="lhs">First matrix.</param>
        /// <param name="rhs">Second matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 Multiply(Matrix4 lhs, Matrix4 rhs)
        {
            var r = new Matrix4
            {
                Row0 = new Vec4(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1), Vec4.Dot(lhs.Row0, rhs.Column2), Vec4.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1), Vec4.Dot(lhs.Row1, rhs.Column2), Vec4.Dot(lhs.Row1, rhs.Column3)),
                Row2 = new Vec4(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1), Vec4.Dot(lhs.Row2, rhs.Column2), Vec4.Dot(lhs.Row2, rhs.Column3)),
                Row3 = new Vec4(Vec4.Dot(lhs.Row3, rhs.Column0), Vec4.Dot(lhs.Row3, rhs.Column1), Vec4.Dot(lhs.Row3, rhs.Column2), Vec4.Dot(lhs.Row3, rhs.Column3))
            };

            return r;
        }
                
        /// <summary>
        /// Multiply two matrices together.
        /// </summary>
        /// <param name="lhs">First matrix.</param>
        /// <param name="rhs">Second matrix.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4 lhs, Matrix4 rhs, out Matrix4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x4.Multiply(Matrix2x4, Matrix4)"/>
        public static Matrix2x4 Multiply(Matrix2x4 lhs, Matrix4 rhs)
        {
            return Matrix2x4.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x4.Multiply(Matrix2x4, Matrix4, out Matrix2x4)"/>
        public static void Multiply(Matrix2x4 lhs, Matrix4 rhs, out Matrix2x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x4.Multiply(Matrix3x4, Matrix4)"/>
        public static Matrix3x4 Multiply(Matrix3x4 lhs, Matrix4 rhs)
        {
            return Matrix3x4.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x4.Multiply(Matrix3x4, Matrix4, out Matrix3x4)"/>
        public static void Multiply(Matrix3x4 lhs, Matrix4 rhs, out Matrix3x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /*======================================================
         Multiply operators
         =======================================================*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4 operator *(Matrix4 lhs, float rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4 operator *(float lhs, Matrix4 rhs)
        {
            return Multiply(rhs, lhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vec4 operator *(Matrix4 lhs, Vec4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vec4 operator *(Vec4 lhs, Matrix4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4x2 operator *(Matrix4 lhs, Matrix4x2 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Matrix4x3 operator *(Matrix4 lhs, Matrix4x3 rhs)
        {
            return Multiply(lhs, rhs);
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

        /* Implemented in Matrix2x4
        public static Matrix2x4 operator *(Matrix2x4 lhs, Matrix4 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /* Implemented in Matrix3x4
        public static Matrix3x4 operator *(Matrix3x4 lhs, Matrix4 rhs)
        {
            return Multiply(lhs, rhs);
        }*/
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix4 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2 && Row3 == other.Row3;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix4 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance) && Row2.Equals(other.Row2, tolerance) && Row3.Equals(other.Row3, tolerance);
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
            var r3 = Row3.ToString(format, formatProvider);

            return string.Format("{0}\n\t{1}\n\t{2}\n\t{3}", r0, r1, r2, r3);
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
