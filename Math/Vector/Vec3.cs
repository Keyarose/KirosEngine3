using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KirosEngine3.Math.Vector
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3 : IEquatable<Vec3>, IFormattable
    {
        public float X;
        public float Y; 
        public float Z;
        
        /// <summary>
        /// The length of the vector
        /// </summary>
        public readonly float Length
        {
            get
            {
                return MathF.Sqrt(X*X + Y*Y + Z*Z);
            }
        }

        /// <summary>
        /// The length of the vector squared, slightly faster than getting the length itself
        /// </summary>
        public readonly float LengthSqr
        {
            get
            {
                return X*X + Y*Y + Z*Z;
            }
        }

        /// <summary>
        /// Predefined 3D vector 0,0,0
        /// </summary>
        public static readonly Vec3 Zero = new Vec3(0.0f);

        /// <summary>
        /// Predefined 3D vector 1,1,1
        /// </summary>
        public static readonly Vec3 One = new Vec3(1.0f);

        /// <summary>
        /// Predefined 3D vector -1,-1,-1
        /// </summary>
        public static readonly Vec3 OneMinus = new Vec3(-1.0f);

        /// <summary>
        /// Predefined 3D vector 1,0,0
        /// </summary>
        public static readonly Vec3 UnitX = new Vec3(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Predefined 3D vector 0,1,0
        /// </summary>
        public static readonly Vec3 UnitY = new Vec3(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Predefined 3D vector 0,0,1
        /// </summary>
        public static readonly Vec3 UnitZ = new Vec3(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Size of the Vec3 struct in bytes
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<Vec3>();

        /// <summary>
        /// Index accessor for the vector
        /// </summary>
        /// <param name="index">Index that corresponds to the X,Y, or Z component</param>
        /// <returns>The value of X, Y, or Z depending on the index value</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is not a value from 0-2 inclusive</exception>
        public float this[int index]
        {
            readonly get
            {
                if (index == 0)
                {
                    return X;
                }
                else if(index == 1) 
                {
                    return Y;
                }
                else if(index == 2)
                {
                    return Z;
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec3.", index));
                }
            }
            set
            {
                if (index == 0) 
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else if (index == 2)
                {
                    Z = value;
                }
                else
                {
                    throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec3.", index));
                }
            }
        }

        #region Constructors
        /// <summary>
        /// Construct a 3D vector with identical components
        /// </summary>
        /// <param name="f">The value for each component</param>
        public Vec3(float f)
        {
            X = f;
            Y = f;
            Z = f;
        }

        /// <summary>
        /// Construct a 3D vector
        /// </summary>
        /// <param name="x">The value of the X component</param>
        /// <param name="y">The value of the Y component</param>
        /// <param name="z">The value of the Z component</param>
        public Vec3(float x, float y, float z) 
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Construct a 3D vector as a copy of another
        /// </summary>
        /// <param name="v">The vector to copy</param>
        public Vec3(Vec3 v) 
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        /// <summary>
        /// Construct a 3D vector from a 2D vector and a float
        /// </summary>
        /// <param name="v">The 2D vector to use</param>
        /// <param name="z">The float value to be the Z component</param>
        public Vec3(Vec2 v, float z)
        {
            X = v.X;
            Y = v.Y;
            Z = z;
        }
        #endregion

        #region Normalize
        /// <summary>
        /// Normalize the vector, should be checked by IsFinite afterwards
        /// </summary>
        public void Normalize()
        {
            float ratio = 1.0f / Length;

            X *= ratio;
            Y *= ratio;
            Z *= ratio;
        }

        /// <summary>
        /// Get a normalized copy of the vector, should be checked by IsFinite afterwards
        /// </summary>
        /// <returns>A normalized copy of the vector</returns>
        public readonly Vec3 NormalizedCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }

        /// <summary>
        /// Normalize the given vector, should be checked by IsFinite afterwards
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns>The normalized vector</returns>
        public static Vec3 Normalize(Vec3 v)
        {
            float ratio = 1.0f / v.Length;

            v.X *= ratio;
            v.Y *= ratio;
            v.Z *= ratio;

            return v;
        }

        /// <summary>
        /// Normalize the given vector, should be checked by IsFinite afterwards
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(Vec3 v, out Vec3 result)
        {
            float ratio = 1.0f / v.Length;

            result.X = v.X * ratio;
            result.Y = v.Y * ratio;
            result.Z = v.Z * ratio;
        }
        #endregion

        /// <summary>
        /// Check for a zero vector
        /// </summary>
        /// <returns>True if LengthSqr and thus Length is 0, false otherwise</returns>
        public readonly bool IsZero()
        {
            return LengthSqr.IsZero();
        }

        /// <summary>
        /// Check that the vector is both finite and not NaN
        /// </summary>
        /// <returns>True if X,Y and Z are finite and not NaN, false otherwise</returns>
        public readonly bool IsFinite()
        {
            return float.IsFinite(X) && float.IsFinite(Y) && float.IsFinite(Z) &&
                !(float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z));
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Vec3 vec && Equals(vec);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Vec3 other) 
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Extract the vector's components
        /// </summary>
        /// <param name="x">The vector's X component</param>
        /// <param name="y">The vector's Y component</param>
        /// <param name="z">The vector's Z component</param>
        public readonly void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector to compare</param>
        /// <param name="rhs">The right vector to compare</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(Vec3 lhs, Vec3 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector to compare</param>
        /// <param name="rhs">The right vector to compare</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(Vec3 lhs, Vec3 rhs) 
        {
            return !(lhs == rhs);
        }

        #region Add
        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Add(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <param name="result">The resulting vector</param>
        public static void Add(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result.X = v1.X + v2.X;
            result.Y = v1.Y + v2.Y;
            result.Z = v1.Z + v2.Z;
        }

        /// <summary>
        /// Define the addition operator between two vectors
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Vec3 operator +(Vec3 left, Vec3 right) 
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Subtract(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <param name="result">The resulting vector</param>
        public static void Subtract(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result.X = v1.X - v2.X;
            result.Y = v1.Y - v2.Y;
            result.Z = v1.Z - v2.Z;
        }

        /// <summary>
        /// Define the subtraction operator between two vectors
        /// </summary>
        /// <param name="left">First/left vector to subtract</param>
        /// <param name="right">Second/right vector to subtract</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator -(Vec3 left, Vec3 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        /// <summary>
        /// Invert the sign and thus direction of the vector
        /// </summary>
        /// <param name="v">The vector to invert</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator -(Vec3 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            return v;
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="v1">The vector to scale</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Multiply(Vec3 v1, float scale)
        {
            return new Vec3(v1.X * scale, v1.Y * scale, v1.Z * scale);
        }

        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="v1">The vector to scale</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec3 v1, float scale, out Vec3 result)
        {
            result.X = v1.X * scale;
            result.Y = v1.Y * scale;
            result.Z = v1.Z * scale;
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector to multiply</param>
        /// <param name="v2">Second vector to multiply</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Multiply(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector to multiply</param>
        /// <param name="v2">Second vector to multiply</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result.X = v1.X * v2.X;
            result.Y = v1.Y * v2.Y;
            result.Z = v1.Z * v2.Z;
        }

        /// <summary>
        /// Define the multiplication operator for a vector and a scalar
        /// </summary>
        /// <param name="left">The vector to multiply</param>
        /// <param name="right">The scalar to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator *(Vec3 left, float right)
        {
            left.X *= right;
            left.Y *= right;
            left.Z *= right;
            return left;
        }

        /// <summary>
        /// Define the multiplication operator for a scalar and a vector
        /// </summary>
        /// <param name="left">The scalar to multiply by</param>
        /// <param name="right">The vector to multiply</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator *(float left, Vec3 right)
        {
            right.X *= left;
            right.Y *= left;
            right.Z *= left;
            return right;
        }

        /// <summary>
        /// Define the multiplication operator for a vector component wise with another vector
        /// </summary>
        /// <param name="left">The left vector to multiply</param>
        /// <param name="right">The right vector to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator *(Vec3 left, Vec3 right) 
        {
            left.X *= right.X;
            left.Y *= right.Y;
            left.Z *= right.Z;
            return left;
        }

        /// <summary>
        /// Multiply a 3D matrix by a column vector
        /// </summary>
        /// <param name="m">The matrix</param>
        /// <param name="v">The vector</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 MultiplyC(Matrix.Matrix3 m, Vec3 v)
        {
            var r = new Vec3
            {
                X = (m.Row0.X * v.X) + (m.Row0.Y * v.X) + (m.Row0.Z * v.X),
                Y = (m.Row1.X * v.Y) + (m.Row1.Y * v.Y) + (m.Row1.Z * v.Y),
                Z = (m.Row2.X * v.Z) + (m.Row2.Y * v.Z) + (m.Row2.Z * v.Z)
            };

            return r;
        }

        /// <summary>
        /// Multiply a 3D matrix by a column vector
        /// </summary>
        /// <param name="m">The matrix</param>
        /// <param name="v">The vector</param>
        /// <param name="result">The resulting vector</param>
        public static void MultiplyC(Matrix.Matrix3 m, Vec3 v, out Vec3 result)
        {
            result = MultiplyC(m, v);
        }

        /// <summary>
        /// Define the multiplication operator for a 3D matrix times a column vector3
        /// </summary>
        /// <param name="left">The matrix on the left</param>
        /// <param name="right">The vector on the right</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator *(Matrix.Matrix3 left, Vec3 right)
        {
            return MultiplyC(left, right);
        }

        /// <summary>
        /// Multiply a row vector by a 3D matrix
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="m">The matrix</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 MultiplyR(Vec3 v, Matrix.Matrix3 m)
        {
            var r = new Vec3
            {
                X = (m.Row0.X * v.X) + (m.Row1.X * v.X) + (m.Row2.X * v.X),
                Y = (m.Row0.Y * v.Y) + (m.Row1.Y * v.Y) + (m.Row2.Y * v.Y),
                Z = (m.Row0.Z * v.Z) + (m.Row1.Z * v.Z) + (m.Row2.Z * v.Z)
            };

            return r;
        }

        /// <summary>
        /// Multiply a row vector by a 3D matrix
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="m">The matrix</param>
        /// <param name="result">The resulting vector</param>
        public static void MultiplyR(Vec3 v, Matrix.Matrix3 m, out Vec3 result)
        {
            result = MultiplyR(v, m);
        }

        /// <summary>
        /// Define the multiplication operator for a row vector3 times a 3D matrix
        /// </summary>
        /// <param name="v">The vector</param>
        /// <param name="m">The matrix</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator *(Vec3 v, Matrix.Matrix3 m)
        {
            return MultiplyR(v, m);
        }

        //todo: matrix4 * vector, quaternion * vector
        #endregion

        #region Divide
        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Divide(Vec3 v, float divisor)
        {
            if(divisor.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            return new Vec3(v.X / divisor, v.Y / divisor, v.Z / divisor);
        }

        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec3 v, float divisor, out Vec3 result)
        {
            if (divisor.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            result.X = v.X / divisor;
            result.Y = v.Y / divisor;
            result.Z = v.Z / divisor;
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 Divide(Vec3 v1, Vec3 v2)
        {
            if (v2.X.IsZero() || v2.Y.IsZero() || v2.Z.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            return new Vec3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec3 v1, Vec3 v2,  out Vec3 result)
        {
            if (v2.X.IsZero() || v2.Y.IsZero() || v2.Z.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            result.X = v1.X / v2.X;
            result.Y = v1.Y / v2.Y;
            result.Z = v1.Z / v2.Z;
        }

        /// <summary>
        /// Define the division operator for a vector and a scalar
        /// </summary>
        /// <param name="left">The vector to be divided</param>
        /// <param name="right">The scalar to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator /(Vec3 left, float right)
        {
            if (right.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            left.X /= right;
            left.Y /= right;
            left.Z /= right;
            return left;
        }

        /// <summary>
        /// Define the division operator for a vector component wise by a vector
        /// </summary>
        /// <param name="left">The vector to be divided</param>
        /// <param name="right">The vector to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec3 operator /(Vec3 left, Vec3 right)
        {
            if (right.X.IsZero() || right.Y.IsZero() || right.Z.IsZero())
            {
                Console.WriteLine("Division by zero in Vec3.Divide()");
                Logger.WriteToLog("Division by zero in Vec3.Divide()");
            }
            left.X /= right.X;
            left.Y /= right.Y;
            left.Z /= right.Z;
            return left;
        }
        #endregion

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The dot product of the vectors</returns>
        public static float Dot(Vec3 v1, Vec3 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="result">The dot product of the vectors</param>
        public static void Dot(Vec3 v1, Vec3 v2, out float result)
        {
            result = (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Get the cross product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The cross product vector of the vectors</returns>
        public static Vec3 Cross(Vec3 v1, Vec3 v2)
        {
            return new Vec3((v1.Y * v2.Z) - (v1.Z * v2.Y),
                (v1.Z * v2.X) - (v1.X * v2.Z),
                (v1.X * v2.Y) - (v1.Y * v2.X));
        }

        /// <summary>
        /// Get the cross product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="result">The cross product vector of the vectors</param>
        public static void Cross(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result.X = (v1.Y * v2.Z) - (v1.Z * v2.Y);
            result.Y = (v1.Z * v2.X) - (v1.X * v2.Z);
            result.Z = (v1.X * v2.Y) - (v1.Y * v2.X);
        }

        #region MinMax
        /// <summary>
        /// Create a vector from the smallest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the smallest components from the input</returns>
        public static Vec3 ComponentMin(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X < v2.X ? v1.X : v2.X,
                v1.Y < v2.Y ? v1.Y : v2.Y,
                v1.Z < v2.Z ? v1.Z : v2.Z);
        }

        /// <summary>
        /// Create a vector from the smallest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the smallest components from the input</param>
        public static void ComponentMin(Vec3 v1, Vec3 v2, Vec3 result)
        {
            result.X = v1.X < v2.X ? v1.X : v2.X;
            result.Y = v1.Y < v2.Y ? v1.Y : v2.Y;
            result.Z = v1.Z < v2.Z ? v1.Z : v2.Z;
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the largest components from the inputs</returns>
        public static Vec3 ComponentMax(Vec3 v1, Vec3 v2)
        {
            return new Vec3(v1.X > v2.X ? v1.X : v2.X,
                v1.Y > v2.Y ? v1.Y : v2.Y,
                v1.Z > v2.Z ? v1.Z : v2.Z);
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the largest components from the inputs</param>
        public static void ComponentMax(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result.X = v1.X > v2.X ? v1.X : v2.X;
            result.Y = v1.Y > v2.Y ? v1.Y : v2.Y;
            result.Z = v1.Z > v2.Z ? v1.Z : v2.Z;
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the smallest magnitude</returns>
        public static Vec3 MagnitudeMin(Vec3 v1, Vec3 v2)
        {
            return v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the smallest magnitude</param>
        public static void MagnitudeMin(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result = v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the largest magnitude</returns>
        public static Vec3 MagnitudeMax(Vec3 v1, Vec3 v2)
        {
            return v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the largest magnitude</param>
        public static void MagnitudeMax(Vec3 v1, Vec3 v2, out Vec3 result)
        {
            result = v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Clamps a vector's components between a max and min
        /// </summary>
        /// <param name="v">The vector to clamp</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>The clamped vector</returns>
        public static Vec3 Clamp(Vec3 v, Vec3 min, Vec3 max)
        {
            float x = v.X < min.X ? min.X : (v.X > max.X ? max.X : v.X);
            float y = v.Y < min.Y ? min.Y : (v.Y > max.Y ? max.Y : v.Y);
            float z = v.Z < min.Z ? min.Z : (v.Z > max.Z ? max.Z : v.Z);

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Clamps a vector's components between a max and a min
        /// </summary>
        /// <param name="v">The vector to clamp</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The max vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(Vec3 v, Vec3 min, Vec3 max, out Vec3 result)
        {
            result.X = v.X < min.X ? min.X : (v.X > max.X ? max.X : v.X);
            result.Y = v.Y < min.Y ? min.Y : (v.Y > max.Y ? max.Y : v.Y);
            result.Z = v.Z < min.Z ? min.Z : (v.Z > max.Z ? max.Z : v.Z);
        }
        #endregion

        /// <summary>
        /// Calculate the distance between two points as represented by vectors
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <returns>The straight line distance between two points</returns>
        public static float Distance(Vec3 v1, Vec3 v2)
        {
            return MathF.Sqrt(((v2.X - v1.X) * (v2.X - v1.X)) + ((v2.Y - v1.Y) * (v2.Y - v1.Y)) + ((v2.Z - v1.Z) * (v2.Z - v1.Z)));
        }

        /// <summary>
        /// Calculate the distance between two points as represented by vectors
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="result">The straight line distance between two points</param>
        public static void Distance(Vec3 v1, Vec3 v2, out float result)
        {
            result = MathF.Sqrt(((v2.X - v1.X) * (v2.X - v1.X)) + ((v2.Y - v1.Y) * (v2.Y - v1.Y)) + ((v2.Z - v1.Z) * (v2.Z - v1.Z)));
        }

        /// <summary>
        /// Calculate the distance between two points as represented by vectors squared
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <returns>The straight line distance between two points squared</returns>
        public static float DistanceSqu(Vec3 v1, Vec3 v2)
        {
            return ((v2.X - v1.X) * (v2.X - v1.Y)) + ((v2.Y - v1.Y) * (v2.Y - v1.Y)) + ((v2.Z - v1.Z) * (v2.Z - v1.Z));
        }

        /// <summary>
        /// Calculate the distance between two points as represented by vectors squared
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="result">The straight line distance between two points squared</param>
        public static void DistanceSqu(Vec3 v1, Vec3 v2, out float result)
        {
            result = ((v2.X - v1.X) * (v2.X - v1.Y)) + ((v2.Y - v1.Y) * (v2.Y - v1.Y)) + ((v2.Z - v1.Z) * (v2.Z - v1.Z));
        }

        #region Lerp
        /// <summary>
        /// Linear interpolation between two points as represented by vectors
        /// </summary>
        /// <param name="v1">The starting point</param>
        /// <param name="v2">The ending point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <returns>The point along the interpolation for the value of t</returns>
        public static Vec3 Lerp(Vec3 v1, Vec3 v2, float t)
        {
            float x = v1.X + (t * (v2.X - v1.X));
            float y = v1.Y + (t * (v2.Y - v1.Y));
            float z = v1.Z + (t * (v2.Z - v1.Z));

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Linear interpolation between two points as represented by vectors
        /// </summary>
        /// <param name="v1">The starting point</param>
        /// <param name="v2">The ending point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <param name="result">The point along the interpolation for the value of t</param>
        public static void Lerp(Vec3 v1, Vec3 v2, float t, out Vec3 result)
        {
            result.X = v1.X + (t * (v2.X - v1.X));
            result.Y = v1.Y + (t * (v2.Y - v1.Y));
            result.Z = v1.Z + (t * (v2.Z - v1.Z));
        }

        /// <summary>
        /// Linear interpolation between two points on a component wise basis
        /// </summary>
        /// <param name="v1">The starting point</param>
        /// <param name="v2">The ending point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <returns>The point along the interpolation for the value of t component wise</returns>
        public static Vec3 Lerp(Vec3 v1, Vec3 v2, Vec3 t)
        {
            float x = v1.X + (t.X * (v2.X - v1.X));
            float y = v1.Y + (t.Y * (v2.Y - v1.Y));
            float z = v1.Z + (t.Z * (v2.Z - v1.Z));

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Linear interpolation between two points on a component wise basis
        /// </summary>
        /// <param name="v1">The starting point</param>
        /// <param name="v2">The ending point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <param name="result">The point along the interpolation for the value of t component wise</param>
        public static void Lerp(Vec3 v1, Vec3 v2, Vec3 t, out Vec3 result)
        {
            result.X = v1.X + (t.X * (v2.X - v1.X));
            result.Y = v1.Y + (t.Y * (v2.Y - v1.Y));
            result.Z = v1.Z + (t.Z * (v2.Z - v1.Z));
        }

        /// <summary>
        /// Interpolate between 3 vectors using barycentric coords
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <param name="u">First barycentric coordinate</param>
        /// <param name="v">Second barycentric coordinate</param>
        /// <returns>v1 where u=v=0, v2 where u=1, v=0, v3 where u=0, v=1, otherwise a linear interpolation of v1,v2,v3</returns>
        public static Vec3 BarycentricInterp(Vec3 v1, Vec3 v2, Vec3 v3, float u, float v)
        {
            return v1 + ((v2 - v1) * u) + ((v3 - v1) * v);
        }

        /// <summary>
        /// Interpolate between 3 vectors using barycentric coords
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <param name="u">First barycentric coordinate</param>
        /// <param name="v">Second barycentric coordinate</param>
        /// <param name="result">v1 where u=v=0, v2 where u=1, v=0, v3 where u=0, v=1, otherwise a linear interpolation of v1,v2,v3</param>
        public static void BarycentricInterp(Vec3 v1, Vec3 v2, Vec3 v3, float u, float v, out Vec3 result)
        {
            result = v1 + ((v2 - v1) * u) + ((v3 - v1) * v);
        }
        #endregion
        
        //todo: transform methods

        /// <summary>
        /// Calculate the angle between two vectors in radians
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The angle between the two in radians, never larger than Pi</returns>
        public static float CalcAngle(Vec3 v1, Vec3 v2)
        {
            float dot = Dot(v1, v2);
            return MathF.Acos(float.Clamp(dot / (v1.Length * v2.Length), -1.0f, 1.0f));
        }

        /// <summary>
        /// Calculate the angle between two vectors in radians
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="result">The angle between the two in radians, never larger than Pi</param>
        public static void CalcAngle(Vec3 v1, Vec3 v2, out float result)
        {
            float dot = Dot(v1, v2);
            result = MathF.Acos(float.Clamp(dot / (v1.Length * v2.Length), -1.0f, 1.0f));
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
            return string.Format("({0},{1},{2})", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
        }
        #endregion

        #region ComponentAccessors
        /// <summary>
        /// Get a vec2 with this vector's x and y or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Xy
        {
            get { return new Vec2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's x and z or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Xz
        {
            get { return new Vec2(X, Z); }
            set { X = value.X; Z = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's y and x or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Yx
        {
            get { return new Vec2(Y, X); }
            set { Y = value.X; X = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's y and z or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Yz
        {
            get { return new Vec2(Y, Z); }
            set { Y = value.X; Z = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's z and x or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Zx
        {
            get { return new Vec2(Z, X); }
            set { Z = value.X; X = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's z and y or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Zy
        {
            get { return new Vec2(Z, Y); }
            set { Z = value.X; Y = value.Y; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as X, Z, Y or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Xzy
        {
            get { return new Vec3(X, Z, Y); }
            set { X = value.X; Z = value.Y; Y = value.Z; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as Y, X, Z or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Yxz
        {
            get { return new Vec3(Y, X, Z); }
            set { Y = value.X; X = value.Y; Z = value.Z; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as Y, Z, X or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Yzx
        {
            get { return new Vec3(Y, Z, X); }
            set { Y = value.X; Z = value.Y; X = value.Z; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as Z, X, Y or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Zxy
        {
            get { return new Vec3(Z, X, Y); }
            set { Z = value.X; X = value.Y; Y = value.Z; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as Z, Y, X or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Zyx
        {
            get { return new Vec3(Z, Y, X); }
            set { Z = value.X; Y = value.Y; X = value.Z; }
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Vector3 to Vec3
        /// </summary>
        /// <param name="v">The vector to convert</param>
        public static implicit operator Vec3(Vector3 v)
        {
            return new Vec3(v.X, v.Y, v.Z);
        }

        /// <summary>
        /// Handle conversion from Vec3 to OpenTK's Vector3
        /// </summary>
        /// <param name="v">The vector to convert</param>
        public static implicit operator Vector3(Vec3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        #endregion
#endif
    }
}
