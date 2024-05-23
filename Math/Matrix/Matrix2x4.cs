using KirosEngine3.Math.Vector;

namespace KirosEngine3.Math.Matrix
{
    public struct Matrix2x4 : IEquatable<Matrix2x4>
    {
        public Vec4 Row0;
        public Vec4 Row1;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix2x4 Zero => new Matrix2x4(Vec4.Zero, Vec4.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix.
        /// </summary>
        public Vec2 Column0
        {
            readonly get => new Vec2(Row0.X, Row1.X);
            set { Row0.X = value.X; Row1.X = value.X; }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec2 Column1
        {
            readonly get => new Vec2(Row0.Y, Row1.Y);
            set { Row0.Y = value.X; Row1.Y = value.Y; }
        }

        /// <summary>
        /// The third column of the matrix.
        /// </summary>
        public Vec2 Column2
        {
            readonly get => new Vec2(Row0.Z, Row1.Z);
            set { Row0.Z = value.X; Row1.Z = value.Y; }
        }

        /// <summary>
        /// The fourth column of the matrix.
        /// </summary>
        public Vec2 Column3
        {
            readonly get => new Vec2(Row0.W, Row1.W);
            set { Row0.W = value.X; Row1.W = value.Y; }
        }
        #endregion


        public readonly Vec2[] GetColumns()
        {
            return [Column0, Column1, Column2, Column3];
        }

        public readonly Vec4[] GetRows()
        {
            return [Row0, Row1];
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec4s.
        /// </summary>
        /// <param name="row0">Row 0.</param>
        /// <param name="row1">Row 1.</param>
        public Matrix2x4(Vec4 row0, Vec4 row1)
        {
            Row0 = row0;
            Row1 = row1;
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, Column 1.</param>
        /// <param name="m01">Row 0, Column 2.</param>
        /// <param name="m02">Row 0, Column 3.</param>
        /// <param name="m03">Row 0, Column 4.</param>
        /// <param name="m10">Row 1, Column 1.</param>
        /// <param name="m11">Row 1, Column 2.</param>
        /// <param name="m12">Row 1, Column 3.</param>
        /// <param name="m13">Row 1, Column 4.</param>
        public Matrix2x4(float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13)
        {
            Row0 = new(m00, m01, m02, m03);
            Row1 = new(m10, m11, m12, m13);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix2x4 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1;
        }

        public readonly bool Equals(Matrix2x4 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance);
        }
        #endregion
    }
}
