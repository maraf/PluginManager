using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A package content.
    /// </summary>
    public interface IPackageContent
    {
        /// <summary>
        /// Extracts all content from package to the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">A path to extract content to.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A continuation task.</returns>
        Task ExtractToAsync(string path, CancellationToken cancellationToken);

        /// <summary>
        /// Removes all package content from the <paramref name="path"/>.
        /// </summary>
        /// <param name="path">A path to remove content from.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A continuation task.</returns>
        Task RemoveFromAsync(string path, CancellationToken cancellationToken);
    }
}
