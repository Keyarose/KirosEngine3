using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Four by three matrix definition.
    /// </summary>
    public struct Matrix4x3
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
        /// The fourth row of the matrix.
        /// </summary>
        public Vec3 Row3;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix4x3 Zero => new Matrix4x3(Vec3.Zero, Vec3.Zero, Vec3.Zero, Vec3.Zero);

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

        /// <summary>
        /// The third column of the matrix.
        /// </summary>
        public Vec4 Column2
        {
            readonly get => new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
                Row2.Z = value.Z;
                Row3.Z = value.W;
            }
        }
        #endregion

        /// <inheritdoc/>
        public readonly Vec4[] GetColumns()
        {
            return [Column0, Column1, Column2];
        }

        /// <inheritdoc/>
        public readonly Vec3[] GetRows()
        {
            return [Row0, Row1, Row2, Row3];
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec3s.
        /// </summary>
        /// <param name="row0">Row 0.</param>
        /// <param name="row1">Row 1.</param>
        /// <param name="row2">Row 2.</param>
        /// <param name="row3">Row 3.</param>
        public Matrix4x3(Vec3 row0, Vec3 row1, Vec3 row2, Vec3 row3)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, Column 0.</param>
        /// <param name="m01">Row 0, Column 1.</param>
        /// <param name="m02">Row 0, Column 2.</param>
        /// <param name="m10">Row 1, Column 0.</param>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <param name="m12">Row 1, Column 2.</param>
        /// <param name="m20">Row 2, Column 0.</param>
        /// <param name="m21">Row 2, Column 1.</param>
        /// <param name="m22">Row 2, Column 2.</param>
        /// <param name="m30">Row 3, Column 0.</param>
        /// <param name="m31">Row 3, Column 1.</param>
        /// <param name="m32">Row 3, Column 2.</param>
        public Matrix4x3(float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22,
            float m30, float m31, float m32)
        {
            Row0 = new(m00, m01, m02);
            Row1 = new(m10, m11, m12);
            Row2 = new(m20, m21, m22);
            Row3 = new(m30, m31, m32);
        }
        #endregion

        #region Comparison
        /// <inheritdoc/>
        public readonly bool Equals(Matrix4x3 other) 
        {
            return Row0 == other.Row0 && Row1 == other.Row1 && Row2 == other.Row2 && Row3 == other.Row3;
        }

        /// <inheritdoc/>
        public readonly bool Equals(Matrix4x3 other, float tolerance)
        {
            return Row0.Equals(other.Row0, tolerance) && Row1.Equals(other.Row1, tolerance) && Row2.Equals(other.Row2, tolerance) && Row3.Equals(other.Row3, tolerance);
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
            var r2 = Row2.ToString(format, formatProvider);
            var r3 = Row3.ToString(format, formatProvider);

            return string.Format("{0}\n\t{1}\n\t{2}\n\t{3}", r0, r1, r2, r3);
        }
        #endregion
    }
}
