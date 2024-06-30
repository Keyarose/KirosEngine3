using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a collection was not properly cleaned up when related items were disposed of.
    /// </summary>
    public class CollectionCleanupException : Exception
    {
        /// <summary>
        /// The name of the collection that is why the exception was thrown.
        /// </summary>
        public string? CollectionName { get; }

        /// <inheritdoc/>
        public CollectionCleanupException() : base() { }

        /// <inheritdoc/>
        public CollectionCleanupException(string message) : base(message) { }

        /// <inheritdoc/>
        public CollectionCleanupException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initialize a new instance of the CollectionCleanupException class with a specified error message, and the name of the related collection.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="collectionName">The name of the collection that caused the error.</param>
        public CollectionCleanupException(string message, string collectionName) : base(message)
        {
            CollectionName = collectionName;
        }

        /// <summary>
        /// Initialize a new instance of the CollectionCleanupException class with a specified error message, an inner exception, and the name of the related collection.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="collectionName">The name of the collection that caused the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CollectionCleanupException(string message, string collectionName, Exception innerException) : base(message, innerException)
        {
            CollectionName = collectionName;
        }
    }
}
