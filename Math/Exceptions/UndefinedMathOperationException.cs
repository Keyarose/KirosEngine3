using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Math.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a mathematical operation is undefined.
    /// </summary>
    public class UndefinedMathOperationException : Exception
    {
        /// <summary>
        /// Initialize a new instance of the UndefinedMathOperationException.
        /// </summary>
        public UndefinedMathOperationException() : base() { }

        /// <summary>
        /// Initialize a new instance of the UndefinedMathOperationException with the specified message.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public UndefinedMathOperationException(string message) : base(message) { }

        /// <summary>        
        /// Initialize a new instance of the UndefinedMathOperationException with the specified message, and a reference to the inner
        /// exception that is the cause of this one.
        /// </summary>
        /// <param name="message">The message for teh exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public UndefinedMathOperationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
