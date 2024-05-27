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
    /// Two by three matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2x3 : IEquatable<Matrix2x3>, IFormattable, IMatrix<Matrix2x3, Vec3, Vec2, Vec2, Matrix3x2>
    {
        public Vec3 Row0;
        public Vec3 Row1;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix2x3 Zero => new Matrix2x3(Vec3.Zero, Vec3.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix.
        /// </summary>
        public Vec2 Column0
        {
            readonly get => new(Row0.X, Row1.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec2 Column1
        {
            readonly get => new(Row0.Y, Row1.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec2 Column2
        {
            readonly get => new(Row0.Z, Row1.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
            }
        }
        #endregion

        /// <inheritdoc/>
        public readonly Vec2[] GetColumns()
        {
            return [Column0, Column1, Column2];
        }

        /// <inheritdoc/>
        public readonly Vec3[] GetRows()
        {
            return [Row0, Row1];
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
        #endregion

        /// <summary>
        /// The matrix's main diagonal.
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
        /// The matrix's trace.
        /// </summary>
        public readonly float Trace => Row0.X + Row1.Y;

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
                if (column < 0 || column > 2)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix2x3", column));

                switch (row)
                {
                    case 0:
                        return Row0[column];
                    case 1:
                        return Row1[column];
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2x3", row));
                }
            }
            set
            {
                if (column < 0 || column > 2)
                    throw new IndexOutOfRangeException(string.Format("Column index: {0} is out of range for Matrix2x3", column));

                switch (row)
                {
                    case 0:
                        Row0[column] = value;
                        break;
                    case 1:
                        Row1[column] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2x3", row));
                }
            }
        }

        /// <summary>
        /// Array type accessor fro the rows of the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>The row at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">Throw if the index values are out of range.</exception>
        public Vec3 this[int row]
        {
            readonly get
            {
                switch (row)
                {
                    case 0:
                        return Row0;
                    case 1:
                        return Row1;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2x3.", row));
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
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2x3.", row));
                }
            }
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec3s.
        /// </summary>
        /// <param name="r0">Row 0.</param>
        /// <param name="r1">Row 1.</param>
        public Matrix2x3(Vec3 r0, Vec3 r1)
        {
            Row0 = r0;
            Row1 = r1;
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, column 0.</param>
        /// <param name="m01">Row 0, column 1.</param>
        /// <param name="m02">Row 0, column 2.</param>
        /// <param name="m10">Row 1, column 0.</param>
        /// <param name="m11">Row 1, column 1.</param>
        /// <param name="m12">Row 1, column 2.</param>
        public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12)
        {
            Row0 = new(m00, m01, m02);
            Row1 = new(m10, m11, m12);
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Transpose the given matrix.
        /// </summary>
        /// <param name="m">The matrix to be transposed.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x2 Transpose(Matrix2x3 m)
        {
            return new Matrix3x2(m.Column0, m.Column1, m.Column2);
        }

        /// <summary>
        /// Transpose the given matrix.
        /// </summary>
        /// <param name="m">The matrix to be transpose.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Transpose(Matrix2x3 m, out Matrix3x2 result)
        {
            result = Transpose(m);
        }

        /// <summary>
        /// Get a transposed copy of the matrix.
        /// </summary>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix3x2 TransposedCopy()
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
        /// <returns>The resulting matrix.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if any of the indexes are out of range.</exception>
        public static Matrix2x3 Swizzle(Matrix2x3 mat, int row0Row, int row1Row)
        {
            if (row0Row < 0 || row0Row > 1)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix2x3.", row0Row));
            if (row1Row < 0 || row1Row > 1)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix2x3.", row1Row));

            return new Matrix2x3(mat[row0Row], mat[row1Row]);
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="mat">The matrix to swizzle.</param>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Swizzle(Matrix2x3 mat, int row0Row, int row1Row, out Matrix2x3 result)
        {
            result = Swizzle(mat, row0Row, row1Row);
        }

        /// <summary>
        /// Create a Swizzled copy of the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix2x3 SwizzleCopy(int row0Row, int row1Row)
        {
            return Swizzle(this, row0Row, row1Row);
        }

        /// <summary>
        /// Swizzle the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        public void Swizzle(int row0Row, int row1Row)
        {
            this = Swizzle(this, row0Row, row1Row);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="scale">The scale factor to be used in both X and Y dimensions.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 CreateScale(float scale)
        {
            return CreateScale(scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="scale">The scale factor to be used in both X and Y dimensions.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(float scale, out Matrix2x3 result)
        {
            result = CreateScale(scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="x">The X dimension scale factor.</param>
        /// <param name="y">The Y dimension scale factor.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 CreateScale(float x, float y)
        {
            return new Matrix2x3(x, 0.0f, 0.0f, 0.0f, y, 0.0f);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="x">The X dimension scale factor.</param>
        /// <param name="y">The Y dimension scale factor.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(float x, float y, out Matrix2x3 result)
        {
            result = CreateScale(x, y);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="v">The scale factors for each dimension.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 CreateScale(Vec2 v)
        {
            return CreateScale(v.X, v.Y);
        }

        /// <summary>
        /// Create a matrix with scale values.
        /// </summary>
        /// <param name="v">The scale factors for each dimension.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateScale(Vec2 v, out Matrix2x3 result)
        {
            result = CreateScale(v.X, v.Y);
        }
        #endregion

        #region Rotate
        /// <summary>
        /// Create a matrix to represent rotation (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 CreateRotation(float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            return new Matrix2x3(cos, sin, 0.0f, -sin, cos, 0.0f);
        }

        /// <summary>
        /// Create a matrix to represent rotation (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotation(float angle, out Matrix2x3 result)
        {
            result = CreateRotation(angle);
        }
        #endregion

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 Add(Matrix2x3 lhs, Matrix2x3 rhs)
        {
            return new Matrix2x3(lhs.Row0 + rhs.Row0, lhs.Row1 + rhs.Row1);
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Add(Matrix2x3 lhs, Matrix2x3 rhs, out Matrix2x3 result)
        {
            result = Add(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix2x3 Add(Matrix2x3 rhs)
        {
            return Add(this, rhs);
        }

        /// <summary>
        /// Addition operator between two Matrix2x3s
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator +(Matrix2x3 lhs, Matrix2x3 rhs)
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
        public static Matrix2x3 Subtract(Matrix2x3 lhs, Matrix2x3 rhs) 
        {
            return new Matrix2x3(lhs.Row0 - rhs.Row0, lhs.Row1 - rhs.Row1);
        }

        /// <summary>
        /// Subtract one matrix from another.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Subtract(Matrix2x3 lhs, Matrix2x3 rhs, out Matrix2x3 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <summary>
        /// Subtraction operator between two Matrix2x3s
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator -(Matrix2x3 lhs, Matrix2x3 rhs)
        {
            return Subtract(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix2x3 Subtract(Matrix2x3 rhs)
        {
            return Subtract(this, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 Multiply(Matrix2x3 lhs, float rhs)
        {
            return new Matrix2x3(lhs.Row0 * rhs, lhs.Row1 * rhs);
        }

        /// <summary>
        /// Multiply a matrix by a scalar.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2x3 lhs, float rhs, out Matrix2x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix2 by a Matrix2x3
        /// </summary>
        /// <param name="lhs">The Matrix2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 Multiply(Matrix2 lhs, Matrix2x3 rhs)
        {
            return new Matrix2x3
            {
                Row0 = new Vec3(Vec2.Dot(lhs.Row0, rhs.Column0), Vec2.Dot(lhs.Row0, rhs.Column1), Vec2.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec2.Dot(lhs.Row1, rhs.Column0), Vec2.Dot(lhs.Row1, rhs.Column1), Vec2.Dot(lhs.Row1, rhs.Column2))
            };
        }

        /// <summary>
        /// Multiply a Matrix2 by a Matrix2x3
        /// </summary>
        /// <param name="lhs">The Matrix2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2 lhs, Matrix2x3 rhs, out Matrix2x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3x2.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x2.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2 Multiply(Matrix2x3 lhs, Matrix3x2 rhs)
        {
            return new Matrix2
            {
                Row0 = new Vec2(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1)),
                Row1 = new Vec2(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1))
            };
        }

        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3x2.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x2.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2x3 lhs, Matrix3x2 rhs, out Matrix2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3x2, Matrix2x3)"/>
        public static Matrix3 Multiply(Matrix3x2 lhs, Matrix2x3 rhs)
        {
            return Matrix3x2.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3x2, Matrix2x3, out Matrix3)"/>
        public static void Multiply(Matrix3x2 lhs, Matrix2x3 rhs, out Matrix3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 Multiply(Matrix2x3 lhs, Matrix3 rhs)
        {
            return new Matrix2x3
            {
                Row0 = new Vec3(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2))
            };
        }

        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2x3 lhs, Matrix3 rhs, out Matrix2x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3x4
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x4 Multiply(Matrix2x3 lhs, Matrix3x4 rhs)
        {
            return new Matrix2x4
            {
                Row0 = new Vec4(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2), Vec3.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2), Vec3.Dot(lhs.Row1, rhs.Column3))
            };
        }
        
        /// <summary>
        /// Multiply a Matrix2x3 by a Matrix3x4
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2x3 lhs, Matrix3x4 rhs, out Matrix2x4 result)
        {
            result = Multiply(lhs, rhs);
        }
        
        /// <summary>
        /// Multiply a Matrix4x2 by a Matrix2x3.
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x3 Multiply(Matrix4x2 lhs, Matrix2x3 rhs)
        {
            return new Matrix4x3
            {
                Row0 = new Vec3(Vec2.Dot(lhs.Row0, rhs.Column0), Vec2.Dot(lhs.Row0, rhs.Column1), Vec2.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec2.Dot(lhs.Row1, rhs.Column0), Vec2.Dot(lhs.Row1, rhs.Column1), Vec2.Dot(lhs.Row1, rhs.Column2)),
                Row2 = new Vec3(Vec2.Dot(lhs.Row2, rhs.Column0), Vec2.Dot(lhs.Row2, rhs.Column1), Vec2.Dot(lhs.Row2, rhs.Column2)),
                Row3 = new Vec3(Vec2.Dot(lhs.Row3, rhs.Column0), Vec2.Dot(lhs.Row3, rhs.Column1), Vec2.Dot(lhs.Row3, rhs.Column2))
            };
        }

        /// <summary>
        /// Multiply a Matrix4x2 by a Matrix2x3.
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4x2 lhs, Matrix2x3 rhs, out Matrix4x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /*======================================================
         Multiply operators
         =======================================================*/

        /// <summary>
        /// Multiplication operator between Matrix2x3 and scalar value.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator *(Matrix2x3 lhs, float rhs) 
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between scalar value and Matrix2x3.
        /// </summary>
        /// <param name="lhs">The scalar operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator *(float lhs, Matrix2x3 rhs)
        {
            return Multiply(rhs, lhs);
        }

        /* Implemented in Matrix2.
        public static Matrix2x3 operator *(Matrix2 lhs, Matrix2x3 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix2x3 and Matrix3x2.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x2 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2 operator *(Matrix2x3 lhs, Matrix3x2 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix2x3 and Matrix3.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator *(Matrix2x3 lhs, Matrix3 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix2x3 and Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix2x3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x4 operator *(Matrix2x3 lhs, Matrix3x4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /* Implemented in Matrix3x2
        public static Matrix3 operator *(Matrix3x2 lhs, Matrix2x3 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix4x2 and Matrix2x3.
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x3 operator *(Matrix4x2 lhs, Matrix2x3 rhs)
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix2x3 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix2x3 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Matrix2x3 other && Equals(other);
        }

        /// <summary>
        /// Equivalence operator definition.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if equal, false if not.</returns>
        public static bool operator ==(Matrix2x3 lhs, Matrix2x3 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition.
        /// </summary>
        /// <param name="lhs">Left matrix operand.</param>
        /// <param name="rhs">Right matrix operand.</param>
        /// <returns>True if not equal, false if equal.</returns>
        public static bool operator !=(Matrix2x3 lhs, Matrix2x3 rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode() 
        {
            return HashCode.Combine(Row0, Row1);
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

            return string.Format("{0}\n{1}", r0, r1);
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix2x3 to Matrix2x3.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator Matrix2x3(OpenTK.Mathematics.Matrix2x3 m)
        {
            return new Matrix2x3(m.Row0, m.Row1);
        }

        /// <summary>
        /// Handle conversion from Matrix2x3 to OpenTK's Matrix2x3.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        public static implicit operator OpenTK.Mathematics.Matrix2x3(Matrix2x3 m)
        {
            return new OpenTK.Mathematics.Matrix2x3(m.Row0, m.Row1);
        }
        #endregion
#endif
    }
}
