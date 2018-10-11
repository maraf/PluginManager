using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// Raised when passed implementation of <see cref="IPackageSource"/> to not compatible with <see cref="NuGetPackageSourceCollection"/>,
    /// </summary>
    [Serializable]
    public class InvalidPackageSourceImplementationException : Exception
    {
        /// <summary>
        /// Creates a new empty instance.
        /// </summary>
        public InvalidPackageSourceImplementationException()
        { }
        
        /// <summary>
        /// Creates a new instance for deserialization.
        /// </summary>
        /// <param name="info">A serialization info.</param>
        /// <param name="context">A streaming context.</param>
        protected InvalidPackageSourceImplementationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
