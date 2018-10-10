using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A package source endpoint.
    /// </summary>
    public interface IPackageSource
    {
        /// <summary>
        /// Gets a user defined name of package source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a URL of package source.
        /// </summary>
        string Url { get; }
    }
}
