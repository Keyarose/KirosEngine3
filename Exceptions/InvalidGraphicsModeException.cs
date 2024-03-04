using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Exceptions
{
    /// <summary>
    /// Defines an exception for when the graphics mode is in an invalid state
    /// </summary>
    internal class InvalidGraphicsModeException : Exception
    {
        public InvalidGraphicsModeException() { }

        public InvalidGraphicsModeException(string message) : base(message) { }

        public InvalidGraphicsModeException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
