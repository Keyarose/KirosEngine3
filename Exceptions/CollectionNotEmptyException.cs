using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Exceptions
{
    /// <summary>
    /// Defines an exception to be thrown when a collection is not empty when it is intended to be due to some logic error
    /// </summary>
    internal class CollectionNotEmptyException : Exception
    {
        public CollectionNotEmptyException() { }

        public CollectionNotEmptyException(string message) : base(message) { }

        public CollectionNotEmptyException(string message,  Exception innerException) : base(message, innerException) { }
    }
}
