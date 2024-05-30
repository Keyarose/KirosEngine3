using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirosEngine3.Exceptions
{
    /// <summary>
    /// Defines an exception to be thrown when a required configuration value is missing.
    /// </summary>
    public class MissingConfigException : Exception
    {
        public MissingConfigException() { }

        public MissingConfigException(string message) : base(message) { }

        public MissingConfigException(string message, Exception innerException) : base(message, innerException) { }
    }
}
