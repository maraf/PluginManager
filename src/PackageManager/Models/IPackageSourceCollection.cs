using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A mutable collection of defined package sources.
    /// </summary>
    public interface IPackageSourceCollection : IPackageSourceProvider
    {
        /// <summary>
        /// Sets <paramref name="source"/> as primary.
        /// If <paramref name="source"/> is <c>null</c>, removed any definition of primary source.
        /// </summary>
        /// <param name="source">A source to set as primary or <c>null</c> to clear this setting.</param>
        void MarkAsPrimary(IPackageSource source);

        /// <summary>
        /// Adds <paramref name="source"/> to collection.
        /// </summary>
        /// <param name="source">A source to add.</param>
        void Add(IPackageSource source);

        /// <summary>
        /// Removes <paramref name="source"/> from collection.
        /// </summary>
        /// <param name="source">A source to remove.</param>
        void Remove(IPackageSource source);
    }
}
