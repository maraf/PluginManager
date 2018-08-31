using Moq;
using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class InstallService
    {
        public IInstallService Object { get; }
        public bool IsInstallCalled { get; private set; }
        public bool IsUninstallCalled { get; private set; }
        public bool IsIsInstalledCalled { get; private set; }

        public InstallService(string extractPath, Package installPackage = null, Package uninstallPackage = null, Package installedPackage = null)
        {
            Mock<IInstallService> mock = new Mock<IInstallService>();

            if (installPackage != null)
            {
                mock
                    .Setup(i => i.Install(It.Is<IPackage>(p => p == installPackage.Object)))
                    .Callback(() => IsInstallCalled = true);
            }

            if (uninstallPackage != null)
            {
                mock
                    .Setup(i => i.Uninstall(It.Is<IPackage>(p => p == uninstallPackage.Object)))
                    .Callback(() => IsUninstallCalled = true);
            }

            if (installedPackage != null)
            {
                mock
                    .Setup(i => i.IsInstalled(It.Is<IPackage>(p => p == installedPackage.Object)))
                    .Callback(() => IsIsInstalledCalled = true)
                    .Returns(true);
            }

            mock
                .Setup(i => i.Path)
                .Returns(() => extractPath);

            Object = mock.Object;
        }
    }
}
