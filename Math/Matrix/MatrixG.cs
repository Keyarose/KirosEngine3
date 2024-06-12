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
        T[,] M;
        readonly int RowCount;
        readonly int ColumnCount;

        /// <summary>
        /// Index accessor for the matrix's cells.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The value in the specified cell.</returns>
        public T this[int row, int column]
        {
            get { return M[row, column]; }
            set { M[row, column] = value; }
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="rowCount">The number of rows for the matrix.</param>
        /// <param name="columnCount">The number of columns for the matrix.</param>
        public MatrixG(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            M = new T[rowCount, columnCount];
        }

        //todo: methods
    }
}
