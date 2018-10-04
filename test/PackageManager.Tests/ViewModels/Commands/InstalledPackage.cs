using Moq;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class InstalledPackage
    {
        public IInstalledPackage Object { get; }

        public InstalledPackage(IPackage definition, bool isCompatible = true)
        {
            Mock<IInstalledPackage> mock = new Mock<IInstalledPackage>();
            mock
                .SetupGet(p => p.Definition)
                .Returns(definition);

            mock
                .SetupGet(p => p.IsCompatible)
                .Returns(isCompatible);

            Object = mock.Object;
        }
    }
}
