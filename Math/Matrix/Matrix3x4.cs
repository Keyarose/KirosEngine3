using KirosEngine3.Math.Data;
using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Three by four matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x4 : IEquatable<Matrix3x4>, IFormattable, IMatrix<Matrix3x4, Vec4, Vec3, Vec3, Matrix4x3>
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
        /// The zero matrix.
        /// </summary>
        public static Matrix3x4 Zero => new Matrix3x4(Vec4.Zero, Vec4.Zero, Vec4.Zero);

        #region Columns
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

        /// <summary>
        /// The third column of the matrix.
        /// </summary>
        public Vec3 Column2
        {
            readonly get => new Vec3(Row0.Z, Row1.Z, Row2.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
                Row2.Z = value.Z;
            }
        }

        /// <summary>
        /// The fourth column of the matrix.
        /// </summary>
        public Vec3 Column3
        {
            readonly get => new Vec3(Row0.W, Row1.W, Row2.W);
            set
            {
                Row0.W = value.X;
                Row1.W = value.Y;
                Row2.W = value.Z;
            }
        }
        #endregion

        /// <inheritdoc/>
        public readonly Vec3[] GetColumns()
        {
            return [Column0, Column1, Column2, Column3];
        }

        /// <inheritdoc/>
        public readonly Vec4[] GetRows()
        {
            return [Row0, Row1, Row2];
        }

        #region Cell Accessors
        /// <summary>
        /// Accessor for Row 0, column 0.
        /// </summary>
        public float M00
        {
            readonly get { return Row0.X; }
            set { Row0.X = value; }
        }

        /// <summary>
        /// Accessor for Row 0, column 1.
        /// </summary>
        public float M01
        {
            readonly get { return Row0.Y; }
            set { Row0.Y = value; }
        }

        /// <summary>
        /// Accessor for Row 0, column 2.
        /// </summary>
        public float M02
        {
            readonly get { return Row0.Z; }
            set { Row0.Z = value; }
        }

        /// <summary>
        /// Accessor for Row 0, column 3.
        /// </summary>
        public float M03
        {
            readonly get { return Row0.W; }
            set { Row0.W = value; }
        }

        /// <summary>
        /// Accessor for Row 1, column 0.
        /// </summary>
        public float M10
        {
            readonly get { return Row1.X; }
            set { Row1.X = value; }
        }

        /// <summary>
        /// Accessor for Row 1, column 1.
        /// </summary>
        public float M11
        {
            readonly get { return Row1.Y; }
            set { Row1.Y = value; }
        }

        /// <summary>
        /// Accessor for Row 1, column 2.
        /// </summary>
        public float M12
        {
            readonly get { return Row1.Z; }
            set { Row1.Z = value; }
        }

        /// <summary>
        /// Accessor for Row 1, column 3.
        /// </summary>
        public float M13
        {
            readonly get { return Row1.W; }
            set { Row1.W = value; }
        }

        /// <summary>
        /// Accessor for Row 2, column 0.
        /// </summary>
        public float M20
        {
            readonly get { return Row2.X; }
            set { Row2.X = value; }
        }

        /// <summary>
        /// Accessor for Row 2, column 1.
        /// </summary>
        public float M21
        {
            readonly get { return Row2.Y; }
            set { Row2.Y = value; }
        }

        /// <summary>
        /// Accessor for Row 2, column 2.
        /// </summary>
        public float M22
        {
            readonly get { return Row2.Z; }
            set { Row2.Z = value; }
        }

        /// <summary>
        /// Accessor for Row 2, column 3.
        /// </summary>
        public float M23
        {
            readonly get { return Row2.W; }
            set { Row2.W = value; }
        }
        #endregion

        /// <summary>
        /// The matrix's main diagonal.
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
        /// The matrix's trace.
        /// </summary>
        public readonly float Trace => Row0.X + Row1.Y + Row2.Z;

        /// <summary>
        /// Array type accessor for the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns>The value at the given indexes.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index values are out of range.</exception>
        public float this[int row, int column]
        {
            readonly get
            {
                if (column < 0 || column > 3)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix3x4", column));

                switch (row)
                {
                    case 0:
                        return Row0[column];
                    case 1:
                        return Row1[column];
                    case 2:
                        return Row2[column];
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3x4", row));
                }
            }
            set
            {
                if (column < 0 || column > 3)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix3x4", column));

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
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3x4", row));
                }
            }
        }

        /// <summary>
        /// Array type accessor for the rows of the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>The row at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">Throw if the index values are out of range.</exception>
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
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3x4.", row));
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
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3x4.", row));
                }
            }
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec4s.
        /// </summary>
        /// <param name="row0">Row 0.</param>
        /// <param name="row1">Row 1.</param>
        /// <param name="row2">Row 2.</param>
        public Matrix3x4(Vec4 row0, Vec4 row1, Vec4 row2)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
        }

        /// <summary>
        /// Construct a Matrix3x4 from a Matrix3.
        /// </summary>
        /// <param name="mat3">The Matrix3 to use as a basis.</param>
        /// <param name="w0">The W component for row 0, default 0.</param>
        /// <param name="w1">The W component for row 1, default 0.</param>
        /// <param name="w2">The W component for row 2, default 0.</param>
        public Matrix3x4(Matrix3 mat3, float w0 = 0, float w1 = 0, float w2 = 0)
        {
            Row0 = new Vec4(mat3.Row0, w0);
            Row1 = new Vec4(mat3.Row1, w1);
            Row2 = new Vec4(mat3.Row2, w2);
        }

        /// <summary>
        /// Construct a Matrix3x4 from a Matrix3.
        /// </summary>
        /// <param name="mat3">The Matrix3 to use as a basis.</param>
        /// <param name="wVals">The W component values.</param>
        public Matrix3x4(Matrix3 mat3, Vec3 wVals) : this(mat3, wVals.X, wVals.Y, wVals.Z)
        {
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, Column 0.</param>
        /// <param name="m01">Row 0, Column 1.</param>
        /// <param name="m02">Row 0, Column 2.</param>
        /// <param name="m03">Row 0, Column 3.</param>
        /// <param name="m10">Row 1, Column 0.</param>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <param name="m12">Row 1, Column 2.</param>
        /// <param name="m13">Row 1, Column 3.</param>
        /// <param name="m20">Row 2, Column 0.</param>
        /// <param name="m21">Row 2, Column 1.</param>
        /// <param name="m22">Row 2, Column 2.</param>
        /// <param name="m23">Row 2, Column 3.</param>
        public Matrix3x4(float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Transpose the given matrix.
        /// </summary>
        /// <param name="m">The matrix to be transpose.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x3 Transpose(Matrix3x4 m)
        {
            return new Matrix4x3(m.Column0, m.Column1, m.Column2, m.Column3);
        }

        /// <summary>
        /// Transpose the given matrix.
        /// </summary>
        /// <param name="m">The matrix to be transpose.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Transpose(Matrix3x4 m, out Matrix4x3 result)
        {
            result = Transpose(m);
        }

        /// <summary>
        /// Create a transposed copy of the matrix.
        /// </summary>
        /// <returns>The transposed copy.</returns>
        public readonly Matrix4x3 TransposedCopy()
        {
            return Transpose(this);
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
        /// <returns>The resulting matrix.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if any of the indexes are out of range.</exception>
        public static Matrix3x4 Swizzle(Matrix3x4 mat, int row0Row, int row1Row, int row2Row)
        {
            if (row0Row < 0 || row0Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3x4.", row0Row));
            if (row1Row < 0 || row1Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3x4.", row1Row));
            if (row2Row < 0 || row2Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3x4.", row2Row));

            return new Matrix3x4(mat[row0Row], mat[row1Row], mat[row2Row]);
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="mat">The matrix to swizzle.</param>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Swizzle(Matrix3x4 mat, int row0Row, int row1Row, int row2Row, out Matrix3x4 result)
        {
            result = Swizzle(mat, row0Row, row1Row, row2Row);
        }

        /// <summary>
        /// Create a Swizzled copy of the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix3x4 SwizzleCopy(int row0Row, int row1Row, int row2Row)
        {
            return Swizzle(this, row0Row, row1Row, row2Row);
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        public void Swizzle(int row0Row, int row1Row, int row2Row)
        {
            this = Swizzle(this, row0Row, row1Row, row2Row);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="scale">The scale factor to be used in the X, Y and Z dimensions.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateScale(float scale)
        {
            return CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="scale">The scale factor to be used in the X, Y and Z dimensions.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(float scale, out Matrix3x4 result)
        {
            result = CreateScale(scale, scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="x">The X dimension scale factor.</param>
        /// <param name="y">The Y dimension scale factor.</param>
        /// <param name="z">The Z dimension scale factor.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateScale(float x, float y, float z)
        {
            return new Matrix3x4(x, 0.0f, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 0.0f, z, 0.0f);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="x">The X dimension scale factor.</param>
        /// <param name="y">The Y dimension scale factor.</param>
        /// <param name="z">The Z dimension scale factor.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(float x, float y, float z, out Matrix3x4 result)
        {
            result = CreateScale(x, y, z);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="v">The scale factors for each dimension.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateScale(Vec3 v)
        {
            return CreateScale(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="v">The scale factors for each dimension.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(Vec3 v, out Matrix3x4 result)
        {
            result = CreateScale(v.X, v.Y, v.Z);
        }
        #endregion

        #region Rotate
        /// <summary>
        /// Create a matrix to represent rotation around the x axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateRotationX(float angle)//Todo: row/column major methods
        {
            Matrix3 rotMat = Matrix3.CreateRotationX(angle);

            var result = new Matrix3x4(rotMat);

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the x axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationX(float angle, out Matrix3x4 result)
        {
            result = CreateRotationX(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Y axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateRotationY(float angle)
        {
            Matrix3 rotMat = Matrix3.CreateRotationY(angle);

            var result = new Matrix3x4(rotMat);

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Y axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationY(float angle, out Matrix3x4 result)
        {
            result = CreateRotationY(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Z axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateRotationZ(float angle)
        {
            Matrix3 rotMat = Matrix3.CreateRotationZ(angle);

            var result = new Matrix3x4(rotMat);

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Z axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationZ(float angle, out Matrix3x4 result)
        {
            result = CreateRotationZ(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateRotationOnAxis(Vec3 axis, float angle)
        {
            Matrix3 rotMat = Matrix3.CreateRotationOnAxis(axis, angle);

            var result = new Matrix3x4(rotMat);

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationOnAxis(Vec3 axis, float angle, out Matrix3x4 result)
        {
            result = CreateRotationOnAxis(axis, angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 CreateRotFromQuaternion(Quaternion quat)
        {
            Matrix3 rotMat = Matrix3.CreateRotFromQuaternion(quat);

            var result = new Matrix3x4(rotMat);

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotFromQuaternion(Quaternion quat, out Matrix3x4 result)
        {
            result = CreateRotFromQuaternion(quat);
        }
        #endregion

        #region Add
        /// <summary>
        /// Add two matrices together.
        /// </summary>
        /// <param name="lhs">First matrix to add.</param>
        /// <param name="rhs">Second matrix to add.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 Add(Matrix3x4 lhs, Matrix3x4 rhs)
        {
            return new Matrix3x4(lhs.Row0 + rhs.Row0, lhs.Row1 + rhs.Row1, lhs.Row2 + rhs.Row2);
        }

        /// <summary>
        /// Add two matrices together.
        /// </summary>
        /// <param name="lhs">First matrix to add.</param>
        /// <param name="rhs">Second matrix to add.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Add(Matrix3x4 lhs, Matrix3x4 rhs, out Matrix3x4 result)
        {
            result = Add(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix3x4 Add(Matrix3x4 rhs)
        {
            return Add(this, rhs);
        }

        /// <summary>
        /// Addition operator between two Matrix2x4s.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator +(Matrix3x4 lhs, Matrix3x4 rhs)
        {
            return Add(lhs, rhs);
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract one matrix from another.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 Subtract(Matrix3x4 lhs, Matrix3x4 rhs)
        {
            return new Matrix3x4(lhs.Row0 - rhs.Row0, lhs.Row1 - rhs.Row1, lhs.Row2 - rhs.Row2);
        }

        /// <summary>
        /// Subtract one matrix from another.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Subtract(Matrix3x4 lhs, Matrix3x4 rhs, out Matrix3x4 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix3x4 Subtract(Matrix3x4 rhs)
        {
            return Subtract(this, rhs);
        }

        /// <summary>
        /// Subtraction operator between two Matrix3x4s
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator -(Matrix3x4 lhs, Matrix3x4 rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 Multiply(Matrix3x4 lhs, float rhs)
        {
            return new Matrix3x4(lhs.Row0 * rhs, lhs.Row1 * rhs, lhs.Row2 * rhs);
        }

        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3x4 lhs, float rhs, out Matrix3x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4x2
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x2 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 Multiply(Matrix3x4 lhs, Matrix4x2 rhs)
        {
            return new Matrix3x2
            {
                Row0 = new Vec2(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1)),
                Row1 = new Vec2(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1)),
                Row2 = new Vec2(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1))
            };
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4x2
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x2 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3x4 lhs, Matrix4x2 rhs, out Matrix3x2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4x3
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 Multiply(Matrix3x4 lhs, Matrix4x3 rhs)
        {
            return new Matrix3
            {
                Row0 = new Vec3(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1), Vec4.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1), Vec4.Dot(lhs.Row1, rhs.Column2)),
                Row2 = new Vec3(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1), Vec4.Dot(lhs.Row2, rhs.Column2))
            };
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4x3
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x3 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3x4 lhs, Matrix4x3 rhs, out Matrix3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4
        /// </summary>
        /// <param name="lhs">The Matrix2x4 operand.</param>
        /// <param name="rhs">The Matrix4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 Multiply(Matrix3x4 lhs, Matrix4 rhs)
        {
            return new Matrix3x4
            {
                Row0 = new Vec4(Vec4.Dot(lhs.Row0, rhs.Column0), Vec4.Dot(lhs.Row0, rhs.Column1), Vec4.Dot(lhs.Row0, rhs.Column2), Vec4.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec4.Dot(lhs.Row1, rhs.Column0), Vec4.Dot(lhs.Row1, rhs.Column1), Vec4.Dot(lhs.Row1, rhs.Column2), Vec4.Dot(lhs.Row1, rhs.Column3)),
                Row2 = new Vec4(Vec4.Dot(lhs.Row2, rhs.Column0), Vec4.Dot(lhs.Row2, rhs.Column1), Vec4.Dot(lhs.Row2, rhs.Column2), Vec4.Dot(lhs.Row2, rhs.Column3))
            };
        }

        /// <summary>
        /// Multiply a Matrix3x4 by a Matrix4
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3x4 lhs, Matrix4 rhs, out Matrix3x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2x3, Matrix3x4)"/>
        public static Matrix2x4 Multiply(Matrix2x3 lhs, Matrix3x4 rhs)
        {
            return Matrix2x3.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2x3, Matrix3x4, out Matrix2x4)"/>
        public static void Multiply(Matrix2x3 lhs, Matrix3x4 rhs, out Matrix2x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3.Multiply(Matrix3, Matrix3x4)"/>
        public static Matrix3x4 Multiply(Matrix3 lhs, Matrix3x4 rhs)
        {
            return Matrix3.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3.Multiply(Matrix3, Matrix3x4, out Matrix3x4)"/>
        public static void Multiply(Matrix3 lhs, Matrix3x4 rhs, out Matrix3x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix4x3 by a Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 Multiply(Matrix4x3 lhs, Matrix3x4 rhs)
        {
            return new Matrix4
            {
                Row0 = new Vec4(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2), Vec3.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2), Vec3.Dot(lhs.Row1, rhs.Column3)),
                Row2 = new Vec4(Vec3.Dot(lhs.Row2, rhs.Column0), Vec3.Dot(lhs.Row2, rhs.Column1), Vec3.Dot(lhs.Row2, rhs.Column2), Vec3.Dot(lhs.Row2, rhs.Column3)),
                Row3 = new Vec4(Vec3.Dot(lhs.Row3, rhs.Column0), Vec3.Dot(lhs.Row3, rhs.Column1), Vec3.Dot(lhs.Row3, rhs.Column2), Vec3.Dot(lhs.Row3, rhs.Column3))
            };
        }

        /// <summary>
        /// Multiply a Matrix4x3 by a Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4x3 lhs, Matrix3x4 rhs, out Matrix4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /*======================================================
         Multiply operators
         =======================================================*/

        /// <summary>
        /// Multiplication operator between Matrix3x4 and scalar value.
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator *(Matrix3x4 lhs, float rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix3x4 and scalar value.
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator *(float lhs, Matrix3x4 rhs)
        {
            return Multiply(rhs, lhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix3x4 and Matrix4x2.
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x2 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 operator *(Matrix3x4 lhs, Matrix4x2 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix3x4 and Matrix4x3.
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 operator *(Matrix3x4 lhs, Matrix4x3 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix3x4 and Matrix4.
        /// </summary>
        /// <param name="lhs">The Matrix3x4 operand.</param>
        /// <param name="rhs">The Matrix4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator *(Matrix3x4 lhs, Matrix4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /* Implemented in Matrix2x3
        public static Matrix2x4 operator *(Matrix2x3 lhs, Matrix3x4 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /* Implemented in Matrix3
        public static Matrix3x4 operator *(Matrix3 lhs, Matrix3x4 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix4x3 and Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4 operator *(Matrix4x3 lhs, Matrix3x4 rhs)
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix3x4 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix3x4 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix3x4 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator ==(Matrix3x4 lhs, Matrix3x4 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if not equal, false if equal.</returns>
        public static bool operator !=(Matrix3x4 lhs, Matrix3x4 rhs)
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

        public string ToDrawString()
        {
            throw new NotImplementedException();
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix2x4 to Matrix2x4.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator Matrix3x4(OpenTK.Mathematics.Matrix3x4 m)
        {
            return new Matrix3x4(m.Row0, m.Row1, m.Row2);
        }

        /// <summary>
        /// Handle conversion from Matrix2x4 to OpenTK's Matrix2x4.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator OpenTK.Mathematics.Matrix3x4(Matrix3x4 m)
        {
            return new OpenTK.Mathematics.Matrix3x4(m.Row0, m.Row1, m.Row2);
        }
        #endregion
#endif
    }
}
