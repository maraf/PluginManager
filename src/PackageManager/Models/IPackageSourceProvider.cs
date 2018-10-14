using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A provider of package sources.
    /// A single primary could be selected or if not all defined should be used.
    /// </summary>
    public interface IPackageSourceProvider
    {
        /// <summary>
        /// Gets a primary source.
        /// Can returns <c>null</c> - if it does, <see cref="All"/> should be used.
        /// </summary>
        IPackageSource Primary { get; }

        /// <summary>
        /// Gets a collection of all defined sources.
        /// </summary>
        IReadOnlyCollection<IPackageSource> All { get; }

        /// <summary>
        /// An event raised when state of this provider has changed.
        /// </summary>
        event Action Changed;
    }
}
