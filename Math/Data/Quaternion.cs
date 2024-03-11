using KirosEngine3.Math.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KirosEngine3.Math.Data
{
    /// <summary>
    /// Defines a Quaternion.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion : IEquatable<Quaternion>, IFormattable
    {
        /// <summary>
        /// The W component
        /// </summary>
        public float W;

        /// <summary>
        /// The Xyz Axis component
        /// </summary>
        public Vec3 Axis;

        /// <summary>
        /// Accessor for the Quaternion's X value
        /// </summary>
        [XmlIgnore]
        public float X
        {
            readonly get { return Axis.X; }
            set { Axis.X = value; }
        }

        /// <summary>
        /// Accessor for the Quaternion's Y value
        /// </summary>
        [XmlIgnore]
        public float Y
        {
            readonly get { return Axis.Y; }
            set { Axis.Y = value; }
        }

        /// <summary>
        /// Accessor for the Quaternion's Z value
        /// </summary>
        [XmlIgnore]
        public float Z
        {
            readonly get { return Axis.Z; }
            set { Axis.Z = value; }
        }

        public static readonly Quaternion Identity = new Quaternion(1, 0, 0, 0);

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="w">The w component</param>
        /// <param name="axis">The axis component</param>
        public Quaternion(float w, Vec3 axis)
        {
            W = w;
            Axis = axis;
        }

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="w">The w component</param>
        /// <param name="x">The axis' x component</param>
        /// <param name="y">The axis' y component</param>
        /// <param name="z">The axis' z component</param>
        public Quaternion(float w, float x, float y, float z) : this(w, new Vec3(x, y, z))
        {
        }

        /// <summary>
        /// Construct a Quaternion from Euler angles given in radians.
        /// </summary>
        /// <param name="rotX">Counterclockwise rotation around the X axis in radians</param>
        /// <param name="rotY">Counterclockwise rotation around the Y axis in radians</param>
        /// <param name="rotZ">Counterclockwise rotation around the Z axis in radians</param>
        public Quaternion(float rotX, float rotY, float rotZ)
        {
            rotX *= 0.5f;
            rotY *= 0.5f;
            rotZ *= 0.5f;

            var cX = MathF.Cos(rotX);
            var cY = MathF.Cos(rotY);
            var cZ = MathF.Cos(rotZ);
            var sX = MathF.Sin(rotX);
            var sY = MathF.Sin(rotY);
            var sZ = MathF.Sin(rotZ);

            W = (cX * cY * cZ) - (sX * sY * sZ);
            Axis.X = (sX * cY * cZ) + (cX * sY * sZ);
            Axis.Y = (cX * sY * cZ) - (sX * cY * sZ);
            Axis.Z = (cX * cY * sZ) + (sX * sY * cZ);
        }

        /// <summary>
        /// Construct a Quaternion from Euler angles given in radians
        /// </summary>
        /// <param name="angles">Counterclockwise angles stored in a Vec3</param>
        public Quaternion(Vec3 angles) : this(angles.X, angles.Y, angles.Z)
        { 
        }

        /// <summary>
        /// Normalize the quaternion
        /// </summary>
        public void Normalize()
        {
            var scale = 1.0f / Magnitude;
            Axis *= scale;
            W *= scale;
        }

        /// <summary>
        /// Create a normalized copy of the Quaternion
        /// </summary>
        /// <returns>The resulting normalized quaternion</returns>
        public readonly Quaternion NormalizedCopy()
        {
            var c = this;
            c.Normalize();

            return c;
        }

        /// <summary>
        /// Normalize the quaternion
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>The resulting quaternion</returns>
        public static Quaternion Normalize(Quaternion q)
        {
            return q.NormalizedCopy();
        }

        /// <summary>
        /// Normalize the quaternion
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <param name="result">The resulting quaternion</param>
        public static void Normalize(Quaternion q, out Quaternion result)
        {
            result = q.NormalizedCopy();
        }

        /// <summary>
        /// The magnitude of the quaternion
        /// </summary>
        public readonly float Magnitude
        {
            get
            {
                return MathF.Sqrt((W * W) + Xyz.Length);
            }
        }

        /// <summary>
        /// The Length of the quaternion, alias for Magnitude
        /// </summary>
        public readonly float Length
        {
            get { return Magnitude; }
        }

        /// <summary>
        /// The magnitude of the quaternion squared
        /// </summary>
        public readonly float MagnitudeSquared
        {
            get
            {
                return (W * W) + Xyz.LengthSqr;
            }
        }

        /// <summary>
        /// The length of the quaternion squared, alias for Magnitude
        /// </summary>
        public readonly float LengthSquared
        {
            get { return MagnitudeSquared; }
        }

        #region Conjugate
        /// <summary>
        /// Get the Quaternion's conjugate by reversing the axis
        /// </summary>
        public void Conjugate()
        {
            Axis = -Axis;
        }

        /// <summary>
        /// Get the Quaternion's conjugate by reversing the axis
        /// </summary>
        /// <param name="q">The quaternion to get the Conjugate from</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion Conjugate(Quaternion q)
        {
            var r = q;
            r.Conjugate();
            return r;
        }

        /// <summary>
        /// Get the Quaternion's conjugate by reversing the axis
        /// </summary>
        /// <param name="q">The quaternion to get the conjugate from</param>
        /// <param name="result">The resulting Quaternion</param>
        public static void Conjugate(Quaternion q, out Quaternion result)
        {
            var r = q;
            r.Conjugate();
            result = r;
        }
        #endregion

        /// <summary>
        /// Get the inverse of the given Quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion Invert(Quaternion q)
        {
            var lengthSq = q.LengthSquared;

            if (lengthSq != 0.0f)
            {
                var i = 1.0f / lengthSq;
                return new Quaternion(q.W * i, q.Axis * -i);
            }
            else
            {
                return q;
            }
        }

        /// <summary>
        /// Get the inverse of the given Quaternion
        /// </summary>
        /// <param name="q">The quaternion to invert</param>
        /// <param name="result">The resulting Quaternion</param>
        public static void Invert(Quaternion q, out Quaternion result)
        {
            result = Invert(q);
        }

        #region Add
        /// <summary>
        /// Add two Quaternions together
        /// </summary>
        /// <param name="q1">The first to add</param>
        /// <param name="q2">The second to add</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion Add(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.W + q2.W, q1.Axis + q2.Axis);
        }

        /// <summary>
        /// Add two Quaternions together
        /// </summary>
        /// <param name="q1">The first to add</param>
        /// <param name="q2">The second to add</param>
        /// <param name="result">The resulting Quaternion</param>
        public static void Add(Quaternion q1, Quaternion q2, out Quaternion result)
        {
            result = Add(q1, q2);
        }

        /// <summary>
        /// Define the addition operator between two Quaternions
        /// </summary>
        /// <param name="lhs">The left Quaternion</param>
        /// <param name="rhs">The right Quaternion</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion operator +(Quaternion lhs, Quaternion rhs)
        {
            return Add(lhs, rhs);
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract one Quaternion from another
        /// </summary>
        /// <param name="q1">The quaternion to subtract from</param>
        /// <param name="q2">The quaternion to subtract</param>
        /// <returns>The resulting quaternion</returns>
        public static Quaternion Subtract(Quaternion q1, Quaternion q2)
        {
            return new Quaternion(q1.W - q2.W, q1.Axis - q2.Axis);
        }

        /// <summary>
        /// Subtract one Quaternion from another
        /// </summary>
        /// <param name="q1">The quaternion to subtract from</param>
        /// <param name="q2">The quaternion to subtract</param>
        /// <param name="result">The resulting quaternion</param>
        public static void Subtract(Quaternion q1, Quaternion q2, out Quaternion result)
        {
            result = Subtract(q1, q2);
        }

        /// <summary>
        /// Define the subtraction operator between two quaternions
        /// </summary>
        /// <param name="lhs">The left quaternion</param>
        /// <param name="rhs">The right quaternion</param>
        /// <returns>The resulting quaternion</returns>
        public static Quaternion operator -(Quaternion lhs, Quaternion rhs)
        {
            return Subtract(lhs, rhs);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply two Quaternions
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second</param>
        /// <returns>The resulting quaternion</returns>
        public static Quaternion Multiply(Quaternion q1, Quaternion q2)
        {
            return new Quaternion((q1.W * q2.W) - Vec3.Dot(q1.Axis, q2.Axis), 
                (q2.W * q1.Axis) + (q1.W * q2.Axis) + Vec3.Cross(q1.Axis, q2.Axis));
        }

        /// <summary>
        /// Multiply two Quaternions
        /// </summary>
        /// <param name="q1">The first quaternion</param>
        /// <param name="q2">The second</param>
        /// <param name="result">The resulting quaternion</param>
        public static void Multiply(Quaternion q1, Quaternion q2, out Quaternion result)
        {
            result = Multiply(q1, q2);
        }

        /// <summary>
        /// Define the multiplication operator between two Quaternions
        /// </summary>
        /// <param name="lhs">The left Quaternion</param>
        /// <param name="rhs">The right Quaternion</param>
        /// <returns>The resulting quaternion</returns>
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Multiply a quaternion by a scalar
        /// </summary>
        /// <param name="q">The Quaternion to multiply</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion Multiply(Quaternion q, float scale)
        {
            return new Quaternion(q.W * scale, q.Axis * scale);
        }

        /// <summary>
        /// Multiply a quaternion by a scalar
        /// </summary>
        /// <param name="q">The Quaternion to multiply</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <param name="result">The resulting Quaternion</param>
        public static void Multiply(Quaternion q, float scale, out Quaternion result)
        {
            result = Multiply(q, scale);
        }

        /// <summary>
        /// Define the multiplication operator between a quaternion and a scalar
        /// </summary>
        /// <param name="lhs">The quaternion</param>
        /// <param name="rhs">The scalar</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion operator *(Quaternion lhs, float rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Define the multiplication operator between a quaternion and a scalar
        /// </summary>
        /// <param name="lhs">The scalar</param>
        /// <param name="rhs">The quaternion</param>
        /// <returns>The resulting Quaternion</returns>
        public static Quaternion operator *(float lhs, Quaternion rhs)
        {
            return Multiply(rhs, lhs);
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Convert the Quaternion to an Axis-angle representation
        /// </summary>
        /// <returns>The axis-angle as a Vec4</returns>
        public Vec4 ToAxisAngle()
        {
            var q = this;
            if(MathF.Abs(q.W) > 1.0f)
            {
                q.Normalize();
            }

            var r = new Vec4
            {
                W = 2.0f * MathF.Acos(q.W) //calc angle
            };

            var denominator = MathF.Sqrt(1.0f - (q.W * q.W));
            if(denominator > 0.0001f)
            {
                r.Xyz = q.Xyz / denominator;
            }
            else
            {
                //zero angle
                r.Xyz = Vec3.UnitX;
            }

            return r;
        }

        /// <summary>
        /// Convert the Quaternion to Axis-angle representation 
        /// </summary>
        /// <param name="axis">The axis value</param>
        /// <param name="angle">The angle value</param>
        public void ToAxisAngle(out Vec3 axis, out float angle)
        {
            var r = ToAxisAngle();
            axis = r.Xyz;
            angle = r.W;
        }

        /// <summary>
        /// Convert the Quaternion to a set of Euler angles
        /// </summary>
        /// <returns>The resulting Euler angles</returns>
        public readonly Vec3 ToEulerAngles()
        {
            var q = this;

            //used to check for +-90 deg in Y axis
            const float SINGULARTIY_THRESHOLD = 0.4999995f;

            var wsqr = q.W * q.W;
            var xsqr = q.X * q.X;
            var ysqr = q.Y * q.Y;
            var zsqr = q.Z * q.Z;
            var unit = xsqr + ysqr + zsqr + wsqr; //one if the quat is normalized, used as a correction factor

            var singularityTest = (q.X * q.Z) + (q.Y * q.W);

            Vec3 angles = new Vec3();

            if(singularityTest > SINGULARTIY_THRESHOLD * unit)
            {
                //+90 deg pitch/y
                angles.Z = 2 * MathF.Atan2(q.X, q.W);
                angles.Y = MathHelpers.PiOver2;
                angles.X = 0;
            }
            else if (singularityTest < -SINGULARTIY_THRESHOLD * unit)
            {
                //-90 deg pitch/y
                angles.Z = -2 * MathF.Atan2(q.X, q.W);
                angles.Y = -MathHelpers.PiOver2;
                angles.X = 0;
            }
            else
            {
                angles.Z = MathF.Atan2(2 * ((q.W * q.Z) - (q.X * q.Y)), wsqr + xsqr - ysqr - zsqr);
                angles.Y = MathF.Asin(2 * singularityTest / unit);
                angles.X = MathF.Atan2(2 * ((q.W * q.X) - (q.Y * q.Z)), wsqr - xsqr - ysqr + zsqr);
            }

            return angles;
        }

        /// <summary>
        /// Construct a Quaternion from the given axis and angle in radians
        /// </summary>
        /// <param name="axis">The axis of rotation</param>
        /// <param name="angle">The rotation angle in radians</param>
        /// <returns>The equivalent quaternion</returns>
        public static Quaternion FromAxisAngle(Vec3 axis, float angle)
        {
            if (axis.LengthSqr.IsZero())
            {
                return Identity;
            }

            var result = Identity;

            angle *= 0.5f;
            axis.Normalize();
            result.Axis = axis * MathF.Sin(angle);
            result.W = MathF.Cos(angle);

            return Normalize(result);
        }

        //todo: fromEuler, fromMatrix, Slerp
        #endregion

        /// <summary>
        /// Define the equality operator between two Quaternions
        /// </summary>
        /// <param name="lhs">The left quaternion</param>
        /// <param name="rhs">The right quaternion</param>
        /// <returns>The resulting quaternion</returns>
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Define the non-equality operator between two Quaternions
        /// </summary>
        /// <param name="lhs">The left quaternion</param>
        /// <param name="rhs">The right quaternion</param>
        /// <returns>The resulting quaternion</returns>
        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Quaternion q && Equals(q);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Quaternion other)
        {
            return Axis == other.Axis && W == other.W;
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(W, Axis);
        }

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
            return string.Format("(({0}), ({1}, {2}, {3}))", W.ToString(format, formatProvider), Axis.X.ToString(format, formatProvider), Axis.Y.ToString(format, formatProvider), Axis.Z.ToString(format, formatProvider));
        }
        #endregion

        #region ComponentAccessors
        /// <summary>
        /// Alias for the axis vector
        /// </summary>
        [XmlIgnore]
        public Vec3 Xyz
        {
            readonly get { return Axis; }
            set { Axis = value; }
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from Quaternion to OpenTK's Quaternion
        /// </summary>
        /// <param name="q">The quaternion to convert</param>
        public static implicit operator OpenTK.Mathematics.Quaternion(Quaternion q) 
        {
            return new OpenTK.Mathematics.Quaternion(q.Axis, q.W);
        }

        /// <summary>
        /// Handle conversion from OpenTK's Quaternion to Quaternion
        /// </summary>
        /// <param name="q">The quaternion to convert</param>
        public static implicit operator Quaternion(OpenTK.Mathematics.Quaternion q)
        {
            return new Quaternion(q.W, q.Xyz);
        }
        #endregion
#endif
    }
}
