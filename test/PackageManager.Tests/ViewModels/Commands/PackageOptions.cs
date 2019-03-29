using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class PackageOptions
    {
        public IPackageOptions Object { get; }

        public PackageOptions()
        {
            Mock<IPackageOptions> mock = new Mock<IPackageOptions>();
            mock
                .Setup(p => p.IsPrereleaseIncluded)
                .Returns(() => false);

            Object = mock.Object;
        }
    }
}
