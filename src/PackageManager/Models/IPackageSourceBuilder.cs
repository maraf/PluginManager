using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A package source definition builder.
    /// </summary>
    public interface IPackageSourceBuilder
    {
        /// <summary>
        /// Sets a name of the source.
        /// </summary>
        /// <param name="name">A name of the source.</param>
        /// <returns>Self (for fluency).</returns>
        IPackageSourceBuilder Name(string name);

        /// <summary>
        /// Sets an URI of the source.
        /// </summary>
        /// <param name="uri">An URI of the source.</param>
        /// <returns>Self (for fluency).</returns>
        IPackageSourceBuilder Uri(Uri uri);

        /// <summary>
        /// Saves changes or creates a new source.
        /// </summary>
        /// <returns>Updated or created source.</returns>
        IPackageSource Save();
    }
}
