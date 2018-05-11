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

        string Authors { get; }
        DateTime? Published { get; }
        string Tags { get; }

        Uri IconUrl { get; }
        Uri ProjectUrl { get; }
        Uri LicenseUrl { get; }

        // TODO: Download and Extract.
    }
}
