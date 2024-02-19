using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public struct Vec4 : IEquatable<Vec4>, IFormattable
    {
        public float X, Y, Z, W;

        /// <summary>
        /// The length of the vector
        /// </summary>
        public readonly float Length
        {
            get 
            {
                return MathF.Sqrt(X*X + Y*Y + Z*Z + W*W);
            }
        }

        /// <summary>
        /// The length of the vector squared, slightly faster than getting the length directly
        /// </summary>
        public readonly float LengthSqr
        {
            get 
            {
                return X*X + Y*Y + Z*Z + W*W;
            }
        }

        /// <summary>
        /// Predefined 4D vector 0,0,0,0
        /// </summary>
        public static readonly Vec4 Zero = new Vec4(0.0f);

        /// <summary>
        /// Predefined 4D vector 1,1,1,1
        /// </summary>
        public static readonly Vec4 One = new Vec4(1.0f);

        /// <summary>
        /// Predefined 4D vector -1,-1,-1,-1
        /// </summary>
        public static readonly Vec4 OneMinus = new Vec4(-1.0f);

        /// <summary>
        /// Predefined 4D vector 1,0,0,0
        /// </summary>
        public static readonly Vec4 UnitX = new Vec4(1.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// Predefined 4D vector 0,1,0,0
        /// </summary>
        public static readonly Vec4 UnitY = new Vec4(0.0f, 1.0f, 0.0f, 0.0f);

        /// <summary>
        /// Predefined 4D vector 0,0,1,0
        /// </summary>
        public static readonly Vec4 UnitZ = new Vec4(0.0f, 0.0f, 1.0f, 0.0f);

        /// <summary>
        /// Predefined 4D vector 0,0,0,1
        /// </summary>
        public static readonly Vec4 UnitW = new Vec4(0.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Size of the Vec4 struct in bytes
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<Vec4>();

        /// <summary>
        /// Index accessor for the vector
        /// </summary>
        /// <param name="index">Index that corresponds to the X,Y,Z, or W component</param>
        /// <returns>The value of the index specified component</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is not a value from 0-3 inclusive</exception>
        public float this[int index]
        {
            readonly get
            {
                switch(index) 
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec4.", index));
                }
            }
            set
            {
                switch (index) 
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec4.", index));
                }
            }
        }

        #region Constructors
        /// <summary>
        /// Construct a 4D vector with identical components
        /// </summary>
        /// <param name="f">The value for each component</param>
        public Vec4(float f)
        {
            X = f;
            Y = f;
            Z = f;
            W = f;
        }

        /// <summary>
        /// Construct a 4D vector
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        /// <param name="z">The Z component</param>
        /// <param name="w">The W component</param>
        public Vec4(float x, float y, float z, float w) 
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Construct a 4D vector as a copy of another
        /// </summary>
        /// <param name="v">The vector to copy</param>
        public Vec4(Vec4 v) 
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = v.W;
        }

        /// <summary>
        /// Construct a 4D vector from a 3D vector and a float
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <param name="w">The W component</param>
        public Vec4(Vec3 v, float w) 
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }

        /// <summary>
        /// Construct a 4D vector from a 2D vector and two floats
        /// </summary>
        /// <param name="v">The 2D vector to use</param>
        /// <param name="z">The Z component</param>
        /// <param name="w">The W component</param>
        public Vec4(Vec2 v, float z, float w)
        {
            X = v.X;
            Y = v.Y;
            Z = z;
            W = w;
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
            W *= ratio;
        }

        /// <summary>
        /// Get a normalized copy of the vector, should be checked by IsFinite afterwards
        /// </summary>
        /// <returns>A normalized copy of the vector</returns>
        public readonly Vec4 NormalizedCopy()
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
        public static Vec4 Normalize(Vec4 v)
        {
            float ratio = 1.0f / v.Length;

            v.X *= ratio;
            v.Y *= ratio;
            v.Z *= ratio;
            v.W *= ratio;

            return v;
        }

        /// <summary>
        /// Normalize the given vector, should be checked by IsFinite afterwards
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <param name="result">The normalized vector</param>
        public static void Normalize(Vec4 v, out Vec4 result)
        {
            result = Normalize(v);
        }
        #endregion

        /// <summary>
        /// Check for a zero vector
        /// </summary>
        /// <returns>True if LengthSqr and thus Length is zero, false otherwise</returns>
        public readonly bool IsZero()
        {
            return LengthSqr.IsZero();
        }

        /// <summary>
        /// Check that the vector is both finite and not NaN
        /// </summary>
        /// <returns>True if all components are finite and not NaN, false otherwise</returns>
        public readonly bool IsFinite()
        {
            return float.IsFinite(X) && float.IsFinite(Y) && float.IsFinite(Z) && float.IsFinite(Z) &&
                !(float.IsNaN(X) || float.IsNaN(Y) || float.IsNaN(Z) || float.IsNaN(W));
        }

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Vec4 vec && Equals(vec);
        }

        /// <inheritdoc/>
        public override int GetHashCode() 
        {
            return HashCode.Combine(X, Y, Z, W);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Vec4 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z && W == other.W;
        }

        /// <summary>
        /// Extract the vector's components
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        /// <param name="z">The Z component</param>
        /// <param name="w">The W component</param>
        public readonly void Deconstruct(out float x, out float y, out float z, out float w)
        {
            x = X; y = Y; z = Z; w = W;
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(Vec4 lhs, Vec4 rhs) 
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns></returns>
        public static bool operator !=(Vec4 lhs, Vec4 rhs) 
        {
            return !lhs.Equals(rhs);
        }

        #region Add
        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Add(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z, v1.W + v2.W);
        }

        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <param name="result">The resulting vector</param>
        public static void Add(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = Add(v1, v2);
        }

        /// <summary>
        /// Define the addition operator for two vectors
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator +(Vec4 lhs, Vec4 rhs) 
        {
            lhs.X += rhs.X;
            lhs.Y += rhs.Y;
            lhs.Z += rhs.Z;
            lhs.W += rhs.W;
            return lhs;
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Subtract(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z, v1.W - v2.W);
        }

        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <param name="result">The resulting vector</param>
        public static void Subtract(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = Subtract(v1, v2);
        }

        /// <summary>
        /// Define the subtraction operator between two vectors
        /// </summary>
        /// <param name="lhs">Left vector</param>
        /// <param name="rhs">Right vector</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator -(Vec4 lhs, Vec4 rhs)
        {
            lhs.X -= rhs.X;
            lhs.Y -= rhs.Y;
            lhs.Z -= rhs.Z;
            lhs.W -= rhs.W;
            return lhs;
        }

        /// <summary>
        /// Invert the sign and thus direction of the vector
        /// </summary>
        /// <param name="v">The vector to invert</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator -(Vec4 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
            v.Z = -v.Z;
            v.W = -v.W;
            return v;
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="v">The vector to scale</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Multiply(Vec4 v, float scale)
        {
            return new Vec4(v.X * scale, v.Y * scale, v.Z * scale, v.W * scale);
        }

        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="v">The vector to scale</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec4 v, float scale, out Vec4 result)
        {
            result = Multiply(v, scale);
        }

        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="lhs">The vector to scale</param>
        /// <param name="rhs">The scalar to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator *(Vec4 lhs, float rhs)
        {
            return new Vec4(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs, lhs.W * rhs);
        }

        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="lhs">The scalar to multiply by</param>
        /// <param name="rhs">The vector to scale</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator *(float lhs,  Vec4 rhs)
        {
            return rhs * lhs;
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Multiply(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z, v1.W * v2.W);
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = Multiply(v1, v2);
        }

        /// <summary>
        /// Define the multiplication operator for component wise vectors
        /// </summary>
        /// <param name="lhs">Left vector</param>
        /// <param name="rhs">Right vector</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator *(Vec4 lhs, Vec4 rhs)
        {
            lhs.X *= rhs.X;
            lhs.Y *= rhs.Y;
            lhs.Z *= rhs.Z;
            lhs.W *= rhs.W;
            return lhs;
        }

        //todo: matrix multiplications
        #endregion

        #region Divide
        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Divide(Vec4 v, float divisor)
        {
            if(divisor.IsZero())
            {
                Console.WriteLine("Division by zero in Vec4.Divide()");
                Logger.Instance.WriteToLog("Division by zero in Vec4.Divide()");
            }

            return new Vec4(v.X / divisor, v.Y / divisor, v.Z / divisor, v.W / divisor);
        }

        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec4 v, float divisor, out Vec4 result)
        {
            result = Divide(v, divisor);
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 Divide(Vec4 v1, Vec4 v2)
        {
            if (v2.X.IsZero() || v2.Y.IsZero() || v2.Z.IsZero() || v2.W.IsZero())
            {
                Console.WriteLine("Division by zero in Vec4.Divide()");
                Logger.Instance.WriteToLog("Division by zero in Vec4.Divide()");
            }
            return new Vec4(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z, v1.W / v2.W);
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = Divide(v1, v2);
        }

        /// <summary>
        /// Define the division operator for a vector and a scalar
        /// </summary>
        /// <param name="lhs">The vector on the left</param>
        /// <param name="rhs">The scalar on the right</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator /(Vec4 lhs, float rhs)
        {
            return Divide(lhs, rhs);
        }

        /// <summary>
        /// Define the division operator for two vectors
        /// </summary>
        /// <param name="lhs">The vector on the left</param>
        /// <param name="rhs">The vector on the right</param>
        /// <returns>The resulting vector</returns>
        public static Vec4 operator /(Vec4 lhs, Vec4 rhs)
        {
            return Divide(lhs, rhs);
        }
        #endregion

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The dot product of the vectors</returns>
        public static float Dot(Vec4 v1, Vec4 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z) + (v1.W * v2.W);
        }

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="result">The dot product of the vectors</param>
        public static void Dot(Vec4 v1, Vec4 v2, out float result)
        {
            result = Dot(v1, v2);
        }

        #region MinMax
        /// <summary>
        /// Create a vector that contains the smallest values for each component from two vectors
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the smallest components from the input</returns>
        public static Vec4 ComponentMin(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X < v2.X ? v1.X : v2.X,
                v1.Y < v2.Y ? v1.Y : v2.Y,
                v1.Z < v2.Z ? v1.Z : v2.Z,
                v1.W < v2.W ? v1.W : v2.W);
        }

        /// <summary>
        /// Create a vector that contains the smallest values for each component from two vectors
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the smallest components from the input</param>
        public static void ComponentMin(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = ComponentMin(v1, v2);
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the largest components from the inputs</returns>
        public static Vec4 ComponentMax(Vec4 v1, Vec4 v2)
        {
            return new Vec4(v1.X > v2.X ? v1.X : v2.X,
                v1.Y > v2.Y ? v1.Y : v2.Y,
                v1.Z > v2.Z ? v1.Z : v2.Z,
                v1.W > v2.W ? v1.W : v2.W);
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the largest components from the inputs</param>
        public static void ComponentMax(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = ComponentMax(v1, v2);
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the smallest magnitude</returns>
        public static Vec4 MagnitudeMin(Vec4 v1, Vec4 v2)
        {
            return v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the smallest magnitude</param>
        public static void MagnitudeMin(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the largest magnitude</returns>
        public static Vec4 MagnitudeMax(Vec4 v1, Vec4 v2)
        {
            return v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the largest magnitude</param>
        public static void MagnitudeMax(Vec4 v1, Vec4 v2, out Vec4 result)
        {
            result = v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Clamps a vector's components between a min and a max
        /// </summary>
        /// <param name="v">The vector to clamp</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <returns>The clamped vector</returns>
        public static Vec4 Clamp(Vec4 v, Vec4 min, Vec4 max)
        {
            float x = v.X < min.X ? min.X : (v.X > max.X ? max.X : v.X);
            float y = v.Y < min.Y ? min.Y : (v.Y > max.Y ? max.Y : v.Y);
            float z = v.Z < min.Z ? min.Z : (v.Z > max.Z ? max.Z : v.Z);
            float w = v.W < min.W ? min.W : (v.W > max.W ? max.W : v.W);

            return new Vec4(x, y, z, w);
        }

        /// <summary>
        /// Clamps a vector's components between a min and a max
        /// </summary>
        /// <param name="v">The vector to clamp</param>
        /// <param name="min">The minimum vector</param>
        /// <param name="max">The maximum vector</param>
        /// <param name="result">The clamped vector</param>
        public static void Clamp(Vec4 v, Vec4 min, Vec4 max, out Vec4 result)
        {
            result = Clamp(v, min, max);
        }
        #endregion

        #region Lerp
        /// <summary>
        /// Linear interpolation between two points
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <returns>The point along the interpolation for the value of t</returns>
        public static Vec4 Lerp(Vec4 v1, Vec4 v2, float t)
        {
            float x = v1.X + (t * (v2.X - v1.X));
            float y = v1.Y + (t * (v2.Y - v1.Y));
            float z = v1.Z + (t * (v2.Z - v1.Z));
            float w = v1.W + (t * (v2.W - v1.W));

            return new Vec4(x, y, z, w);
        }

        /// <summary>
        /// Linear interpolation between two points
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="t">The fractional position along the interpolation between 0 and 1</param>
        /// <param name="result">The point along the interpolation for the value of t</param>
        public static void Lerp(Vec4 v1, Vec4 v2, float t, out Vec4 result)
        {
            result = Lerp(v1, v2, t);
        }

        /// <summary>
        /// Linear interpolation between two points on a component basis
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="t">The fractional position along the interpolation 0-1</param>
        /// <returns>The point along the interpolation for the value of t component wise</returns>
        public static Vec4 Lerp(Vec4 v1, Vec4 v2, Vec4 t)
        {
            float x = v1.X + (t.X * (v2.X - v1.X));
            float y = v1.Y + (t.Y * (v2.Y - v1.Y));
            float z = v1.Z + (t.Z * (v2.Z - v1.Z));
            float w = v1.W + (t.W * (v2.W - v1.W));

            return new Vec4(x, y, z, w);
        }

        /// <summary>
        /// Linear interpolation between two points on a component basis
        /// </summary>
        /// <param name="v1">The first point</param>
        /// <param name="v2">The second point</param>
        /// <param name="t">The fractional position along the interpolation 0-1</param>
        /// <param name="result">The point along the interpolation for the value of t component wise</param>
        public static void Lerp(Vec4 v1, Vec4 v2, Vec4 t, out Vec4 result)
        {
            result = Lerp(v1, v2, t);
        }

        /// <summary>
        /// Interpolate between 3 vectors using barycentric coords
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="v3">The third vector</param>
        /// <param name="u">First barycentric coordinate</param>
        /// <param name="v">Second barycentric coordinate</param>
        /// <returns>v1 where u=v=0, v2 where u=1 v=0, v3 where u=0 v=1, otherwise a linear interpolation of v1,v2,v3</returns>
        public static Vec4 BarycentricInterp(Vec4 v1, Vec4 v2, Vec4 v3, float u, float v)
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
        /// <param name="result">v1 where u=v=0, v2 where u=1 v=0, v3 where u=0 v=1, otherwise a linear interpolation of v1,v2,v3</param>
        public static void BarycentricInterp(Vec4 v1, Vec4 v2, Vec4 v3, float u, float v, out Vec4 result)
        {
            result = BarycentricInterp(v1, v2, v3, u, v);
        }
        #endregion

        //todo: transform methods
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
            return string.Format("({0},{1},{2},{3})",
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider),
                Z.ToString(format, formatProvider),
                W.ToString(format, formatProvider));
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
        /// Get a vec2 with this vector's x and w or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Xw
        {
            get { return new Vec2(X, W); }
            set { X = value.X; W = value.Y; }
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
        /// Get a vec2 with this vector's y and w or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Yw
        {
            get { return new Vec2(Y, W); }
            set { Y = value.X; W = value.Y; }
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
        /// Get a vec2 with this vector's z and w or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Zw
        {
            get { return new Vec2(Z, W); }
            set { Z = value.X; W = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's w and x or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Wx
        {
            get { return new Vec2(W, X); }
            set { W = value.X; X = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's w and y or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Wy
        {
            get { return new Vec2(W, Y); }
            set { W = value.X; Y = value.Y; }
        }

        /// <summary>
        /// Get a vec2 with this vector's w and z or set them with a vec2
        /// </summary>
        [XmlIgnore]
        public Vec2 Wz
        {
            get { return new Vec2(W, Z); }
            set { W = value.X; Z = value.Y; }
        }

        /// <summary>
        /// Get a vec3 with this vector's values as X, Y, Z or set them with a vec3
        /// </summary>
        [XmlIgnore]
        public Vec3 Xyz
        {
            get { return new Vec3(X, Y, Z); }
            set { X = value.X; Y = value.Y; Z = value.Z; }
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

        /// <summary>
        /// Get a Vec4 with this vector's values as X, Y, W, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Xywz
        {
            get { return new Vec4(X, Y, W, Z); }
            set { X = value.X; Y = value.Y; W = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as X, Z, Y, W or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Xzyw
        {
            get { return new Vec4(X, Z, Y, W); }
            set { X = value.X; Z = value.Y; Y = value.Z; W = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as X, Z, W, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Xzwy
        {
            get { return new Vec4(X, Z, W, Y); }
            set { X = value.X; Z = value.Y; W = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as X, W, Y, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Xwyz
        {
            get { return new Vec4(X, W, Y, Z); }
            set { X = value.X; W = value.Y; Y = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as X, W, Z, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Xwzy
        {
            get { return new Vec4(X, W, Z, Y); }
            set { X = value.X; W = value.Y; Z = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, X, Z, W or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Yxzw
        {
            get { return new Vec4(Y, X, Z, W); }
            set { Y = value.X; X = value.Y; Z = value.Z; W = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, X, W, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Yxwz
        {
            get { return new Vec4(Y, X, W, Z); }
            set { Y = value.X; X = value.Y; W = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, Z, X, W or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Yzxw
        {
            get { return new Vec4(Y, Z, X, W); }
            set { Y = value.X; Z = value.Y; X = value.Z; W = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, Z, W, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Yzwx
        {
            get { return new Vec4(Y, Z, W, X); }
            set { Y = value.X; Z = value.Y; W = value.Z; X = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, W, X, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Ywxz
        {
            get { return new Vec4(Y, W, X, Z); }
            set { Y = value.X; W = value.Y; X = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Y, W, Z, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Ywzx
        {
            get { return new Vec4(Y, W, Z, X); }
            set { Y = value.X; W = value.Y; Z = value.Z; X = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, X, Y, W or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zxyw
        {
            get { return new Vec4(Z, X, Y, W); }
            set { Z = value.X; X = value.Y; Y = value.Z; W = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, X, W, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zxwy
        {
            get { return new Vec4(Z, X, W, Y); }
            set { Z = value.X; X = value.Y; W = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, Y, X, W or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zyxw
        {
            get { return new Vec4(Z, Y, X, W); }
            set { Z = value.X; Y = value.Y; X = value.Z; W = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, Y, W, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zywx
        {
            get { return new Vec4(Z, Y, W, X); }
            set { Z = value.X; Y = value.Y; W = value.Z; X = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, W, X, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zwxy
        {
            get { return new Vec4(Z, W, X, Y); }
            set { Z = value.X; W = value.Y; X = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as Z, W, Y, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Zwyx
        {
            get { return new Vec4(Z, W, Y, X); }
            set { Z = value.X; W = value.Y; Y = value.Z; X = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, X, Y, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wxyz
        {
            get { return new Vec4(W, X, Y, Z); }
            set { W = value.X; X = value.Y; Y = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, X, Z, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wxzy
        {
            get { return new Vec4(W, X, Z, Y); }
            set { W = value.X; X = value.Y; Z = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, Y, X, Z or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wyxz
        {
            get { return new Vec4(W, Y, X, Z); }
            set { W = value.X; Y = value.Y; X = value.Z; Z = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, Y, Z, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wyzx
        {
            get { return new Vec4(W, Y, Z, X); }
            set { W = value.X; Y = value.Y; Z = value.Z; X = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, Z, X, Y or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wzxy
        {
            get { return new Vec4(W, Z, X, Y); }
            set { W = value.X; Z = value.Y; X = value.Z; Y = value.W; }
        }

        /// <summary>
        /// Get a Vec4 with this vector's values as W, Z, Y, X or set them with a vec4
        /// </summary>
        [XmlIgnore]
        public Vec4 Wzyx
        {
            get { return new Vec4(W, Z, Y, X); }
            set { W = value.X; Z = value.Y; Y = value.Z; X = value.W; }
        }
        #endregion

#if OPENTK
        #region OpenTKCompat
        /// <summary>
        /// Handle conversion from OpenTK's Vector4 to Vec4
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Vec4(Vector4 v)
        {
            return new Vec4(v.X, v.Y, v.Z, v.W);
        }

        /// <summary>
        /// Handle conversion from Vec4 to OpenTK's Vector4
        /// </summary>
        /// <param name="v"></param>
        public static implicit operator Vector4(Vec4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        #endregion
#endif
    }
}
