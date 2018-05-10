using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public interface IPackage
    {
        string Id { get; }
        string Version { get; }
        string Description { get; }
        Uri IconUrl { get; }

        IReadOnlyCollection<IPackage> Dependecies { get; }

        // TODO: Download and Extract.
    }
}
