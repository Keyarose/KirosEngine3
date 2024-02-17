using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// General vector struct that can be defined for any size up to int.MaxValue and value type
    /// </summary>
    internal struct VectorG<T> where T : INumber<T>
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

        public readonly T Magnitude
        {
            get
            {
                return MathF.Sqrt((dynamic)LengthSqr);
            }
        }

        public readonly T Length
        {
            get
            {
                return Magnitude;
            }
        }

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

        public VectorG<T> Negate()
        {
            var c = this;
            for (int i = 0; i < Size; i++)
            {
                c[i] = -c[i];
            }
            return c;
        }

        #region Normalize
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

        public static VectorG<T> Normalize(VectorG<T> v)
        {
            if(v.Length.Equals(T.Zero))
            {
                throw new InvalidOperationException("Attempt to normalize a zero vector.");
            }
            T ratio = T.One / v.Length;
            var c = v;
            for(int i = 0; i < c.Size; i++)
            {
                c[i] *= ratio;
            }
            return c;
        }
        #endregion
        //todo: math functions
    }
}
