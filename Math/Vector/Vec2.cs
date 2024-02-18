using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// A 2D Vector struct that contains float values
    /// </summary>
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X;
        public float Y;

        /// <summary>
        /// The Length of the vector
        /// </summary>
        public float Length
        {
            get
            {
                return MathF.Sqrt(X * X + Y * Y);
            }
        }

        /// <summary>
        /// The length of the vector squared, slightly faster than getting the length itself
        /// </summary>
        public float LengthSqr
        {
            get 
            {
                return X * X + Y * Y;
            }
        }

        /// <summary>
        /// Predefined 2D vector 0,0
        /// </summary>
        public static readonly Vec2 Zero = new Vec2(0.0f);

        /// <summary>
        /// Predefined 2D vector 1,1
        /// </summary>
        public static readonly Vec2 One = new Vec2(1.0f);

        /// <summary>
        /// Predefined 2D vector -1,-1
        /// </summary>
        public static readonly Vec2 OneMinus = new Vec2(-1.0f);

        /// <summary>
        /// Predefined 2D vector 1,0
        /// </summary>
        public static readonly Vec2 UnitX = new Vec2(1.0f, 0.0f);

        /// <summary>
        /// Predefined 2D vector 0,1
        /// </summary>
        public static readonly Vec2 UnitY = new Vec2(0.0f, 1.0f);

        /// <summary>
        /// Size of the Vec2 struct in bytes
        /// </summary>
        public static readonly int SizeInBytesU = Unsafe.SizeOf<Vec2>();

        /// <summary>
        /// Index accessor for the vector
        /// </summary>
        /// <param name="index">Index that corresponds to the X, or Y component</param>
        /// <returns>The value of X, or Z depending on the index value</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the index is not a value from 0-1 inclusive</exception>
        public float this[int index]
        {
            readonly get
            {
                switch (index) 
                {
                    case 0: 
                        return X;
                    case 1:
                        return Y;
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec2.", index));
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
                    default:
                        throw new IndexOutOfRangeException(string.Format("Index: {0} out of range for Vec2.", index));
                }
            }
        }

        /// <summary>
        /// Construct a 2D vector with identical components
        /// </summary>
        /// <param name="f">The value of both components</param>
        public Vec2(float f) 
        {
            X = f;
            Y = f;
        }

        /// <summary>
        /// Construct a 2D vector
        /// </summary>
        /// <param name="x">The value of the X component</param>
        /// <param name="y">The value of the Y component</param>
        public Vec2(float x, float y) 
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Construct a 2D vector as a copy of another
        /// </summary>
        /// <param name="v">The vector to copy from</param>
        public Vec2(Vec2 v) 
        {
            X = v.X;
            Y = v.Y;
        }

        #region FromVec3
        /// <summary>
        /// Construct a 2D vector using the X and Y components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3XY(Vec3 v)
        {
            return new Vec2(v.X, v.Y);
        }

        /// <summary>
        /// Construct a 2D vector using the Y and Z components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3YZ(Vec3 v)
        {
            return new Vec2(v.Y, v.Z);
        }

        /// <summary>
        /// Construct a 2D vector using the Z and X components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3ZX(Vec3 v)
        {
            return new Vec2(v.Z, v.X);
        }

        /// <summary>
        /// Construct a 2D vector using the X and Z components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3XZ(Vec3 v)
        {
            return new Vec2(v.X, v.Z);
        }

        /// <summary>
        /// Construct a 2D vector using the Y and X components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3YX(Vec3 v)
        {
            return new Vec2(v.Y, v.X);
        }

        /// <summary>
        /// Construct a 2D vector using the Z and Y components of a 3D vector
        /// </summary>
        /// <param name="v">The 3D vector to use</param>
        /// <returns>The resulting 2D vector</returns>
        public static Vec2 FromVec3ZY(Vec3 v)
        {
            return new Vec2(v.Z, v.Y);
        }
        #endregion

        /// <summary>
        /// Normalize the vector, should be checked by IsFinite after to avoid infinites and NaNs
        /// </summary>
        public void Normalize()
        {
            float ratio = 1.0f / Length;

            X *= ratio;
            Y *= ratio;
        }

        /// <summary>
        /// Get a normalized copy of the vector, should be checked by IsFinite after to avoid infinites and NaNs
        /// </summary>
        /// <returns>A normalized copy of the vector</returns>
        public readonly Vec2 NormalizeCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }

        /// <summary>
        /// Check for a zero vector
        /// </summary>
        /// <returns>True if both X and Y are 0, false otherwise</returns>
        public readonly bool IsZero() 
        {
            return X.IsZero() && Y.IsZero();
        }

        /// <summary>
        /// Check that the vector is both finite and not NaN
        /// </summary>
        /// <returns>True if X and Y are finite and not Nan, false otherwise</returns>
        public readonly bool IsFinite()
        {
            return float.IsFinite(X) && float.IsFinite(Y) && !(float.IsNaN(X) || float.IsNaN(Y));
        }

        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Vec2 && Equals((Vec2)obj);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode() 
        {
            return HashCode.Combine(X, Y);
        }

        /// <inheritdoc/>
        public readonly bool Equals(Vec2 other) 
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Extract the vector's components
        /// </summary>
        /// <param name="x">The vector's X component</param>
        /// <param name="y">The vector's Y component</param>
        public readonly void Deconstruct(out float x, out float y)
        {
            x = X; 
            y = Y;
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="left">The left vector to compare</param>
        /// <param name="right">The right vector to compare</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="left">The left vector to compare</param>
        /// <param name="right">The right vector to compare</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !(left == right);
        }

        #region Add
        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 Add(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X + v2.X, v1.Y + v2.Y);
        }

        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">First vector to add</param>
        /// <param name="v2">Second vector to add</param>
        /// <param name="result">The resulting vector</param>
        public static void Add(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result.X = v1.X + v2.X;
            result.Y = v1.Y + v2.Y;
        }

        /// <summary>
        /// Define the addition operator between two vectors
        /// </summary>
        /// <param name="v1">First/left vector to add</param>
        /// <param name="v2">Second/right vector to add</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 operator +(Vec2 v1, Vec2 v2)
        {
            v1.X += v2.X;
            v1.Y += v2.Y;
            return v1;
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 Subtract(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">Vector to subtract from</param>
        /// <param name="v2">Vector to subtract</param>
        /// <param name="result">The resulting vector</param>
        public static void Subtract(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result.X = v1.X - v2.X;
            result.Y = v1.Y - v2.Y;
        }

        /// <summary>
        /// Define the subtraction operator between two vectors
        /// </summary>
        /// <param name="v1">First/left vector to subtract</param>
        /// <param name="v2">Second/right vector to subtract</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 operator -(Vec2 v1, Vec2 v2)
        {
            v1.X -= v2.X;
            v1.Y -= v2.Y;
            return v1;
        }

        /// <summary>
        /// Invert the sign and thus direction of the vector
        /// </summary>
        /// <param name="v">The vector to invert</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 operator -(Vec2 v)
        {
            v.X = -v.X;
            v.Y = -v.Y;
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
        public static Vec2 Multiply(Vec2 v1, float scale)
        {
            return new Vec2(v1.X * scale, v1.Y * scale);
        }

        /// <summary>
        /// Multiply the vector by a scalar value, scaling it's magnitude
        /// </summary>
        /// <param name="v1">The vector to scale</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec2 v1, float scale, out Vec2 result)
        {
            result.X = v1.X * scale;
            result.Y = v1.Y * scale;
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector to multiply</param>
        /// <param name="v2">Second vector to multiply</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 Multiply(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X * v2.X, v1.Y * v2.Y);
        }

        /// <summary>
        /// Multiply the individual components of two vectors
        /// </summary>
        /// <param name="v1">First vector to multiply</param>
        /// <param name="v2">Second vector to multiply</param>
        /// <param name="result">The resulting vector</param>
        public static void Multiply(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result.X = v1.X * v2.X;
            result.Y = v1.Y * v2.Y;
        }

        /// <summary>
        /// Define the multiplication operator for a vector and a scalar
        /// </summary>
        /// <param name="v">The vector to multiply</param>
        /// <param name="scale">The scalar to multiply by</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 operator *(Vec2 v, float scale)
        {
            v.X *= scale;
            v.Y *= scale;
            return v;
        }

        /// <summary>
        /// Define the multiplication operator for a scalar and a vector
        /// </summary>
        /// <param name="scale">The scalar to multiply by</param>
        /// <param name="v">The vector to multiply</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 operator *(float scale, Vec2 v)
        {
            v.X *= scale;
            v.Y *= scale;
            return v;
        }

        /// <summary>
        /// Define the multiplication operator for a vector component wise with another vector
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vec2 operator *(Vec2 v1, Vec2 v2)
        {
            v1.X *= v2.X;
            v1.Y *= v2.Y;
            return v1;
        }
        #endregion

        #region Divide
        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 Divide(Vec2 v1, float divisor)
        {
            if (divisor.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
            }
            return new Vec2(v1.X / divisor, v1.Y / divisor);
        }

        /// <summary>
        /// Divide the vector by a scalar, scaling the vector
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="divisor">The scalar to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec2 v1, float divisor, out Vec2 result)
        {
            if (divisor.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
            }

            result.X = v1.X / divisor;
            result.Y = v1.Y / divisor;
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <returns>The resulting vector</returns>
        public static Vec2 Divide(Vec2 v1, Vec2 v2)
        {
            if (v2.X.IsZero() || v2.Y.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
            }

            return new Vec2(v1.X / v2.X, v1.Y / v2.Y);
        }

        /// <summary>
        /// Divide a vector by the individual components of another
        /// </summary>
        /// <param name="v1">The vector to divide</param>
        /// <param name="v2">The vector to divide by</param>
        /// <param name="result">The resulting vector</param>
        public static void Divide(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            if (v2.X.IsZero() || v2.Y.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", v1));
            }

            result.X = v1.X / v2.X;
            result.Y = v1.Y / v2.Y;
        }
        
        public static Vec2 operator /(Vec2 left, float right)
        {
            if (right.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", left));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", left));
            }

            left.X /= right;
            left.Y /= right;
            return left;
        }

        public static Vec2 operator /(Vec2 left, Vec2 right)
        {
            if (right.X.IsZero() || right.Y.IsZero())
            {
                Console.WriteLine(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", left));
                Logger.Instance.WriteToLog(string.Format("Division by zero in Vec2.Divide() Vector value: {0}", left));
            }

            left.X /= right.X;
            left.Y /= right.Y;
            return left;
        }
        #endregion

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>Returns the dot product of the vectors</returns>
        public static float Dot(Vec2 v1, Vec2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        /// <summary>
        /// Get the dot product of two vectors
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="result">The resulting dot product</param>
        public static void Dot(Vec2 v1, Vec2 v2, out float result)
        {
            result = v1.X * v2.X + v1.Y * v2.Y;
        }

        #region MinMax
        /// <summary>
        /// Create a vector from the smallest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the smallest components from the input</returns>
        public static Vec2 ComponentMin(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X < v2.X ? v1.X : v2.X,
                v1.Y < v2.Y ? v1.Y : v2.Y);
        }

        /// <summary>
        /// Create a vector from the smallest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the smallest components from the inputs</param>
        public static void ComponentMin(Vec2 v1, Vec2 v2, Vec2 result)
        {
            result.X = v1.X < v2.X ? v1.X : v2.X;
            result.Y = v1.Y < v2.Y ? v1.Y : v2.Y;
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <returns>A vector with the largest components from the inputs</returns>
        public static Vec2 ComponentMax(Vec2 v1, Vec2 v2)
        {
            return new Vec2(v1.X > v2.X ? v1.X : v2.X,
                v1.Y > v2.Y ? v1.Y : v2.Y);
        }

        /// <summary>
        /// Create a vector from the largest corresponding vector components
        /// </summary>
        /// <param name="v1">The first vector to use</param>
        /// <param name="v2">The second vector to use</param>
        /// <param name="result">A vector with the largest components from the inputs</param>
        public static void ComponentMax(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result.X = v1.X > v2.X ? v1.X : v2.X;
            result.Y = v1.Y > v2.Y ? v1.Y : v2.Y;
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the smallest magnitude</returns>
        public static Vec2 MagnitudeMin(Vec2 v1, Vec2 v2)
        {
            return v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the smallest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the smallest magnitude</param>
        public static void MagnitudeMin(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result = v1.LengthSqr < v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <returns>The vector with the largest magnitude</returns>
        public static Vec2 MagnitudeMax(Vec2 v1, Vec2 v2)
        {
            return v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }

        /// <summary>
        /// Finds the vector with the largest magnitude
        /// </summary>
        /// <param name="v1">The first vector to compare</param>
        /// <param name="v2">The second vector to compare</param>
        /// <param name="result">The vector with the largest magnitude</param>
        public static void MagnitudeMax(Vec2 v1, Vec2 v2, out Vec2 result)
        {
            result = v1.LengthSqr > v2.LengthSqr ? v1 : v2;
        }
        #endregion

        #region OpenTkCompat
        /// <summary>
        /// Handle conversion from OpenTK's Vector2 to Vec2
        /// </summary>
        /// <param name="v">The vector to convert</param>
        public static implicit operator Vec2(Vector2 v) 
        {
            return new Vec2(v.X, v.Y);
        }

        /// <summary>
        /// Handle conversion from Vec2 to OpenTK's Vector2
        /// </summary>
        /// <param name="v">The vector to convert</param>
        public static implicit operator Vector2(Vec2 v)
        {
            return new Vector2(v.X, v.Y);
        }
        #endregion
    }
}
