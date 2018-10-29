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
        /// REturns a builder for creating a new source.
        /// </summary>
        IPackageSourceBuilder Add();

        /// <summary>
        /// Returns builder for editing package source.
        /// </summary>
        /// <param name="source">A source to edit.</param>
        /// <returns>Builder for editing package source.</returns>
        IPackageSourceBuilder Edit(IPackageSource source);

        /// <summary>
        /// Removes <paramref name="source"/> from the collection.
        /// </summary>
        /// <param name="source">A source to remove.</param>
        void Remove(IPackageSource source);

        /// <summary>
        /// Moves <paramref name="source"/> by one up.
        /// </summary>
        /// <param name="source">A source to move up.</param>
        /// <returns>A new index.</returns>
        int MoveUp(IPackageSource source);

        /// <summary>
        /// Moves <paramref name="source"/> by one down.
        /// </summary>
        /// <param name="source">A source to move down.</param>
        /// <returns>A new index.</returns>
        int MoveDown(IPackageSource source);
    }
}
