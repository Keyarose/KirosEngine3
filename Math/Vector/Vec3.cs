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
    public struct Vec3 : IEquatable<Vec3>
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
        public static readonly Vec3 Unit = new Vec3(1.0f);

        /// <summary>
        /// Predefined 3D vector -1,-1,-1
        /// </summary>
        public static readonly Vec3 UnitMinus = new Vec3(-1.0f);

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
        public readonly Vec3 NormalizeCopy()
        {
            var c = this;
            c.Normalize();
            return c;
        }

        /// <summary>
        /// Check for a zero vector
        /// </summary>
        /// <returns>True if both X,Y and Z are 0, false otherwise</returns>
        public readonly bool IsZero()
        {
            return X.IsZero() && Y.IsZero() && Z.IsZero();
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
        /// <param name="left">The left vector to compare</param>
        /// <param name="right">The right vector to compare</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(Vec3 left, Vec3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="left">The left vector to compare</param>
        /// <param name="right">The right vector to compare</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(Vec3 left, Vec3 right) 
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
                Logger.Instance.WriteToLog("Division by zero in Vec3.Divide()");
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
    }
}
