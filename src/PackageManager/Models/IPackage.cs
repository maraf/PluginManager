using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A package.
    /// </summary>
    public interface IPackage : IPackageIdentity, IEquatable<IPackage>
    {
        /// <summary>
        /// Gets a package description.
        /// </summary>
        string Description { get; }


        /// <summary>
        /// Gets package authors.
        /// </summary>
        string Authors { get; }

        /// <summary>
        /// Gets a date of publish.
        /// </summary>
        DateTime? Published { get; }

        /// <summary>
        /// Gets defined tags.
        /// </summary>
        string Tags { get; }


        /// <summary>
        /// Gets a path to the package icon.
        /// </summary>
        Uri IconUrl { get; }

        /// <summary>
        /// Gets an URL to the package project.
        /// </summary>
        Uri ProjectUrl { get; }

        /// <summary>
        /// Gets an URL to the package license text.
        /// </summary>
        Uri LicenseUrl { get; }


        /// <summary>
        /// Gets a content of the package.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A continuation task returning a content of the package.</returns>
        Task<IPackageContent> GetContentAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets an enumeration of all available package versions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A continutaion task returning an enumeration of all available package versions.</returns>
        Task<IEnumerable<IPackage>> GetVersionsAsync(CancellationToken cancellationToken);
    }
}
