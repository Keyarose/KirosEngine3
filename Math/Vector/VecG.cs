using KirosEngine3.Math.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// General vector struct that can be defined for any size up to int.MaxValue and value type
    /// </summary>
    public class VecG<T> : IEquatable<VecG<T>> where T : INumber<T>
    {
        private T[] _comp = [];

        private int _size;

        /// <summary>
        /// The size of the vector.
        /// </summary>
        public int Size { get { return _size; } }

        /// <summary>
        /// Index accessor for the vector
        /// </summary>
        /// <param name="index">Index that corresponds to the component</param>
        /// <returns>The value of _comp at the given index</returns>
        public T this[int index]
        {
            get { return _comp[index]; }
            set { _comp[index] = value; }
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
                foreach (var v in _comp)
                {
                    lengthSqu += v * v;
                }
                return lengthSqu;
            }
        }

        /// <summary>
        /// The vector's components.
        /// </summary>
        public T[] Components { get { return _comp; } }

        /// <summary>
        /// The type of the vector's value.
        /// </summary>
        public Type ValueType { get { return typeof(T); } }

        /// <summary>
        /// Whether the vector is a zero vector or not.
        /// </summary>
        public bool IsZero => LengthSqr.Equals(T.Zero);

        /// <summary>
        /// Constructor defining only the size of the vector.
        /// </summary>
        /// <param name="size">The size of the vector to construct.</param>
        public VecG(int size)
        {
            _size = size;
            _comp = new T[_size];
        }

        /// <summary>
        /// Constructor from a single value.
        /// </summary>
        /// <param name="val">The value to construct from.</param>
        public VecG(T val)
        {
            _comp = [.. _comp, val];
            _size = _comp.Length;
        }

        /// <summary>
        /// Constructor from an array of T.
        /// </summary>
        /// <param name="vals">The array to construct from.</param>
        public VecG(T[] vals)
        {
            _size = vals.Length;
            _comp = vals;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="vec">The array to copy.</param>
        public VecG(VecG<T> vec)
        {
            _size = vec.Size;
            _comp = vec.Components;
        }

        /// <summary>
        /// Construct a vector of the given size and the components of the given vector. If the new vector is
        /// smaller data is discarded.
        /// </summary>
        /// <param name="vec">The vector to get components from.</param>
        /// <param name="size">The size of the new vector.</param>
        public VecG(VecG<T> vec, int size)
        {
            _size = size;
            _comp = new T[size];

            for (int i = 0; i < size; i++)
            {
                _comp[i] = vec.Components[i];
            }
        }

        /// <summary>
        /// Construct a vector using the given vector as a basis, and appending the given array as more components.
        /// </summary>
        /// <param name="vec">The vector to use as a base.</param>
        /// <param name="vals">The components to be appended.</param>
        public VecG(VecG<T> vec, T[] vals)
        {
            _size = vec.Size + vals.Length;
            _comp = [.. vec.Components, .. vals];
        }

        #region Setters
        /// <summary>
        /// Change the size of the vector.
        /// </summary>
        /// <param name="size">The new size of the vector.</param>
        public void SetSize(int size)
        {
            Array.Resize(ref _comp, size);

            _size = size;
        }
        #endregion

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
        /// Create a Vec2 from the VecG's value truncating what doesn't fit.
        /// </summary>
        /// <param name="strict">True enables strict fitting.</param>
        /// <returns>The Vec2 result</returns>
        /// <exception cref="InvalidOperationException">Thrown if strict is true and the vector is not the correct size.</exception>
        public Vec2 AsVec2(bool strict = false)
        {
            if (strict)
            {
                if (Size != 2)
                    throw InvalidOpExceptionAsStructBuilder(Size, nameof(Vec2));

                return new Vec2(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]));
            }
            else
            {
                if (Size == 1)
                {
                    return new Vec2(float.CreateTruncating(_comp[0]), 0.0f);
                }
                else
                    return new Vec2(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]));
            }
        }

        /// <summary>
        /// Create a Vec3 from the VecG's value truncating what doesn't fit
        /// </summary>
        /// <param name="strict">True enables strict fitting.</param>
        /// <returns>The resulting Vec3</returns>
        /// <exception cref="InvalidOperationException">Thrown if strict is true and the vector is not the correct size.</exception>
        public Vec3 AsVec3(bool strict = false)
        {
            if (strict)
            {
                if (Size != 3)
                    throw InvalidOpExceptionAsStructBuilder(Size, nameof(Vec3));

                return new Vec3(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), float.CreateTruncating(_comp[2]));
            }
            else
            {

                if (Size == 1)
                {
                    return new Vec3(float.CreateTruncating(_comp[0]), 0.0f, 0.0f);
                }
                else if (Size == 2)
                {
                    return new Vec3(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), 0.0f);
                }
                else
                    return new Vec3(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), float.CreateTruncating(_comp[2]));
            }
        }

        /// <summary>
        /// Create a Vec4 from the VecG's value truncating what doesn't fit
        /// </summary>
        /// <param name="strict">True enables strict fitting.</param>
        /// <returns>The resulting Vec4</returns>
        /// <exception cref="InvalidOperationException">Thrown if strict is true and the vector is not the correct size.</exception>
        public Vec4 AsVec4(bool strict = false)
        {
            if (strict)
            {
                if (Size != 4)
                    throw InvalidOpExceptionAsStructBuilder(Size,nameof(Vec4));

                return new Vec4(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), float.CreateTruncating(_comp[2]), float.CreateTruncating(_comp[3]));
            }
            else
            {
                if (Size == 1)
                {
                    return new Vec4(float.CreateTruncating(_comp[0]), 0.0f, 0.0f, 0.0f);
                }
                else if (Size == 2)
                {
                    return new Vec4(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), 0.0f, 0.0f);
                }
                else if (Size == 3)
                {
                    return new Vec4(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), float.CreateTruncating(_comp[2]), 0.0f);
                }
                else
                    return new Vec4(float.CreateTruncating(_comp[0]), float.CreateTruncating(_comp[1]), float.CreateTruncating(_comp[2]), float.CreateTruncating(_comp[3]));
            }
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

            for (int i = 0; i < _size; i++)
            {
                _comp[i] *= ratio;
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

            for (int i = 0; i < v._size; i++)
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

        #region Comparison
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
                if (_comp[i] != other._comp[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Indicates if the vector is equal to the given one within the given tolerance.
        /// </summary>
        /// <param name="other">The vector to compare.</param>
        /// <param name="tolerance">The tolerance for difference between the vectors.</param>
        /// <returns>True if the vectors are with the tolerance of each other.</returns>
        public bool Equals(VecG<T> other, T tolerance)
        {
            var zipped = _comp.Zip(other._comp);
            foreach (var (First, Second) in zipped)
            {
                T diff = First - Second;
                if (diff < T.Zero)
                    diff = -diff;

                if (!(diff < tolerance))
                    return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _comp.GetHashCode();
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

        /// <summary>
        /// Check to see if the vector is parallel to the given vector.
        /// </summary>
        /// <param name="other">The vector to check against.</param>
        /// <returns>True if the vectors are parallel to each other, false otherwise.</returns>
        public bool IsParallel(VecG<T> other)
        {
            T d = Dot(other);
            if (d < T.Zero)
                d = -d;

            if (d == T.One) { return true; }

            return false;
        }
        #endregion

        #region Add
        /// <summary>
        /// Add two vectors together
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vectors are different sizes</exception>
        public static VecG<T> Add(VecG<T> v1, VecG<T> v2)
        {
            if (v1.Size != v2.Size)
            {
                throw new InvalidOperationException("Attempting to add two different sizes of vector.");
            }

            var c = v1;
            for (int i = 0; i < c.Size; i++)
            {
                c[i] += v2[i];
            }
            return c;
        }

        /// <summary>
        /// Add two vectors together.
        /// </summary>
        /// <param name="rhs">The second vector to add.</param>
        /// <returns>The resulting vector.</returns>
        public VecG<T> Add(VecG<T> rhs)
        {
            try
            {
                return Add(this, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Define the addition operator for two vectors
        /// </summary>
        /// <param name="lhs">The left vector</param>
        /// <param name="rhs">The right vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="InvalidOperationException">Passed up from Add</exception>
        public static VecG<T> operator +(VecG<T> lhs, VecG<T> rhs)
        {
            try
            {
                return Add(lhs, rhs);
            }
            catch (InvalidOperationException)
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
        /// <exception cref="InvalidOperationException">Thrown if the two vectors are different sizes</exception>
        public static VecG<T> Subtract(VecG<T> v1, VecG<T> v2)
        {
            if (v1.Size != v2.Size)
            {
                throw new InvalidOperationException("Attempting to subtract two different sizes of vector.");
            }

            var c = v1;
            for (int i = 0; i < c.Size; i++)
            {
                c[i] -= v2[i];
            }
            return c;
        }

        /// <summary>
        /// Subtract the given vector from this one.
        /// </summary>
        /// <param name="rhs">The vector to subtract.</param>
        /// <returns>The resulting vector.</returns>
        public VecG<T> Subtract(VecG<T> rhs)
        {
            try
            {
                return Subtract(this, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Define the subtraction operator between two vectors
        /// </summary>
        /// <param name="lhs">Left vector</param>
        /// <param name="rhs">Right vector</param>
        /// <returns>The resulting vector</returns>
        /// <exception cref="InvalidOperationException">Passed up from Subtract</exception>
        public static VecG<T> operator -(VecG<T> lhs, VecG<T> rhs)
        {
            try
            {
                return Subtract(lhs, rhs);
            }
            catch (InvalidOperationException)
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

        #region Multiply
        /// <summary>
        /// Multiply the vector by a scalar value.
        /// </summary>
        /// <param name="lhs">The vector to scale.</param>
        /// <param name="rhs">The scalar to multiply by.</param>
        /// <returns>The resulting vector.</returns>
        public static VecG<T> Multiply(VecG<T> lhs, T rhs)
        {
            var r = lhs;
            for (int i = 0; i < lhs.Size; i++)
            {
                r[i] *= rhs;
            }

            return r;
        }

        /// <summary>
        /// Multiply the individual components of the vectors.
        /// </summary>
        /// <param name="lhs">Left vector to multiply.</param>
        /// <param name="rhs">Right vector to multiply.</param>
        /// <returns>The resulting vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vectors are different sizes.</exception>
        public static VecG<T> Multiply(VecG<T> lhs, VecG<T> rhs)
        {
            if (lhs.Size != rhs.Size)
            {
                throw new InvalidOperationException("Both vectors must have the same size.");
            }

            var r = lhs;
            for (int i = 0; i < lhs.Size; i++)
            {
                r[i] *= rhs[i];
            }
            return r;
        }

        /// <summary>
        /// Define the multiplication operator for a vector with a scalar.
        /// </summary>
        /// <param name="lhs">The left vector to multiply.</param>
        /// <param name="rhs">The scalar to multiply by.</param>
        /// <returns>The resulting vector.</returns>
        public static VecG<T> operator *(VecG<T> lhs, T rhs)
        {
            return Multiply(lhs, rhs);
        }

        /// <summary>
        /// Define the multiplication operator for a vector component wise with another vector.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The resulting vector.</returns>
        public static VecG<T> operator *(VecG<T> lhs, VecG<T> rhs)
        {
            try
            {
                return Multiply(lhs, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        #endregion

        #region Divide
        /// <summary>
        /// Divide a vector by a scalar value.
        /// </summary>
        /// <param name="lhs">The vector to divide.</param>
        /// <param name="rhs">The scalar to divide by.</param>
        /// <returns>The resulting vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the divisor is zero.</exception>
        public static VecG<T> Divide(VecG<T> lhs, T rhs)
        {
            if (rhs.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to divide a vector by zero.");
            }

            var r = lhs;
            for (int i = 0; i < lhs.Size; i++)
            {
                r[i] /= rhs;
            }

            return r;
        }

        /// <summary>
        /// Divide the vector by a scalar value.
        /// </summary>
        /// <param name="rhs">The scalar to divide by.</param>
        /// <returns>The resulting vector.</returns>
        public VecG<T> Divide(T rhs)
        {
            try
            {
                return Divide(this, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Define the division operator for a vector and a scalar.
        /// </summary>
        /// <param name="lhs">The vector operand.</param>
        /// <param name="rhs">The scalar operand.</param>
        /// <returns>The resulting vector.</returns>
        public static VecG<T> operator /(VecG<T> lhs, T rhs)
        {
            try
            {
                return Divide(lhs, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        #endregion

        #region Dot
        /// <summary>
        /// Get the Dot product of two vectors of the same size.
        /// </summary>
        /// <param name="lhs">The left vector.</param>
        /// <param name="rhs">The right vector.</param>
        /// <returns>The dot product of the vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vectors are of different sizes.</exception>
        public static T Dot(VecG<T> lhs, VecG<T> rhs)
        {
            if (lhs.Size != rhs.Size)
            {
                throw new InvalidOperationException("Both vectors must have the same size.");
            }

            T result = T.Zero;
            for (int i = 0; i <= lhs.Size; i++)
            {
                result += lhs[i] * rhs[i];
            }

            return result;
        }

        /// <summary>
        /// Get the Dot product of two vector of the same size.
        /// </summary>
        /// <param name="rhs">The second vector.</param>
        /// <returns>The dot product.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vectors are of different sizes.</exception>
        public T Dot(VecG<T> rhs)
        {
            try
            {
                return Dot(this, rhs);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }
        #endregion

        #region Cross
        /// <summary>
        /// Get the cross product of two vectors. Only defined for Vectors in R³ or R⁷.
        /// </summary>
        /// <param name="lhs">The left operand.</param>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The resulting vector.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the vectors are different sizes.</exception>
        /// <exception cref="UndefinedMathOperationException">Thrown for vectors of size other than 3 and 7.</exception>
        public static VecG<T> Cross(VecG<T> lhs, VecG<T> rhs)
        {
            if (lhs.Size != rhs.Size)
                throw new InvalidOperationException("Both vectors must be of the same size.");

            switch (lhs.Size)
            {
                case 3:
                    {
                        var r = new VecG<T>(3);
                        r[0] = lhs[1] * rhs[2] - lhs[2] * rhs[1];
                        r[1] = lhs[2] * rhs[0] - lhs[0] * rhs[2];
                        r[2] = lhs[0] * rhs[1] - lhs[1] * rhs[0];
                        return r;
                    }
                case 7:
                    throw new NotImplementedException("Cross product in R⁷ not yet implemented.");//todo: R⁷ Cross product
                default:
                    throw new UndefinedMathOperationException("Cross product is undefined for vectors of size other than three or seven.");
            }
        }

        /// <summary>
        /// Get the cross product of this vector and another. Only defined for Vectors in R³ or R⁷.
        /// </summary>
        /// <param name="rhs">The right operand.</param>
        /// <returns>The resulting vector.</returns>
        public VecG<T> Cross(VecG<T> rhs)
        {
            try
            {
                return Cross(this, rhs);
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region ExceptionBuilders
        private static InvalidOperationException InvalidOpExceptionAsStructBuilder(int size, string structName)
        {
            return new InvalidOperationException(string.Format("Vector of size: {0} does not fit in {1} under strict usage.", size, structName));
        }
        #endregion
    }
}
