using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Two by two matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2 : IEquatable<Matrix2>, IFormattable, IMatrix<Matrix2, Vec2, Vec2, Vec2, Matrix2>
    {
        public Vec2 Row0;
        public Vec2 Row1;

        /// <summary>
        /// The Identity matrix
        /// </summary>
        public static Matrix2 Identity => new Matrix2(Vec2.UnitX, Vec2.UnitY);

        /// <summary>
        /// the zero matrix
        /// </summary>
        public static Matrix2 Zero => new Matrix2(Vec2.Zero, Vec2.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix
        /// </summary>
        public Vec2 Column0
        {
            readonly get => new Vec2(Row0.X, Row1.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
            }
        }

        /// <summary>
        /// The second column of the matrix
        /// </summary>
        public  Vec2 Column1
        {
            readonly get => new Vec2(Row0.Y, Row1.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
            }
        }
        #endregion

        /// <inheritdoc/>
        public readonly Vec2[] GetColumns()
        {
            return [Column0, Column1];
        }

        /// <inheritdoc/>
        public readonly Vec2[] GetRows()
        {
            return [Row0, Row1];
        }

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

        /// <summary>
        /// Calculate the determinant for a matrix with the given values.
        /// </summary>
        /// <param name="m00">Row 0, Column 0.</param>
        /// <param name="m01">Row 0, Column 1.</param>
        /// <param name="m10">Row 1, Column 0.</param>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <returns>The determinant.</returns>
        public static float CalcDeterminant(float m00, float m01, float m10, float m11)
        {
            return (m00 * m11) - (m01 * m10);
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
                Row1.Y = value.Y;
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
                if (column < 0 || column > 1)
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
                if (column < 0 || column > 1)
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
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2.", row));
                }
            }
        }

        /// <summary>
        /// Array type accessor for the rows of the matrix
        /// </summary>
        /// <param name="row">Row index</param>
        /// <returns>The row at the given index</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index value is outside the allowed range of 0,1</exception>
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
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2.", row));
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
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix2.", row));
                }
            }
        }

        #region Constructors
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
        #endregion

        #region ElementaryMatrices
        /// <summary>
        /// Produce an elementary matrix for the scalar multiplication row operation on the given row.
        /// </summary>
        /// <param name="row">The index of the row to multiply.</param>
        /// <param name="scalar">The scalar the row is to be multiplied by.</param>
        /// <returns>The 2D elementary matrix that performs the row operation.</returns>
        public static Matrix2 RowMultiplyElemMat(int row, float scalar)
        {
            switch (row) 
            {
                case 0:
                    return new Matrix2(scalar, 0.0f, 0.0f, 1.0f);
                case 1:
                    return new Matrix2(1.0f, 0.0f, 0.0f, scalar);
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2.", row));
            }
        }

        /// <summary>
        /// Produce an elementary matrix for row interchange operations
        /// </summary>
        /// <returns>The 2D elementary matrix that performs the row operation</returns>
        public static Matrix2 RowInterchangeElemMat()//doesn't follow the mat3 format there are only 2 rows
        {
            return new Matrix2(0.0f, 1.0f, 1.0f, 0.0f);
        }

        /// <summary>
        /// Produce an elementary matrix for adding one row to another.
        /// </summary>
        /// <param name="r1">The index of the row to add.</param>
        /// <param name="r2">The index of the row to add to.</param>
        /// <param name="scalar">The number of times to add the first row.</param>
        /// <returns>The 2D elementary matrix that performs the row operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the indexes are the same.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if the indexes are out of range.</exception>
        public static Matrix2 RowAddElemMat(int r1, int r2, float scalar)
        {
            if (r1 == r2)
                throw new InvalidOperationException(string.Format("Adding a row to itself is not a valid operation."));

            switch (r1)
            {
                case 0:
                    {
                        if (r2 == 1)//add row 0 to row 1
                        {
                            return new Matrix2(1.0f, 0.0f, scalar, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2.", r2));
                    }
                case 1:
                    {
                        if (r2 == 0)//add row 1 to row 0
                        {
                            return new Matrix2(1.0f, scalar, 0.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2.", r2));
                    }
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix2.", r1));
            }
        }
        #endregion

        #region RowOperations
        /// <summary>
        /// Perform the row interchange operation
        /// </summary>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix2 RowInterchange()
        {
            return RowInterchangeElemMat() * this;
        }

        /// <summary>
        /// Perform the row multiplication operation.
        /// </summary>
        /// <param name="row">The row to apply the operation on.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix2 RowMultiplication(int row, float scalar)
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
        public readonly Matrix2 RowAddition(int r1, int r2, float scalar)
        {
            return RowAddElemMat(r1, r2, scalar) * this;
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Convert the matrix into it's transpose
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
        }

        /// <summary>
        /// Get a transposed copy of the matrix
        /// </summary>
        /// <returns>The resulting transpose</returns>
        public readonly Matrix2 TransposedCopy()
        {
            return Transpose(this);
        }

        /// <summary>
        /// Get a copy of the transpose of a matrix
        /// </summary>
        /// <param name="m">The matrix to transpose</param>
        /// <returns>The transpose in a new instance</returns>
        public static Matrix2 Transpose(Matrix2 m)
        {
            return new Matrix2(m.Column0, m.Column1);
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
        #endregion

        #region Invert
        /// <summary>
        /// Convert the matrix into it's inverse
        /// </summary>
        public void Invert()
        {
            this = Invert(this);
        }

        /// <summary>
        /// Get an inverted copy of the matrix
        /// </summary>
        /// <returns></returns>
        public readonly Matrix2 InvertedCopy()
        {
            return Invert(this);
        }

        /// <summary>
        /// Get an inverted copy of the given matrix
        /// </summary>
        /// <param name="m">The matrix to invert</param>
        /// <returns>The resulting matrix</returns>
        /// <exception cref="InvalidOperationException">Thrown if the matrix's determinant is 0, thus singular</exception>
        public static Matrix2 Invert(Matrix2 m)
        {
            if (m.Determinant.IsZero())
            {
                throw new InvalidOperationException("Matrix cannot be inverted as it's singular.");
            }

            var invDet = 1f / m.Determinant;

            return new Matrix2(m.Row1.Y * invDet, -m.Row0.Y * invDet, -m.Row1.X * invDet, m.Row0.X * invDet);
        }

        /// <summary>
        /// Invert the given matrix
        /// </summary>
        /// <param name="m">The matrix to invert</param>
        /// <param name="result">The resulting matrix</param>
        /// /// <exception cref="InvalidOperationException">Thrown if the matrix's determinant is 0, thus singular</exception>
        public static void Invert(Matrix2 m, out Matrix2 result)
        {
            try
            {
                result = Invert(m);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
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
                Console.WriteLine("Matrix2: {0} has a determinant of 0. Thus normalize is undefined.", this);
                Logger.WriteToLog("Matrix2: {0} has a determinant of 0. Thus normalize is undefined.", this);
                //todo: write debug
            }
            else
            {
                Row0 /= det;
                Row1 /= det;
            }
        }

        /// <summary>
        /// Create a normalized copy of the matrix.
        /// </summary>
        /// <returns>A copy of the matrix that has been normalized, or the original matrix 
        /// if it is undefined.</returns>
        public readonly Matrix2 NormalizedCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }
        #endregion

        #region Swizzle
        /// <summary>
        /// Swizzle, switch the rows of the matrix
        /// </summary>
        /// <param name="mat">The matrix to swizzle</param>
        /// <param name="row0Row">The index of the row to be moved to row 0</param>
        /// <param name="row1Row">The index of the row to be moved to row 1</param>
        /// <returns>The resulting matrix</returns>
        /// /// <exception cref="IndexOutOfRangeException">Thrown if any of the indexes are out of range.</exception>
        public static Matrix2 Swizzle(Matrix2 mat, int row0Row, int row1Row)
        {
            if (row0Row < 0 || row0Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix2.", row0Row));
            if (row1Row < 0 || row1Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix2.", row1Row));

            Matrix2 result = new Matrix2
            {
                Row0 = mat[row0Row],
                Row1 = mat[row1Row]
            };

            return result;
        }

        /// <summary>
        /// Swizzle, switch the rows of the matrix
        /// </summary>
        /// <param name="mat">The matrix to swizzle</param>
        /// <param name="row0Row">The index of the row to be moved to row 0</param>
        /// <param name="row1Row">The index of the row to be moved to row 1</param>
        /// <param name="result">The resulting matrix</param>
        public static void Swizzle(Matrix2 mat, int row0Row, int row1Row, out Matrix2 result)
        {
            result = Swizzle(mat, row0Row, row1Row);
        }

        /// <summary>
        /// Create a swizzled copy of the matrix
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0</param>
        /// <param name="row1Row">The index of the row to be moved to row 1</param>
        /// <returns>The swizzled copy</returns>
        public readonly Matrix2 SwizzleCopy(int row0Row, int row1Row)
        {
            return Swizzle(this, row0Row, row1Row);
        }

        /// <summary>
        /// Swizzle the matrix
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0</param>
        /// <param name="row1Row">The index of the row to be moved to row 1</param>
        public void Swizzle(int row0Row, int row1Row)
        {
            this = Swizzle(this, row0Row, row1Row);
        }
        #endregion

        #region Scale
        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in both dimensions</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix2 CreateScale(float scale)
        {
            return CreateScale(scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="scale">The scale factor to be used in both dimensions</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(float scale, out Matrix2 result)
        {
            result = CreateScale(scale, scale);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix2 CreateScale(float x, float y)
        {
            return new Matrix2(x, 0.0f, 0.0f, y);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="x">The X dimension scale factor</param>
        /// <param name="y">The Y dimension scale factor</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(float x, float y, out Matrix2 result)
        {
            result = CreateScale(x, y);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="v">The scale factors for each dimension</param>
        /// <returns>The matrix that represents the scale factors</returns>
        public static Matrix2 CreateScale(Vec2 v)
        {
            return CreateScale(v.X, v.Y);
        }

        /// <summary>
        /// Create a matrix with scale values
        /// </summary>
        /// <param name="v">The scale factors for each dimension</param>
        /// <param name="result">The matrix that represents the scale factors</param>
        public static void CreateScale(Vec2 v, out Matrix2 result)
        {
            result = CreateScale(v.X, v.Y);
        }
        #endregion

        #region Rotate
        /// <summary>
        /// Create a matrix to represent the rotation (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 CreateRotation(float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            return new Matrix2(cos, sin, -sin, cos);
        }

        /// <summary>
        /// Create a matrix to represent the rotation (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians</param>
        /// <param name="result">The resulting matrix</param>
        public static void CreateRotation(float angle, out Matrix2 result)
        {
            result = CreateRotation(angle);
        }
        #endregion

        #region Add
        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">First matrix to add</param>
        /// <param name="rhs">Second matrix to add</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Add(Matrix2 lhs, Matrix2 rhs)
        {
            var r = new Matrix2
            {
                Row0 = lhs.Row0 + rhs.Row0,
                Row1 = lhs.Row1 + rhs.Row1,
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

        /// <inheritdoc/>
        public readonly Matrix2 Add(Matrix2 rhs)
        {
            return Add(this, rhs);
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

        #region Subtract
        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <param name="lhs">The matrix to subtract from</param>
        /// <param name="rhs">The matrix to subtract</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Subtract(Matrix2 lhs, Matrix2 rhs)
        {
            var r = new Matrix2
            {
                Row0 = lhs.Row0 - rhs.Row0,
                Row1 = lhs.Row1 - rhs.Row1
            };

            return r;
        }

        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <param name="lhs">The matrix to subtract from</param>
        /// <param name="rhs">The matrix to subtract</param>
        /// <param name="result">The resulting matrix</param>
        public static void Subtract(Matrix2 lhs, Matrix2 rhs, out Matrix2 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix2 Subtract(Matrix2 rhs)
        {
            return Subtract(this, rhs);
        }

        /// <summary>
        /// Define the subtraction operator between two matrices
        /// </summary>
        /// <param name="lhs">The left matrix</param>
        /// <param name="rhs">The right matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 operator -(Matrix2 lhs, Matrix2 rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="lhs">First matrix</param>
        /// <param name="rhs">Second matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Multiply(Matrix2 lhs, Matrix2 rhs)
        {
            var r = new Matrix2
            {
                Row0 = new Vec2(Vec2.Dot(lhs.Row0, rhs.Column0), Vec2.Dot(lhs.Row0, rhs.Column1)),
                Row1 = new Vec2(Vec2.Dot(lhs.Row1, rhs.Column0), Vec2.Dot(lhs.Row1, rhs.Column1))
            };
            return r;
        }

        /// <summary>
        /// Multiply two matrices together
        /// </summary>
        /// <param name="lhs">First matrix</param>
        /// <param name="rhs">Second matrix</param>
        /// <param name="result">The resulting matrix</param>
        public static void Multiply(Matrix2 lhs, Matrix2 rhs, out Matrix2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply the matrix by a scalar value
        /// </summary>
        /// <param name="lhs">The matrix to multiply</param>
        /// <param name="rhs">The scalar to multiply by</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 Multiply(Matrix2 lhs, float rhs)
        {
            return new Matrix2
            {
                Row0 = lhs.Row0 * rhs,
                Row1 = lhs.Row1 * rhs
            };
        }

        /// <summary>
        /// Multiply the matrix by a scalar value
        /// </summary>
        /// <param name="lhs">The matrix to multiply</param>
        /// <param name="rhs">The scalar to multiply by</param>
        /// <param name="result">The resulting matrix</param>
        public static void Multiply(Matrix2 lhs, float rhs, out Matrix2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2, Matrix2x3)"/>
        public static Matrix2x3 Multiply(Matrix2 lhs, Matrix2x3 rhs)
        {
            return Matrix2x3.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2, Matrix2x3, out Matrix2x3)"/>
        public static void Multiply(Matrix2 lhs, Matrix2x3 rhs, out Matrix2x3 result)
        {
            result = Multiply(lhs, rhs);
        }
        
        /// <summary>
        /// Multiply a Matrix2 by a Matrix2x4.
        /// </summary>
        /// <param name="lhs">The Matrix2 operator.</param>
        /// <param name="rhs">The Matrix2x4 operator.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x4 Multiply(Matrix2 lhs, Matrix2x4 rhs)
        {
            return new Matrix2x4
            {
                Row0 = new Vec4(Vec2.Dot(lhs.Row0, rhs.Column0), Vec2.Dot(lhs.Row0, rhs.Column1), Vec2.Dot(lhs.Row0, rhs.Column2), Vec2.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec2.Dot(lhs.Row1, rhs.Column0), Vec2.Dot(lhs.Row1, rhs.Column1), Vec2.Dot(lhs.Row1, rhs.Column2), Vec2.Dot(lhs.Row1, rhs.Column3))
            };
        }

        /// <summary>
        /// Multiply a Matrix2 by a Matrix2x4.
        /// </summary>
        /// <param name="lhs">The Matrix2 operator.</param>
        /// <param name="rhs">The Matrix2x4 operator.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix2 lhs, Matrix2x4 rhs, out Matrix2x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3x2, Matrix2)"/>
        public static Matrix3x2 Multiply(Matrix3x2 lhs, Matrix2 rhs)
        {
            return Matrix3x2.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3x2, Matrix2, out Matrix3x2)"/>
        public static void Multiply(Matrix3x2 lhs, Matrix2 rhs, out Matrix3x2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix4x2 by a Matrix2
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operator.</param>
        /// <param name="rhs">The Matrix2 operator.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x2 Multiply(Matrix4x2 lhs, Matrix2 rhs)
        {
            return new Matrix4x2
            {
                Row0 = new(Vec2.Dot(lhs.Row0, rhs.Column0), Vec2.Dot(lhs.Row0, rhs.Column1)),
                Row1 = new(Vec2.Dot(lhs.Row1, rhs.Column0), Vec2.Dot(lhs.Row1, rhs.Column1)),
                Row2 = new(Vec2.Dot(lhs.Row2, rhs.Column0), Vec2.Dot(lhs.Row2, rhs.Column1)),
                Row3 = new(Vec2.Dot(lhs.Row3, rhs.Column0), Vec2.Dot(lhs.Row3, rhs.Column1))
            };
        }

        /// <summary>
        /// Multiply a Matrix4x2 by a Matrix2
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operator.</param>
        /// <param name="rhs">The Matrix2 operator.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4x2 lhs, Matrix2 rhs, out Matrix4x2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a matrix by a scalar
        /// </summary>
        /// <param name="lhs">The matrix</param>
        /// <param name="rhs">The scalar</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 operator *(Matrix2 lhs, float rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a matrix by a scalar
        /// </summary>
        /// <param name="lhs">The scalar</param>
        /// <param name="rhs">The matrix</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix2 operator *(float lhs, Matrix2 rhs)
        {
            return Multiply(rhs, lhs);
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

        /// <summary>
        /// Multiplication operator between Matrix2 and Matrix2x3
        /// </summary>
        /// <param name="lhs">The Matrix2 operand.</param>
        /// <param name="rhs">The Matrix2x3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x3 operator *(Matrix2 lhs, Matrix2x3 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiplication operator between Matrix2 and Matrix2x4
        /// </summary>
        /// <param name="lhs">The Matrix2 operand.</param>
        /// <param name="rhs">The Matrix2x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2x4 operator *(Matrix2 lhs, Matrix2x4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /* Implemented in Matrix3x2
        public static Matrix3x2 operator *(Matrix3x2 lhs, Matrix2 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix4x2 and Matrix2
        /// </summary>
        /// <param name="lhs">The Matrix4x2 operator.</param>
        /// <param name="rhs">The Matrix2 operator.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x2 operator *(Matrix4x2 lhs, Matrix2 rhs)
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix2 other)
        {
            //todo: compare runtime to Equals(other, 0.0f)
            return Row0 == other.Row0 && Row1 == other.Row1;
        }

        /// <summary>
        /// Indicates whether the current Matrix2 is equal to another within the provided tolerance.
        /// </summary>
        /// <param name="other">The other Matrix2.</param>
        /// <param name="tolerance">The allowed difference between the values.</param>
        /// <returns>True if the difference between the two Matrix2s is less than the tolerance,
        /// false otherwise.</returns>
        public readonly bool Equals(Matrix2 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance);
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
