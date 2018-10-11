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
        /// Adds a source with <paramref name="name"/> and <paramref name="uri"/> to the collection.
        /// </summary>
        /// <param name="name">A name of the new source.</param>
        /// <param name="name">An uri of the new source.</param>
        IPackageSource Add(string name, Uri uri);

        /// <summary>
        /// Removes <paramref name="source"/> from the collection.
        /// </summary>
        /// <param name="source">A source to remove.</param>
        void Remove(IPackageSource source);
    }
}
