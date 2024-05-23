using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Matrix
{
    /// <summary>
    /// Three by four matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix3x4
    {
        public Vec4 Row0;
        public Vec4 Row1;
        public Vec4 Row2;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix3x4 Zero => new Matrix3x4(Vec4.Zero, Vec4.Zero, Vec4.Zero);

        #region Columns
        /// <summary>
        /// The first column of the matrix.
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
        /// The second column of the matrix.
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
        /// The third column of the matrix.
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

        /// <summary>
        /// The fourth column of the matrix.
        /// </summary>
        public Vec3 Column3
        {
            readonly get => new Vec3(Row0.W, Row1.W, Row2.W);
            set
            {
                Row0.W = value.X;
                Row1.W = value.Y;
                Row2.W = value.Z;
            }
        }
        #endregion

        public readonly Vec3[] GetColumns()
        {
            return [Column0, Column1, Column2, Column3];
        }

        public readonly Vec4[] GetRows()
        {
            return [Row0, Row1, Row2];
        }

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec4s.
        /// </summary>
        /// <param name="row0">Row 0.</param>
        /// <param name="row1">Row 1.</param>
        /// <param name="row2">Row 2.</param>
        public Matrix3x4(Vec4 row0, Vec4 row1, Vec4 row2)
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, Column 0.</param>
        /// <param name="m01">Row 0, Column 1.</param>
        /// <param name="m02">Row 0, Column 2.</param>
        /// <param name="m03">Row 0, Column 3.</param>
        /// <param name="m10">Row 1, Column 0.</param>
        /// <param name="m11">Row 1, Column 1.</param>
        /// <param name="m12">Row 1, Column 2.</param>
        /// <param name="m13">Row 1, Column 3.</param>
        /// <param name="m20">Row 2, Column 0.</param>
        /// <param name="m21">Row 2, Column 1.</param>
        /// <param name="m22">Row 2, Column 2.</param>
        /// <param name="m23">Row 2, Column 3.</param>
        public Matrix3x4(float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
        }
        #endregion
    }
}
