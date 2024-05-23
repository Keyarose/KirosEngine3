using KirosEngine3.Math.Vector;
using System.Diagnostics.CodeAnalysis;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Defines the non static requirements of a matrix implementation.
    /// </summary>
    /// <typeparam name="MatType">The matrix's type.</typeparam>
    /// <typeparam name="RowType">The matrix's row type, i.e. Vec2, Vec3, etc.</typeparam>
    /// <typeparam name="ColumnType">The matrix's column type.</typeparam>
    /// <typeparam name="SmallType">The type of the matrix's smallest dimension. 
    /// i.e. Matrix2x3 would be 2 and thus Vec2.</typeparam>
    /// <typeparam name="TransType">The type of the matrix's transpose.</typeparam>
    public interface IMatrix<MatType, RowType, ColumnType, SmallType, TransType>
    {
        /// <summary>
        /// Array type accessor for the rows of the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <returns>The row at the given index.</returns>
        RowType this[int row] { get; set; }

        /// <summary>
        /// Array type accessor for the matrix.
        /// </summary>
        /// <param name="row">Row index.</param>
        /// <param name="column">Column index.</param>
        /// <returns>The value at the given indexes.</returns>
        float this[int row, int column] { get; set; }

        /// <summary>
        /// The main diagonal of the matrix.
        /// </summary>
        SmallType Diagonal { get; set; }

        /// <summary>
        /// The matrix's trace.
        /// </summary>
        float Trace { get; }

        /// <summary>
        /// Add the given matrix to this one.
        /// </summary>
        /// <param name="rhs">The matrix to add.</param>
        /// <returns>The resulting matrix.</returns>
        MatType Add(MatType rhs);

        bool Equals(MatType other);

        /// <summary>
        /// Indicates whether the current Matrix is equal to another within the provided tolerance.
        /// </summary>
        /// <param name="other">The matrix to compare.</param>
        /// <param name="tolerance">The allowed difference between the values.</param>
        /// <returns>True if the difference between the two matrices is less than the tolerance, false otherwise.</returns>
        bool Equals(MatType other, float tolerance);
        bool Equals([NotNullWhen(true)] object? obj);

        /// <summary>
        /// Get a collection containing the columns of the matrix.
        /// </summary>
        /// <returns>The columns of the matrix as an array.</returns>
        ColumnType[] GetColumns();
        int GetHashCode();

        /// <summary>
        /// Get a collection containing the rows of the matrix.
        /// </summary>
        /// <returns>The rows of the matrix as an array.</returns>
        RowType[] GetRows();

        /// <summary>
        /// Subtract the given matrix from this one.
        /// </summary>
        /// <param name="rhs">The matrix to subtract.</param>
        /// <returns>The resulting matrix.</returns>
        MatType Subtract(MatType rhs);

        string ToString();
        string ToString(IFormatProvider? formatProvider);
        string ToString(string? format);
        string ToString(string? format, IFormatProvider? formatProvider);

        /// <summary>
        /// Get a transposed copy of the matrix.
        /// </summary>
        /// <returns>The resulting transpose.</returns>
        TransType TransposedCopy();
    }
}