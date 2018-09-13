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
        public CallCounter InstallCalled { get; } = new CallCounter();
        public CallCounter UninstallCalled { get; } = new CallCounter();
        public CallCounter IsInstalledCalled { get; } = new CallCounter();

        public List<IPackage> InstallPackages { get; } = new List<IPackage>();
        public List<IPackage> UninstallPackages { get; } = new List<IPackage>();
        public List<IPackage> IsInstalledPackages { get; } = new List<IPackage>();

        public InstallService(string extractPath, Package installPackage = null, Package uninstallPackage = null, Package isInstalledPackage = null)
        {
            Mock<IInstallService> mock = new Mock<IInstallService>();

            mock
                .Setup(i => i.Install(It.Is<IPackage>(p => p == InstallPackages[InstallCalled])))
                .Callback(() => InstallCalled.Increment());

            mock
                .Setup(i => i.Uninstall(It.Is<IPackage>(p => p == UninstallPackages[UninstallCalled])))
                .Callback(() => UninstallCalled.Increment());

            mock
                .Setup(i => i.IsInstalled(It.Is<IPackage>(p => p == IsInstalledPackages[IsInstalledCalled])))
                .Callback(() => IsInstalledCalled.Increment())
                .Returns(true);

            mock
                .Setup(i => i.Path)
                .Returns(() => extractPath);

            if (installPackage != null)
                InstallPackages.Add(installPackage.Object);

            if (uninstallPackage != null)
                UninstallPackages.Add(uninstallPackage.Object);

            if (isInstalledPackage != null)
                IsInstalledPackages.Add(isInstalledPackage.Object);

            Object = mock.Object;
        }
    }
}
