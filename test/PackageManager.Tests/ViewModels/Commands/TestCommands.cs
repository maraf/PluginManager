using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    [TestClass]
    public class TestCommands
    {
        [TestMethod]
        public void Install()
        {
            const string extractPath = @"D:\";

            var package = new Package(extractPath, "Test");
            var install = new InstallService(extractPath, package);

            var command = new InstallCommand(install.Object);
            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(package.IsGetContentCalled);
            Assert.IsTrue(package.IsExtractToAsyncCalled);
            Assert.IsTrue(install.IsInstallCalled);
        }

        [TestMethod]
        public void Uninstall()
        {
            const string extractPath = @"D:\";

            var package = new Package(extractPath, "Test");
            var install = new InstallService(extractPath, null, package, package);

            var command = new UninstallCommand(install.Object, new SelfPackageConfiguration(null));
            Assert.IsTrue(command.CanExecute(package.Object));

            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(package.IsGetContentCalled);
            Assert.IsTrue(package.IsRemoveFromAsyncCalled);
            Assert.IsTrue(install.IsIsInstalledCalled);
            Assert.IsTrue(install.IsUninstallCalled);
        }

        [TestMethod]
        public void UninstallSelf()
        {
            const string extractPath = @"D:\";

            var package = new Package(extractPath, "Test");
            var install = new InstallService(extractPath, null, package, package);

            var command = new UninstallCommand(install.Object, new SelfPackageConfiguration("Test"));
            Assert.IsFalse(command.CanExecute(package.Object));
        }
    }
}
