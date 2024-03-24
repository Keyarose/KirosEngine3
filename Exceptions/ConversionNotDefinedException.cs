using System.Runtime.Serialization;

namespace KirosEngine3.Exceptions
{
    [Serializable]
    internal class ConversionNotDefinedException : Exception
    {
        public ConversionNotDefinedException()
        {
        }

        public ConversionNotDefinedException(string? message) : base(message)
        {
        }

        public ConversionNotDefinedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}