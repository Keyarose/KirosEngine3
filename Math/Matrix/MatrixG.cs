using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Generic matrix definition.
    /// </summary>
    /// <typeparam name="T">The number value type to use in the matrix.</typeparam>
    public class MatrixG<T> where T : INumber<T>
    {
        private T[,] _m;

        private int _rowCount;
        private int _columnCount;

        /// <summary>
        /// The number of rows in the matrix.
        /// </summary>
        public int RowCount { get { return _rowCount; } }
        /// <inheritdoc cref="RowCount"/>
        public int M => RowCount;

        /// <summary>
        /// The size of the rows in the matrix.
        /// </summary>
        public int RowSize { get { return _columnCount; } }

        /// <summary>
        /// The number of columns in the matrix.
        /// </summary>
        public int ColumnCount { get { return _columnCount; } }
        /// <inheritdoc cref="ColumnCount"/>
        public int N => ColumnCount;

        /// <summary>
        /// The size of the columns in the matrix.
        /// </summary>
        public int ColumnSize { get { return _rowCount; }}

        /// <summary>
        /// Index accessor for the matrix's cells.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The value in the specified cell.</returns>
        public T this[int row, int column]
        {
            get { return _m[row, column]; }

            set { _m[row, column] = value; }
        }

        /// <summary>
        /// Change the number of rows in the matrix. Values that no longer fit are discarded when resizing.
        /// </summary>
        /// <param name="rowCount">The new row count.</param>
        public void SetRowCount(int rowCount)
        {
            T[,] newMat = new T[rowCount, _columnCount];

            if (_rowCount >= rowCount)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < _columnCount; j++)
                    {
                        newMat[i,j] = _m[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < _rowCount; i++)
                {
                    for (int j = 0; j < _columnCount; j++)
                    {
                        newMat[i, j] = _m[i, j];
                    }
                }
            }

            _rowCount = rowCount;
            _m = newMat;
        }

        /// <summary>
        /// Change the number of columns in the matrix. Values that no longer fit are discarded when resizing.
        /// </summary>
        /// <param name="columnCount">The new column count.</param>
        public void SetColumnCount(int columnCount)
        {
            T[,] newMat = new T[_rowCount, columnCount];

            if (_columnCount >= columnCount)
            {
                for (int i = 0; i < _rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        newMat[i, j] = _m[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < _rowCount; i++)
                {
                    for (int j = 0; j < _columnCount; j++)
                    {
                        newMat[i, j] = _m[i, j];
                    }
                }
            }

            _columnCount = columnCount;
            _m = newMat;
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="rowCount">The number of rows for the matrix.</param>
        /// <param name="columnCount">The number of columns for the matrix.</param>
        public MatrixG(int rowCount, int columnCount)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;

            _m = new T[rowCount, columnCount];
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The Matrix to copy.</param>
        private MatrixG(MatrixG<T> other)
        {
            _rowCount = other.RowCount;
            _columnCount = other.ColumnCount;

            _m = new T[_rowCount, _columnCount];

            for (int i = 0; i < _rowCount; i++)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    _m[i, j] = other[i, j];
                }
            }
        }

        /// <summary>
        /// Constructor from vectors as rows.
        /// </summary>
        /// <param name="rows">The vector rows, which should all be the same length.</param>
        /// <exception cref="ArgumentException">Thrown if the array is empty.</exception>
        public MatrixG(VecG<T>[] rows)
        {
            ArgumentNullException.ThrowIfNull(rows);

            if (rows.Length < 1)
            {
                throw new ArgumentException("A matrix requires at least one row.");
            }

            int r1Size = rows.First().Size;
            foreach (var r in rows)
            {
                if (r.Size != r1Size)
                    throw new ArgumentException(string.Format("All rows must be of the same size, row {0} is not the same as the first row in the array.", r));
            }

            _rowCount = rows.Length;
            _columnCount = r1Size;

            _m = new T[_rowCount, _columnCount];

            for (int i = 0; i < _rowCount; ++i)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    _m[i, j] = rows[i][j];
                }
            }
        }

        /// <summary>
        /// Return the Columns of the matrix as an array.
        /// </summary>
        /// <returns>The columns of the matrix.</returns>
        public VecG<T>[] GetColumns()
        {
            VecG<T>[] result = new VecG<T>[_columnCount];

            for (int i = 0; i< _columnCount; i++)
            {
                result[i] = new VecG<T>(ColumnSize);

                for (int j = 0; j < ColumnSize; j++)
                {
                    result[i][j] = _m[j, i];
                }
            }

            return result;
        }

        /// <summary>
        /// The matrix's Column Space, see <see cref="GetColumns"/>
        /// </summary>
        public VecG<T>[] ColumnSpace => GetColumns();

        /// <summary>
        /// Return the Rows of the matrix as an array.
        /// </summary>
        /// <returns></returns>
        public VecG<T>[] GetRows()
        {
            VecG<T>[] result = new VecG<T>[RowCount];

            for (int i = 0; i < RowCount; i++)
            {
                result[i] = new VecG<T>(RowSize);

                for (int j = 0; j < RowSize; j++)
                {
                    result[i][j] = _m[j, i];
                }
            }

            return result;
        }

        /// <summary>
        /// The matrix's Row Space, see <see cref="GetRows"/>
        /// </summary>
        public VecG<T>[] RowSpace =>GetRows();

        /// <summary>
        /// Clone the matrix.
        /// </summary>
        /// <returns>The resulting clone.</returns>
        public MatrixG<T> Clone()
        {
            return new MatrixG<T>(this);
        }

        /// <summary>
        /// Augment the matrix with the given vector.
        /// </summary>
        /// <param name="b">The vector to augment the matrix with, it's size must be the same as the matrix's ColumnSize.</param>
        /// <exception cref="InvalidOperationException">Thrown if the vector is not the correct size.</exception>
        public void AugmentMatrix(VecG<T> b)
        {
            if (b.Size != ColumnSize)
                throw new InvalidOperationException(string.Format("The vector to augment the matrix with must have the same size as the matrix's ColumnSize. Vector size: {0}, Matrix column size: {1}", b.Size, ColumnSize));

            SetColumnCount(ColumnCount + 1);

            for (int i = 0; i < b.Size; i++)
            {
                _m[i, ColumnSize - 1] = b[i];
            }
        }

        /// <summary>
        /// Augment a copy of the matrix with the given vector.
        /// </summary>
        /// <param name="b">The vector to augment the matrix with, it's size must be the same as the matrix's ColumnSize.</param>
        /// <returns>The resulting copy augmented with the vector.</returns>
        public MatrixG<T> AugmentMatrixCopy(VecG<T> b)
        {
            MatrixG<T> result = Clone();
            try
            {
                result.AugmentMatrix(b);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Augment a matrix with the given vector.
        /// </summary>
        /// <param name="a">The matrix to augment.</param>
        /// <param name="b">The vector to augment the matrix with, it's size must be the same as the matrix's ColumnSize.</param>
        /// <returns>The resulting matrix.</returns>
        public static MatrixG<T> AugmentMatrix(MatrixG<T> a, VecG<T> b)
        {
            try
            {
                a.AugmentMatrix(b);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            return a;
        }

        #region Elementary Row Operations
        /// <summary>
        /// Perform a row interchange operation on the matrix.
        /// </summary>
        /// <param name="firstRow">The index of the first row to exchange.</param>
        /// <param name="secondRow">The index of the second row to exchange.</param>
        public void RowInterchange(int firstRow, int secondRow)
        {
            for (int i = 0; i < RowSize; i++)//move each row value by value
            {
                (_m[secondRow, i], _m[firstRow, i]) = (_m[firstRow, i], _m[secondRow, i]);
            }
        }

        /// <summary>
        /// Perform a row interchange operation on a copy of the matrix.
        /// </summary>
        /// <param name="firstRow">The index of the first row to exchange.</param>
        /// <param name="secondRow">The index of the second row to exchange.</param>
        /// <returns>The resulting copy.</returns>
        public MatrixG<T> RowInterchangeCopy(int firstRow, int secondRow)
        {
            MatrixG<T> result = Clone();

            result.RowInterchange(firstRow, secondRow);

            return result;
        }

        /// <summary>
        /// Perform a row interchange operation on a matrix.
        /// </summary>
        /// <param name="mat">The matrix to operate on.</param>
        /// <param name="firstRow">The index of the first row to exchange.</param>
        /// <param name="secondRow">The index of the second row to exchange.</param>
        public static void RowInterchange(MatrixG<T> mat, int firstRow, int secondRow)
        {
            mat.RowInterchange(firstRow, secondRow);
        }

        /// <summary>
        /// Perform a row scaling operation on the matrix.
        /// </summary>
        /// <param name="row">The row to multiply.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        public void RowScaling(int row, T scalar)
        {
            for (int i = 0; row < RowSize; row++)
            {
                _m[row, i] *= scalar;
            }
        }

        /// <summary>
        /// Perform a row scaling operation on a copy of the matrix.
        /// </summary>
        /// <param name="row">The row to multiply.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        /// <returns>The resulting matrix.</returns>
        public MatrixG<T> RowScalingCopy(int row, T scalar)
        {
            MatrixG<T> result = Clone();

            result.RowScaling(row, scalar);

            return result;
        }

        /// <summary>
        /// Perform a row scaling operation on a matrix.
        /// </summary>
        /// <param name="mat">The matrix to operate on.</param>
        /// <param name="row">The row to multiply.</param>
        /// <param name="scalar">The scalar to multiply the row by.</param>
        public static void RowScaling(MatrixG<T> mat, int row, T scalar)
        {
            mat.RowScaling(row, scalar);
        }

        /// <summary>
        /// Perform a row addition operation on the matrix.
        /// </summary>
        /// <param name="rowAddend">The row to be added to another.</param>
        /// <param name="rowAddTo">The row to add to.</param>
        /// <param name="multiplier">The number of times for the row to be added.</param>
        public void RowAddition(int rowAddend, int rowAddTo, T multiplier)
        {
            for (int i = 0; i < RowSize; i++)
            {
                _m[rowAddTo, i] += (multiplier * _m[rowAddend, i]);
            }
        }

        /// <summary>
        /// Perform a row addition operation on a copy of the matrix.
        /// </summary>
        /// <param name="rowAddend">The row to be added to another.</param>
        /// <param name="rowAddTo">The row to add to.</param>
        /// <param name="multiplier">The number of times for the row to be added.</param>
        /// <returns>The resulting matrix.</returns>
        public MatrixG<T> RowAdditionCopy(int rowAddend, int rowAddTo, T multiplier)
        {
            MatrixG<T> result = Clone();

            result.RowAddition(rowAddend, rowAddTo, multiplier);

            return result;
        }

        /// <summary>
        /// Perform a row addition operation on a matrix.
        /// </summary>
        /// <param name="mat">The matrix to operate on.</param>
        /// <param name="rowAddend">The row to be added to another.</param>
        /// <param name="rowAddTo">The row to add to.</param>
        /// <param name="multiplier">The number of times for the row to be added.</param>
        public static void RowAddition(MatrixG<T> mat, int rowAddend, int rowAddTo, T multiplier)
        {
            mat.RowAddition(rowAddend, rowAddTo, multiplier);
        }
        #endregion

        /// <summary>
        /// Convert the matrix into it's reduced row echelon form using gaussian elimination.
        /// </summary>
        /// <returns>The resulting matrix.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public MatrixG<T> ReducedRowEchelonGaussian()
        {
            throw new NotImplementedException();
        }

        #region ToString
        /// <inheritdoc/>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(string? format)
        {
            return ToString(format, null);
        }

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public string ToString(IFormatProvider? formatProvider)
        {
            return ToString(null, formatProvider);
        }

        /// <inheritdoc/>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            string result = "{";

            for (int i = 0; i < RowCount; i++)
            {
                string line = "(";
                for (int j = 0; j < ColumnCount; j++)
                {
                    line += _m[i, j].ToString(format, formatProvider);
                    if (j != ColumnCount - 1)
                        line += ", ";
                }

                if (i == RowCount - 1)
                {
                    result += line + ")}\n";
                }
                else
                    result += line + ")\n";
            }

            return result;
        }
        #endregion
        //todo: methods
    }
}
