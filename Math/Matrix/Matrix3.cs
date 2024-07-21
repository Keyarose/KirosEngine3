
using KirosEngine3.Math.Data;
using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Three by three matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3 : IEquatable<Matrix3>, IFormattable, IMatrix<Matrix3, Vec3, Vec3, Vec3, Matrix3>
    {
        /// <summary>
        /// The first row of the matrix.
        /// </summary>
        public Vec3 Row0;
        /// <summary>
        /// The second row of the matrix.
        /// </summary>
        public Vec3 Row1;
        /// <summary>
        /// The third row of the matrix.
        /// </summary>
        public Vec3 Row2;

        /// <summary>
        /// The Identity matrix
        /// </summary>
        public static Matrix3 Identity => new Matrix3(Vec3.UnitX, Vec3.UnitY, Vec3.UnitZ);

        /// <summary>
        /// The zero matrix
        /// </summary>
        public static Matrix3 Zero => new Matrix3(Vec3.Zero, Vec3.Zero, Vec3.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix
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
        /// The second column of the matrix
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
        /// The third column of the matrix
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
        #endregion

        /// <inheritdoc/>
        public readonly Vec3[] GetColumns()
        {
            return [Column0, Column1, Column2];
        }
        /// <inheritdoc/>
        public readonly Vec3[] GetRows()
        {
            return [Row0, Row1, Row2];
        }

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
                if (column < 0 || column > 2)
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
                if (column < 0 || column > 2)
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
        /// Array type accessor for the rows of the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>The row at the given index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is outside the range of 0-2</exception>
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
                    case 2:
                        return Row2;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3.", row));
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
                        throw new IndexOutOfRangeException(string.Format("Row index: {0} out of range for Matrix3.", row));
                }
            }
        }

        #region Constructors
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
        public static Matrix3 RowInterchangeElemMat(int r1, int r2)
        {
            if (r1 == r2)
                throw new InvalidOperationException(string.Format("Cannot interchange a row with itself."));

            if (r1 < 0 || r1 > 2)
            {
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r1));
            }

            if (r2 < 0 || r2 > 2)
            {
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r2));
            }

            //switch row 0 and 2
            if ((r1 == 0 && r2 == 2) || (r1 == 2 && r2 == 0))
            {
                return new Matrix3(0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            }

            //switch row 0 and row 1
            if ((r1 == 0 && r2 == 1) || (r1 == 1 && r2 == 0))
            {
                return new Matrix3(0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f);
            }
            else
            {
                //switch row 1 and 2
                return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f);
            }
        }

        /// <summary>
        /// Produce an elementary matrix for the scalar multiplication row operation.
        /// </summary>
        /// <param name="row">The row to be multiplied.</param>
        /// <param name="scalar">The scalar to multiply by.</param>
        /// <returns>The 3D elementary matrix that performs the scalar multiplication.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the row index is out of range.</exception>
        public static Matrix3 RowMultiplyElemMat(int row, float scalar)
        {
            switch (row)
            {
                case 0:
                    return new Matrix3(scalar, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
                case 1:
                    return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, scalar, 0.0f, 0.0f, 0.0f, 1.0f);
                case 2:
                    return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, scalar);
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", row));
            }
        }

        /// <summary>
        /// Produce an elementary matrix for adding one row to another.
        /// </summary>
        /// <param name="r1">The index of the row to add.</param>
        /// <param name="r2">The index of the row to add to.</param>
        /// <param name="scalar">The number of times to add the first row.</param>
        /// <returns>The 3D elementary matrix that performs the row operation.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the indexes are the same.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown if the indexes are out of range for Matrix3.</exception>
        public static Matrix3 RowAddElemMat(int r1, int r2, float scalar)
        {
            if (r1 == r2)
                throw new InvalidOperationException(string.Format("Adding a row to itself is not a valid operation."));

            switch (r1)
            {
                case 0:
                    {
                        if (r2 == 1)//add row 0 to row 1
                        {
                            return new Matrix3(1.0f, 0.0f, 0.0f, scalar, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 2)//add row 0 to row 2
                        {
                            return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, scalar, 0.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r2));
                    }
                case 1:
                    {
                        if (r2 == 0)//add row 1 to row 0
                        {
                            return new Matrix3(1.0f, scalar, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 2)//add row 1 to row 2
                        {
                            return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, scalar, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r2));
                    }
                case 2:
                    {
                        if (r2 == 0)//add row 2 to row 0
                        {
                            return new Matrix3(1.0f, 0.0f, scalar, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
                        }
                        else if (r2 == 1)//add row 2 to row 1
                        {
                            return new Matrix3(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, scalar, 0.0f, 0.0f, 1.0f);
                        }
                        else
                            throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r2));
                    }
                default:
                    throw new IndexOutOfRangeException(string.Format("Row index: {0} is out of range for Matrix3.", r1));
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
        public readonly Matrix3 RowInterchange(int r1, int r2)
        {
            return RowInterchangeElemMat(r1, r2) * this;
        }

        /// <summary>
        /// Perform the row multiplication operation on this matrix.
        /// </summary>
        /// <param name="row">Index of the row to multiply.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix3 RowMultiplication(int row, float scalar)
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
        public readonly Matrix3 RowAddition(int r1, int r2, float scalar)
        {
            return RowAddElemMat(r1, r2, scalar) * this;
        }
        #endregion

        #region Transpose
        /// <summary>
        /// Converts a matrix into it's transpose
        /// </summary>
        public void Transpose()
        {
            this = Transpose(this);
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

        /// <summary>
        /// Get a copy of the matrix's transpose.
        /// </summary>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix3 TransposedCopy()
        {
            return Transpose(this);
        }
        #endregion

        #region Invert
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
            if (c.Determinant != 0)
            {
                c.Invert();
            }
            return c;
        }

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

            //calculate the determinant here since we have to do some of the work anyway
            float determ = (row0x * inRow0X) + (row0y * inRow1X) + (row0z * inRow2X);

            //check that the determinant isn't zero
            if (determ.IsZero())
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
        #endregion

        #region Normalize
        /// <summary>
        /// Normalize the matrix by dividing by the determinant. Also the final
        /// step in Inverting.
        /// </summary>
        public void Normalize()
        {
            var determinant = Determinant;
            if (determinant.IsZero())
            {
                Console.WriteLine("Matrix3: {0} has a determinant of 0. Thus normalize is undefined.", this);
                Logger.WriteToLog("Matrix3: {0} has a determinant of 0. Thus normalize is undefined.", this);
                DebugConsole.WriteLine("Matrix3: {0} has a determinant of 0. Thus normalize is undefined.", this);
            }
            else
            {
                Row0 /= determinant;
                Row1 /= determinant;
                Row2 /= determinant;
            }
        }

        /// <summary>
        /// Create a normalized copy of the matrix.
        /// </summary>
        /// <returns>A copy of the matrix that has been normalized, or the original matrix
        /// if it is undefined.</returns>
        public readonly Matrix3 NormalizedCopy()
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
        /// <returns>The resulting matrix.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if any of the indexes are out of range.</exception>
        public static Matrix3 Swizzle(Matrix3 mat, int row0Row, int row1Row, int row2Row)
        {
            if (row0Row < 0 || row0Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3.", row0Row));
            if (row1Row < 0 || row1Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3.", row1Row));
            if (row2Row < 0 || row2Row > 2)
                throw new IndexOutOfRangeException(string.Format("Row index: {0} is not valid for Matrix3.", row2Row));

            var result = new Matrix3
            {
                Row0 = mat[row0Row],
                Row1 = mat[row1Row],
                Row2 = mat[row2Row]
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
        /// <param name="result">The resulting matrix.</param>
        public static void Swizzle(Matrix3 mat, int row0Row, int row1Row, int row2Row, out Matrix3 result)
        {
            result = Swizzle(mat, row0Row, row1Row, row2Row);
        }

        /// <summary>
        /// Create a swizzled copy of the matrix.
        /// </summary>
        /// <param name="row0Row">The index of the row to be moved to row 0.</param>
        /// <param name="row1Row">The index of the row to be moved to row 1.</param>
        /// <param name="row2Row">The index of the row to be moved to row 2.</param>
        /// <returns>The resulting matrix.</returns>
        public readonly Matrix3 SwizzleCopy(int row0Row, int row1Row, int row2Row)
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
        /// Create a copy of the matrix without any scaling
        /// </summary>
        /// <returns>The matrix without scaling</returns>
        public readonly Matrix3 ClearScale()//todo: does this actually work with combined transforms?
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

        #region Rotate
        /// <summary>
        /// Create a copy of the matrix with the rotation removed
        /// </summary>
        /// <returns>The matrix without rotation</returns>
        public readonly Matrix3 ClearRotation()//todo: check math proof
        {
            var c = this;
            c.Row0 = new Vec3(Row0.Length, 0.0f, 0.0f);
            c.Row1 = new Vec3(0.0f, Row1.Length, 0.0f);
            c.Row2 = new Vec3(0.0f, 0.0f, Row2.Length);
            return c;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the x axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 CreateRotationX(float angle)//Todo: row/column major methods
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            var result = Identity;
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the x axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationX(float angle, out Matrix3 result)
        {
            result = CreateRotationX(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Y axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 CreateRotationY(float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            var result = Identity;
            result.Row0.X = cos;
            result.Row0.Z = -sin;
            result.Row2.X = sin;
            result.Row2.Z = cos;

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Y axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationY(float angle, out Matrix3 result)
        {
            result = CreateRotationY(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Z axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 CreateRotationZ(float angle)
        {
            var cos = MathF.Cos(angle);
            var sin = MathF.Sin(angle);

            var result = Identity;
            result.Row0.X = cos;
            result.Row0.Y = sin;
            result.Row1.X = -sin;
            result.Row1.Y = cos;

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the Z axis. (radians) (row major)
        /// </summary>
        /// <param name="angle">The angle to rotate by in radians.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationZ(float angle, out Matrix3 result)
        {
            result = CreateRotationZ(angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 CreateRotationOnAxis(Vec3 axis, float angle)
        {//todo: double check major
            //get a normalized working copy
            Vec3 axisNorm = axis.NormalizedCopy();

            float cos = MathF.Cos(-angle);
            float sin = MathF.Sin(-angle);
            float t = 1.0f - cos;

            //axis component conversion
            float tXSq = t * axisNorm.X * axisNorm.X;
            float tXY = t * axisNorm.X * axisNorm.Y;
            float tXZ = t * axisNorm.X * axisNorm.Z;
            float tYSq = t * axisNorm.Y * axisNorm.Y;
            float tYZ = t * axisNorm.Y * axisNorm.Z;
            float tZSq = t * axisNorm.Z * axisNorm.Z;

            float sinX = sin * axisNorm.X;
            float sinY = sin * axisNorm.Y;
            float sinZ = sin * axisNorm.Z;

            Matrix3 result = new Matrix3
            {
                M00 = cos + tXSq, M01 = tXY - sinZ, M02 = tXZ + sinY,
                M10 = tXY + sinZ, M11 = cos + tYSq, M12 = tYZ - sinX,
                M20 = tXZ - sinY, M21 = tYZ + sinX, M22 = cos + tZSq
            };

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation around the provided axis. (radians) (row major)
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="angle">The angle to rotate by cc-wise looking in the axis' direction.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotationOnAxis(Vec3 axis, float angle, out Matrix3 result)
        {
            result = CreateRotationOnAxis(axis, angle);
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 CreateRotFromQuaternion(Quaternion quat)
        {
            float sqX = quat.X * quat.X;
            float sqY = quat.Y * quat.Y;
            float sqZ = quat.Z * quat.Z;
            float sqW = quat.W * quat.W;

            float xy = quat.X * quat.Y;
            float xz = quat.X * quat.Z;
            float xw = quat.X * quat.W;

            float yz = quat.Y * quat.Z;
            float yw = quat.Y * quat.W;

            float zw = quat.Z * quat.W;

            float sumSqrU2 = 2.0f / (sqX + sqY + sqZ + sqW);

            Matrix3 result = new Matrix3
            {
                M00 = 1.0f - (sumSqrU2 * (sqY + sqZ)), M01 = sumSqrU2 * (xy + zw), M02 = sumSqrU2 * (xz - yw),
                M10 = sumSqrU2 * (xy - zw), M11 = 1.0f - (sumSqrU2 * (sqX + sqZ)), M12 = sumSqrU2 * (yz - xw),
                M20 = sumSqrU2 * (xz + yw), M21 = sumSqrU2 * (yz - xw), M22 = 1.0f - (sumSqrU2 * (sqX - sqY)),
            };

            return result;
        }

        /// <summary>
        /// Create a matrix to represent rotation from a Quaternion. (row major)
        /// </summary>
        /// <param name="quat">The Quaternion to create the rotation from.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void CreateRotFromQuaternion(Quaternion quat, out Matrix3 result)
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
            var r0 = Row0;
            var r1 = Row1;
            var r2 = Row2;

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
        /// <param name="lhs">First matrix to add</param>
        /// <param name="rhs">Second matrix to add</param>
        /// <returns>The resulting matrix in a new instance</returns>
        public static Matrix3 Add(Matrix3 lhs, Matrix3 rhs)
        {
            var r = new Matrix3
            {
                Row0 = lhs.Row0 + rhs.Row0,
                Row1 = lhs.Row1 + rhs.Row1,
                Row2 = lhs.Row2 + rhs.Row2
            };
            return r;
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">First matrix to add</param>
        /// <param name="rhs">Second matrix to add</param>
        /// <param name="result">The resulting matrix in a new instance</param>
        public static void Add(Matrix3 lhs, Matrix3 rhs, out Matrix3 result)
        {
            result = Add(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix3 Add(Matrix3 rhs)
        {
            return Add(this, rhs);
        }

        /// <summary>
        /// Add two matrices together
        /// </summary>
        /// <param name="lhs">First matrix to add</param>
        /// <param name="rhs">Second matrix to add</param>
        /// <returns>The resulting matrix in a new instance</returns>
        public static Matrix3 operator +(Matrix3 lhs, Matrix3 rhs)
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
        public static Matrix3 Subtract(Matrix3 lhs, Matrix3 rhs)
        {
            var r = new Matrix3
            {
                Row0 = lhs.Row0 - rhs.Row0,
                Row1 = lhs.Row1 - rhs.Row1,
                Row2 = lhs.Row2 - rhs.Row2
            };

            return r;
        }

        /// <summary>
        /// Subtract one matrix from another
        /// </summary>
        /// <param name="lhs">The matrix to subtract from</param>
        /// <param name="rhs">The matrix to subtract</param>
        /// <param name="result">The resulting matrix</param>
        public static void Subtract(Matrix3 lhs, Matrix3 rhs, out Matrix3 result)
        {
            result = Subtract(lhs, rhs);
        }

        /// <inheritdoc/>
        public readonly Matrix3 Subtract(Matrix3 rhs)
        {
            return Subtract(this, rhs);
        }

        /// <summary>
        /// Define the subtraction operator between two matrices
        /// </summary>
        /// <param name="lhs">The left matrix operand</param>
        /// <param name="rhs">The right matrix operand</param>
        /// <returns>The resulting matrix</returns>
        public static Matrix3 operator -(Matrix3 lhs, Matrix3 rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply two matrices together.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 Multiply(Matrix3 lhs, Matrix3 rhs)
        {
            Matrix3 result = new Matrix3
            {
                Row0 = new Vec3(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2)),
                Row2 = new Vec3(Vec3.Dot(lhs.Row2, rhs.Column0), Vec3.Dot(lhs.Row2, rhs.Column1), Vec3.Dot(lhs.Row2, rhs.Column2))
            };

            return result;
        }

        /// <summary>
        /// Multiply two matrices together.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3 lhs, Matrix3 rhs, out Matrix3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply the matrix by a scalar value.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 Multiply(Matrix3 lhs, float rhs)
        {
            var r = new Matrix3
            {
                Row0 = lhs.Row0 * rhs,
                Row1 = lhs.Row1 * rhs,
                Row2 = lhs.Row2 * rhs
            };

            return r;
        }

        /// <summary>
        /// Multiply the matrix by a scalar value.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3 lhs, float rhs, out Matrix3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3, Matrix3x2)"/>
        public static Matrix3x2 Multiply(Matrix3 lhs, Matrix3x2 rhs)
        {
            return Matrix3x2.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix3x2.Multiply(Matrix3, Matrix3x2, out Matrix3x2)"/>
        public static void Multiply(Matrix3 lhs, Matrix3x2 rhs, out Matrix3x2 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix3 by a Matrix3x4
        /// </summary>
        /// <param name="lhs">The Matrix3 operator.</param>
        /// <param name="rhs">The Matrix3x4 operator.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 Multiply(Matrix3 lhs, Matrix3x4 rhs)
        {
            return new Matrix3x4
            {
                Row0 = new Vec4(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2), Vec3.Dot(lhs.Row0, rhs.Column3)),
                Row1 = new Vec4(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2), Vec3.Dot(lhs.Row1, rhs.Column3)),
                Row2 = new Vec4(Vec3.Dot(lhs.Row2, rhs.Column0), Vec3.Dot(lhs.Row2, rhs.Column1), Vec3.Dot(lhs.Row2, rhs.Column2), Vec3.Dot(lhs.Row2, rhs.Column3))
            };
        }

        /// <summary>
        /// Multiply a Matrix3 by a Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix3 operator.</param>
        /// <param name="rhs">The Matrix3x4 operator.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix3 lhs, Matrix3x4 rhs, out Matrix3x4 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2x3, Matrix3)"/>
        public static Matrix2x3 Multiply(Matrix2x3 lhs, Matrix3 rhs)
        {
            return Matrix2x3.Multiply(lhs, rhs);
        }

        /// <inheritdoc cref="Matrix2x3.Multiply(Matrix2x3, Matrix3, out Matrix2x3)"/>
        public static void Multiply(Matrix2x3 lhs, Matrix3 rhs, out Matrix2x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a Matrix4x3 by a Matrix3.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The matrix3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x3 Multiply(Matrix4x3 lhs, Matrix3 rhs)
        {
            return new Matrix4x3
            {
                Row0 = new Vec3(Vec3.Dot(lhs.Row0, rhs.Column0), Vec3.Dot(lhs.Row0, rhs.Column1), Vec3.Dot(lhs.Row0, rhs.Column2)),
                Row1 = new Vec3(Vec3.Dot(lhs.Row1, rhs.Column0), Vec3.Dot(lhs.Row1, rhs.Column1), Vec3.Dot(lhs.Row1, rhs.Column2)),
                Row2 = new Vec3(Vec3.Dot(lhs.Row2, rhs.Column0), Vec3.Dot(lhs.Row2, rhs.Column1), Vec3.Dot(lhs.Row2, rhs.Column2)),
                Row3 = new Vec3(Vec3.Dot(lhs.Row3, rhs.Column0), Vec3.Dot(lhs.Row3, rhs.Column1), Vec3.Dot(lhs.Row3, rhs.Column2))
            };
        }

        /// <summary>
        /// Multiply a Matrix4x3 by a Matrix3.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The matrix3 operand.</param>
        /// <param name="result">The resulting matrix.</param>
        public static void Multiply(Matrix4x3 lhs, Matrix3 rhs, out Matrix4x3 result)
        {
            result = Multiply(lhs, rhs);
        }

        /*======================================================
         Multiply operators
         =======================================================*/

        /// <summary>
        /// Multiply two matrices together.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 operator *(Matrix3 lhs, Matrix3 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Matrix multiplied by scalar operator.
        /// </summary>
        /// <param name="lhs">The matrix operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 operator *(Matrix3 lhs, float rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Matrix multiplied by scalar operator.
        /// </summary>
        /// <param name="lhs">The scalar operand.</param>
        /// <param name="rhs">The matrix operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3 operator *(float lhs, Matrix3 rhs)
        {
            return Multiply(rhs, lhs);
        }

        /* Implemented in Matrix3x2
        public static Matrix3x2 operator *(Matrix3 lhs, Matrix3x2 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix3 and Matrix3x4.
        /// </summary>
        /// <param name="lhs">The Matrix3 operand.</param>
        /// <param name="rhs">The Matrix3x4 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix3x4 operator *(Matrix3 lhs, Matrix3x4 rhs)
        {
            return Multiply(lhs, rhs);
        }

        /* Implemented in Matrix2x3
        public static Matrix2x3 operator *(Matrix2x3 lhs, Matrix3 rhs)
        {
            return Multiply(lhs, rhs);
        }*/

        /// <summary>
        /// Multiplication operator between Matrix4x3 and Matrix3.
        /// </summary>
        /// <param name="lhs">The Matrix4x3 operand.</param>
        /// <param name="rhs">The Matrix3 operand.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix4x3 operator *(Matrix4x3 lhs, Matrix3 rhs)
        {
            return Multiply(lhs, rhs);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix3 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2;
        }

        /// <summary>
        /// Indicates whether the current Matrix3 is equal to another within the provided tolerance
        /// </summary>
        /// <param name="other">The other Matrix3</param>
        /// <param name="tolerance">The allowed difference between the values</param>
        /// <returns>True if the difference between the two Matrix3s is less than the tolerance, false otherwise.</returns>
        public readonly bool Equals(Matrix3 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance) && Row2.Equals(other.Row2, tolerance);
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

        ///<inheritdoc/>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            var r0 = Row0.ToString(format, formatProvider);
            var r1 = Row1.ToString(format, formatProvider);
            var r2 = Row2.ToString(format, formatProvider);

            return string.Format("{0}\n{1}\n{2}", r0, r1, r2);
        }
        #endregion

        /// <inheritdoc/>
        public string ToDrawString()
        {
            string result;
            string ln1, ln2, ln3, ln4, ln5;

            //column 1
            ln2 = string.Format("\u2502{0}", M00);
            ln3 = string.Format("\u2502{0}", M10);
            ln4 = string.Format("\u2502{0}", M20);

            for (int i = 1; i < 3; i++)
            {
                if (ln2.Length > ln3.Length && ln2.Length > ln4.Length)
                {
                    int diff23 = ln2.Length - ln3.Length;
                    int diff24 = ln2.Length - ln4.Length;

                    ln2 += string.Format(" {0}", this[0, i]);
                    ln3 += new string(' ', diff23) + string.Format(" {0}", this[1, i]);
                    ln4 += new string(' ', diff24) + string.Format(" {0}", this[2, i]);
                }
                else if (ln3.Length > ln2.Length && ln3.Length > ln4.Length)
                {
                    int diff = ln3.Length - ln2.Length;
                    int diff34 = ln3.Length - ln4.Length;

                    ln2 += new string(' ', diff) + string.Format(" {0}", this[0, i]);
                    ln3 += string.Format(" {0}", this[1, i]);
                    ln4 += new string(' ', diff34) + string.Format(" {0}", this[2, i]);
                }
                else
                {
                    int diff = ln4.Length - ln2.Length;
                    int diff43 = ln4.Length - ln3.Length;

                    ln2 += new string(' ', diff) + string.Format(" {0}", this[0, i]);
                    ln3 += new string(' ', diff43) + string.Format(" {0}", this[1, i]);
                    ln4 += string.Format(" {0}", this[2, i]);
                }
            }

            //end bracket
            if (ln2.Length > ln3.Length && ln2.Length > ln4.Length)
            {
                int diff = ln2.Length - ln3.Length;
                int diff24 = ln2.Length - ln4.Length;

                ln2 += "\u2502\n";
                ln3 += new string(' ', diff) + "\u2502\n";
                ln4 += new string(' ', diff24) + "\u2502\n";

            }
            else if (ln3.Length > ln2.Length && ln3.Length > ln4.Length)
            {
                int diff = ln3.Length - ln2.Length;
                int diff34 = ln3.Length - ln4.Length;

                ln2 += new string(' ', diff) + "\u2502\n";
                ln3 += "\u2502\n";
                ln4 += new string(' ', diff34) + "\u2502\n";
            }
            else
            {
                int diff = ln4.Length - ln2.Length;
                int diff43 = ln4.Length - ln3.Length;

                ln2 += new string(' ', diff) + "\u2502\n";
                ln3 += new string(' ', diff43) + "\u2502\n";
                ln4 += "\u2502\n";
            }

            ln1 = "\u250C" + new string(' ', ln2.Length - 3) + "\u2510\n";
            ln5 = "\u2514" + new string(' ', ln2.Length - 3) + "\u2518\n";

            result = ln1 + ln2 + ln3 + ln4 + ln5;

            return result;
        }

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Matrix3 to Matrix3
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator Matrix3(OpenTK.Mathematics.Matrix3 m)
        {
            return new Matrix3
            {
                Row0 = m.Row0,
                Row1 = m.Row1,
                Row2 = m.Row2
            };
        }

        /// <summary>
        /// Handle conversion from Matrix3 to OpenTK's Matrix3
        /// </summary>
        /// <param name="m">The matrix to convert</param>
        public static implicit operator OpenTK.Mathematics.Matrix3(Matrix3 m)
        {
            return new OpenTK.Mathematics.Matrix3
            {
                Row0 = m.Row0,
                Row1 = m.Row1,
                Row2 = m.Row2
            };
        }
        #endregion
#endif
    }
}
