using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    /// <summary>
    /// A provider of current package sources.
    /// </summary>
    public interface IPackageSourceSelector
    {
        /// <summary>
        /// Gets an enumeration of current package sources.
        /// </summary>
        IEnumerable<IPackageSource> Sources { get; }
    }
}
