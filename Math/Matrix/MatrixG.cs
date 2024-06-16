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
    public struct MatrixG<T> where T : INumber<T>
    {
        private T[,] _m;

        private int _rowCount;
        private int _columnCount;

        /// <summary>
        /// The number of rows in the matrix.
        /// </summary>
        public readonly int RowCount { get { return _rowCount; } }

        /// <summary>
        /// The number of columns in the matrix.
        /// </summary>
        public readonly int ColumnCount { get { return _columnCount; } }

        /// <summary>
        /// Index accessor for the matrix's cells.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The value in the specified cell.</returns>
        public readonly T this[int row, int column]
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
        /// Constructor from vectors as rows.
        /// </summary>
        /// <param name="rows">The vector rows.</param>
        /// <exception cref="ArgumentException">Thrown if the array is empty.</exception>
        public MatrixG(VecG<T>[] rows)
        {
            if (rows.Length < 1)
            {
                throw new ArgumentException("A matrix requires at least one row.");
            }

            _rowCount = rows.Length;
            _columnCount = rows.FirstOrDefault()!.Size;

            _m = new T[_rowCount, _columnCount];
        }
        //todo: methods
    }
}
