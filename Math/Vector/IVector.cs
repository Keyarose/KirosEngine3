namespace KirosEngine3.Math.Vector
{
    /// <summary>
    /// Defines the non static requirements of a vertex implementation.
    /// </summary>
    /// <typeparam name="VecType">The vector's type.</typeparam>
    public interface IVector<VecType>
    {
        /// <summary>
        /// The length of the vector.
        /// </summary>
        float Length { get; }

        /// <summary>
        /// The length of the vector squared, slightly faster than getting the length directly
        /// </summary>
        float LengthSqr { get; }

        /// <summary>
        /// Indicates whether the current Vector is equal to another within the provided tolerance.
        /// </summary>
        /// <param name="other">The vector to compare.</param>
        /// <param name="tolerance">The allowed difference between the values.</param>
        /// <returns>True if the difference between the two matrices is less than the tolerance.</returns>
        bool Equals(VecType other, float tolerance);

        //todo: complete
    }
}