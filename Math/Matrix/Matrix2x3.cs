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
    /// Two by three matrix definition.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix2x3
    {
        public Vec3 Row0;
        public Vec3 Row1;

        /// <summary>
        /// The zero matrix.
        /// </summary>
        public static Matrix2x3 Zero => new Matrix2x3(Vec3.Zero, Vec3.Zero);

        /// <summary>
        /// The first column of the matrix.
        /// </summary>
        public Vec2 Column0
        {
            readonly get => new(Row0.X, Row1.X);
            set
            {
                Row0.X = value.X;
                Row1.X = value.Y;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec2 Column1
        {
            readonly get => new(Row0.Y, Row1.Y);
            set
            {
                Row0.Y = value.X;
                Row1.Y = value.Y;
            }
        }

        /// <summary>
        /// The second column of the matrix.
        /// </summary>
        public Vec2 Column2
        {
            readonly get => new(Row0.Z, Row1.Z);
            set
            {
                Row0.Z = value.X;
                Row1.Z = value.Y;
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
        #endregion

        #region Constructors
        /// <summary>
        /// Basic constructor using Vec3s.
        /// </summary>
        /// <param name="r0">Row 0.</param>
        /// <param name="r1">Row 1.</param>
        public Matrix2x3(Vec3 r0, Vec3 r1)
        {
            Row0 = r0;
            Row1 = r1;
        }

        /// <summary>
        /// Basic constructor using floats.
        /// </summary>
        /// <param name="m00">Row 0, column 0.</param>
        /// <param name="m01">Row 0, column 1.</param>
        /// <param name="m02">Row 0, column 2.</param>
        /// <param name="m10">Row 1, column 0.</param>
        /// <param name="m11">Row 1, column 1.</param>
        /// <param name="m12">Row 1, column 2.</param>
        public Matrix2x3(float m00, float m01, float m02, float m10, float m11, float m12) 
        {
            Row0 = new(m00, m01, m02);
            Row1 = new(m10, m11, m12);
        }
        #endregion
    }
}
