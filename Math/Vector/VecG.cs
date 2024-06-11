using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// General vector struct that can be defined for any size up to int.MaxValue and value type
    /// </summary>
    public class VecG<T> : IEquatable<VecG<T>> where T : INumber<T>
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
        public T Magnitude
        {
            get
            {
                return MathF.Sqrt((dynamic)LengthSqr);
            }
        }

        /// <summary>
        /// The vector's length, alias for Magnitude
        /// </summary>
        public T Length
        {
            get
            {
                return Magnitude;
            }
        }

        /// <summary>
        /// The vector's length squared
        /// </summary>
        public T LengthSqr
        {
            get
            {
                T lengthSqu = T.Zero;
                foreach (var v in Comp)
                {
                    lengthSqu += v * v;
                }
                return lengthSqu;
            }
        }

        /// <summary>
        /// Constructor defining only the size of the vector.
        /// </summary>
        /// <param name="size">The size of the vector to construct.</param>
        public VecG(int size)
        {
            Size = size;
            Comp = new T[Size];
        }

        /// <summary>
        /// Constructor from a single value.
        /// </summary>
        /// <param name="val">The value to construct from.</param>
        public VecG(T val)
        {
            Comp = [.. Comp, val];
            Size = Comp.Length;
        }

        /// <summary>
        /// Constructor from an array of T.
        /// </summary>
        /// <param name="vals">The array to construct from.</param>
        public VecG(T[] vals)
        {
            Size = vals.Length;
            Comp = vals;
        }

        #region CommonValueFactory
        /// <summary>
        /// Construct a vector of given size with all values set to one
        /// </summary>
        /// <param name="size">The size of the vector to create</param>
        /// <returns>The resulting vector</returns>
        public static VecG<T> One(int size)
        {
            return new VecG<T>(new T[size].Populate(T.One));
        }

        /// <summary>
        /// Construct a vector of given size with all values set to zero
        /// </summary>
        /// <param name="size">The size of the vector to create</param>
        /// <returns>The resulting vector</returns>
        public static VecG<T> Zero(int size)
        {
            return new VecG<T>(new T[size].Populate(T.Zero));
        }

        /// <summary>
        /// Construct a vector of given size with all values set to minus one
        /// </summary>
        /// <param name="size">The size of the vector to create</param>
        /// <returns>The resulting vector</returns>
        public static VecG<T> OneMinus(int size)
        {
            return new VecG<T>(new T[size].Populate(-T.One));
        }
        #endregion

        #region StructFormFactory
        /// <summary>
        /// Create a Vec2 from the VecG's value truncating what doesn't fit
        /// </summary>
        /// <returns>The Vec2 result</returns>
        public Vec2 AsVec2()
        {
            if (Size == 1)
            {
                return new Vec2(float.CreateTruncating(Comp[0]), 0.0f);
            }
            else
                return new Vec2(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]));
        }

        /// <summary>
        /// Create a Vec3 from the VecG's value truncating what doesn't fit
        /// </summary>
        /// <returns>The resulting Vec3</returns>
        public Vec3 AsVec3()
        {
            if (Size == 1)
            {
                return new Vec3(float.CreateTruncating(Comp[0]), 0.0f, 0.0f);
            }
            else if (Size == 2)
            {
                return new Vec3(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]), 0.0f);
            }
            else
                return new Vec3(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]), float.CreateTruncating(Comp[2]));
        }

        /// <summary>
        /// Create a Vec4 from the VecG's value truncating what doesn't fit
        /// </summary>
        /// <returns>The resulting Vec4</returns>
        public Vec4 AsVec4()
        {
            if (Size == 1)
            {
                return new Vec4(float.CreateTruncating(Comp[0]), 0.0f, 0.0f, 0.0f);
            }
            else if (Size == 2)
            {
                return new Vec4(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]), 0.0f, 0.0f);
            }
            else if (Size == 3)
            {
                return new Vec4(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]), float.CreateTruncating(Comp[2]), 0.0f);
            }
            else
                return new Vec4(float.CreateTruncating(Comp[0]), float.CreateTruncating(Comp[1]), float.CreateTruncating(Comp[2]), float.CreateTruncating(Comp[3]));
        }
        #endregion

        #region Normalize
        /// <summary>
        /// Normalize the vector
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the vector is a zero vector.</exception>
        public void Normalize()
        {
            if (Length.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to normalize a zero vector.");
            }
            T ratio = T.One / Length;

            for (int i = 0; i < Size; i++)
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
        public static VecG<T> Normalize(VecG<T> v)
        {
            if (v.Length.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to normalize a zero vector.");
            }
            T ratio = T.One / v.Length;

            for (int i = 0; i < v.Size; i++)
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
        public VecG<T> NormalizedCopy()
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
            return obj is VecG<T> vec && Equals(vec);
        }

        /// <inheritdoc/>
        public bool Equals(VecG<T>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (Size != other.Size)
            {
                return false;
            }

            for (int i = 0; i < Size; i++)
            {
                if (Comp[i] != other.Comp[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Comp.GetHashCode();
        }

        /// <summary>
        /// Equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>True if equivalent, false otherwise</returns>
        public static bool operator ==(VecG<T> lhs, VecG<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Non-equivalence operator definition
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>False if equivalent, true otherwise</returns>
        public static bool operator !=(VecG<T> lhs, VecG<T> rhs)
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
        public static VecG<T> Add(VecG<T> v1, VecG<T> v2)
        {
            if (v1.Size != v2.Size)
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
        public static VecG<T> operator +(VecG<T> lhs, VecG<T> rhs)
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
        public static VecG<T> Subtract(VecG<T> v1, VecG<T> v2)
        {
            if (v1.Size != v2.Size)
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
        public static VecG<T> operator -(VecG<T> lhs, VecG<T> rhs)
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
        public static VecG<T> operator -(VecG<T> v)
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
