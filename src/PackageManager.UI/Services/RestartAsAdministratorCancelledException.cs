using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    /// <summary>
    /// An exception raised when attempt to restart the application as administrator was cancelled by the user.
    /// </summary>
    [Serializable]
    public class RestartAsAdministratorCancelledException : Exception
    {
        /// <summary>
        /// Creates new instance with the <paramref name="inner"/> exception.
        /// </summary>
        /// <param name="inner">The inner cause of the exceptional state.</param>
        public RestartAsAdministratorCancelledException(Exception inner)
            : base("Attempt to restart the application as administrator was cancelled by the user.", inner)
        { }

        /// <summary>
        /// Creates new instance for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected RestartAsAdministratorCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
