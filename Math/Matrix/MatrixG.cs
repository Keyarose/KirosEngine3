using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    public struct MatrixG<T> where T : INumber<T>
    {
        T[,] M;
        readonly int RowCount;
        readonly int ColumnCount;

        public T this[int row, int column]
        {
            get { return M[row, column]; }
            set { M[row, column] = value; }
        }

        public MatrixG(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            M = new T[rowCount, columnCount];
        }
    }
}
