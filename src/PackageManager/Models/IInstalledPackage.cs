using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    /// <summary>
    /// A model of installed package.
    /// </summary>
    public interface IInstalledPackage
    {
        /// <summary>
        /// Gets a package definition.
        /// </summary>
        IPackage Definition { get; }

        /// <summary>
        /// Gets <c>true</c> when package is compatible with current application version; <c>false</c> otherwise.
        /// </summary>
        bool IsCompatible { get; }
    }
}
