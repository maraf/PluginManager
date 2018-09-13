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
    public interface IPackage
    {
        /// <summary>
        /// Gets an unique package identifier.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a package version.
        /// </summary>
        string Version { get; }

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
        /// <returns>A continuation task returning </returns>
        Task<IPackageContent> GetContentAsync(CancellationToken cancellationToken);
    }
}
