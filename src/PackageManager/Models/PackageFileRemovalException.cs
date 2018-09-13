using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// An exception raised when there was an removing file from installed package.
    /// </summary>
    [Serializable]
    public class PackageFileRemovalException : Exception
    {
        /// <summary>
        /// Gets a real file path where removal problem occured.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Creates a new instance where <paramref name="filePath"/> caused the problem and <paramref name="inner"/> exception.
        /// </summary>
        /// <param name="filePath">A real file path where removal problem occured.</param>
        /// <param name="inner">An inner cause of the exceptional state.</param>
        public PackageFileRemovalException(string filePath, Exception inner)
            : base($"Error extracting file to '{filePath}'.", inner)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Creates a new instance for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected PackageFileRemovalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
