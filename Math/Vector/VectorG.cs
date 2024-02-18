using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// General vector struct that can be defined for any size up to int.MaxValue and value type
    /// </summary>
    internal struct VectorG<T> : IEquatable<VectorG<T>> where T : INumber<T>
    {
        T[] Comp = [];
        readonly int Size;

        /// <summary>
        /// Index accessor for the vector
        /// </summary>
        /// <param name="index">Index that corresponds to the component</param>
        /// <returns>The value of Comp at the given index</returns>
        public T this[int index]
        {
            get { return Comp[index]; }
            set { Comp[index] = value; }
        }

        /// <summary>
        /// The vector's magnitude
        /// </summary>
        public readonly T Magnitude
        {
            get
            {
                return MathF.Sqrt((dynamic)LengthSqr);
            }
        }

        /// <summary>
        /// The vector's length, alias for Magnitude
        /// </summary>
        public readonly T Length
        {
            get
            {
                return Magnitude;
            }
        }

        /// <summary>
        /// The vector's length squared
        /// </summary>
        public readonly T LengthSqr
        {
            get
            {
                T lengthSqu = T.Zero;
                foreach(var v in Comp)
                {
                    lengthSqu += v * v;
                }
                return lengthSqu;
            }
        }
        public VectorG(int size)
        {
            Size = size;
            Comp = new T[Size];
        }

        public VectorG(T val)
        {
            _ = Comp.Append(val);
            Size = Comp.Length;
        }

        public VectorG(T[] vals)
        {
            Size = vals.Length;
            Comp = vals;
        }

        #region Normalize
        /// <summary>
        /// Normalize the vector
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the vector is a zero vector.</exception>
        public void Normalize()
        {
            if(Length.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to normalize a zero vector.");
            }
            T ratio = T.One / Length;

            for (int i = 0;i < Size; i++)
            {
                Comp[i] *= ratio;
            }
        }

        /// <summary>
        /// Normalize the vector provided
        /// </summary>
        /// <param name="v">The vector to normalize</param>
        /// <returns>The normalized vector</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vector is a zero vector.</exception>
        public static VectorG<T> Normalize(VectorG<T> v)
        {
            if(v.Length.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to normalize a zero vector.");
            }
            T ratio = T.One / v.Length;
            
            for(int i = 0; i < v.Size; i++)
            {
                v[i] *= ratio;
            }
            return v;
        }

        /// <summary>
        /// Create a normalized copy of the the vector
        /// </summary>
        /// <returns>A normalized copy</returns>
        /// <exception cref="InvalidOperationException">Passed up from Normalize</exception>
        public readonly VectorG<T> NormalizedCopy()
        {
            var c = this;
            try
            {
                c.Normalize();
            }
            catch (InvalidOperationException) 
            {
                throw;
            }
            return c;
        }
        #endregion

        /// <inheritdoc/>
        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is VectorG<T> vec && Equals(vec);
        }

        /// <inheritdoc/>
        public readonly bool Equals(VectorG<T> other)
        {
            if(Size != other.Size)
            {
                return false;
            }

            for(int i = 0; i < Size; i++) 
            {
                if (Comp[i] != other.Comp[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode() 
        {
            return Comp.GetHashCode();
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(VectorG<T> lhs, VectorG<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(VectorG<T> lhs, VectorG<T> rhs) 
        {
            return !lhs.Equals(rhs);
        }

        #region Add
        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="ArgumentException">Thrown if the vectors are different sizes</exception>
        public static VectorG<T> Add(VectorG<T> v1, VectorG<T> v2)
        {
            if(v1.Size != v2.Size)
            {
                throw new ArgumentException("Attempting to add two different sizes of vector.");
            }

            var c = v1;
            for (int i = 0; i < c.Size; i++) 
            {
                c[i] += v2[i];
            }
            return c;
        }

        /// <summary>
        /// Define the addition operator for two vectors
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="ArgumentException">Passed up from Add</exception>
        public static VectorG<T> operator +(VectorG<T> lhs, VectorG<T> rhs)
        {
            try
            {
                return Add(lhs, rhs);
            }
            catch (ArgumentException)
            {
                throw;
            }
        }
        #endregion

        #region Subtract
        /// <summary>
        /// Subtract the second vector from the first
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="ArgumentException">Thrown if the two vectors are different sizes</exception>
        public static VectorG<T> Subtract(VectorG<T> v1, VectorG<T> v2)
        {
            if(v1.Size != v2.Size) 
            {
                throw new ArgumentException("Attempting to subtract two different sizes of vector.");
            }

            var c = v1;
            for (int i = 0; i < c.Size; i++)
            {
                c[i] -= v2[i];
            }
            return c;
        }

        /// <summary>
        /// Define the subtraction operator between two vectors
        /// </summary>
        /// <param name="lhs">Left vector</param>
        /// <param name="rhs">Right vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="ArgumentException">Passed up from Subtract</exception>
        public static VectorG<T> operator -(VectorG<T> lhs, VectorG<T> rhs)
        {
            try
            {
                return Subtract(lhs, rhs);
            }
            catch (ArgumentException) 
            {
                throw;
            }
        }

        /// <summary>
        /// Invert the sign and direction of the vector
        /// </summary>
        /// <param name="v">The vector to invert</param>
        /// <returns>The resulting vector</returns>
        public static VectorG<T> operator -(VectorG<T> v)
        {
            for (int i = 0; i < v.Size; i++)
            {
                v[i] = -v[i];
            }
            return v;
        }
        #endregion
        //todo: math functions
    }
}
