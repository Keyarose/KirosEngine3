using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Four by two matrix definition.
    /// </summary>
    public struct Matrix4x2
    {
        public Vec2 Row0;
        public Vec2 Row1;
        public Vec2 Row2;
        public Vec2 Row3;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix4x2 Zero => new Matrix4x2(Vec2.Zero, Vec2.Zero, Vec2.Zero, Vec2.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix.
        /// </summary>
        public Vec4 Column0
        {
            readonly get => new Vec4(Row0.X, Row1.X, Row2.X, Row3.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
                Row2.X = value.Z;
                Row3.X = value.W;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec4 Column1
        {
            readonly get => new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
                Row2.Y = value.Z;
                Row3.Y = value.W;
            }
        }
        #endregion

        public readonly Vec4[] GetColumns()
        {
            return [Column0, Column1];
        }

        public readonly Vec2[] GetRows()
        {
            return [Row0, Row1, Row2, Row3];
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec2s.
        /// </summary>
        /// <param name="row0">Row 0.</param>
        /// <param name="row1">Row 1.</param>
        /// <param name="row2">Row 2.</param>
        /// <param name="row3">Row 3.</param>
        public Matrix4x2(Vec2 row0, Vec2 row1, Vec2 row2, Vec2 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Basic constructor using floats
        /// </summary>
        /// <param name="m00">Row 0, Column 0.</param>
        /// <param name="m01">Row 0, Column 1.</param>
        /// <param name="m10">Row 1, Column 0.</param>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <param name="m20">Row 2, Column 0.</param>
        /// <param name="m21">Row 2, Column 1.</param>
        /// <param name="m30">Row 3, Column 0.</param>
        /// <param name="m31">Row 3, Column 1.</param>
        public Matrix4x2(float m00, float m01,
            float m10, float m11,
            float m20, float m21,
            float m30, float m31)
        {
            Row0 = new(m00, m01);
            Row1 = new(m10, m11);
            Row2 = new(m20, m21);
            Row3 = new(m30, m31);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix4x2 other)
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2 && Row3 == other.Row3;
        }

        public readonly bool Equals(Matrix4x2 other, float tolerance) 
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance) && Row2.Equals(other.Row2, tolerance) && Row3.Equals(other.Row3, tolerance);
        }
        #endregion
    }
}
