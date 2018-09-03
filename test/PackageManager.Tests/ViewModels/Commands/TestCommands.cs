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
        private const string ExtractPath = @"D:\";

        [TestMethod]
        public void Install()
        {
            var package = new Package(ExtractPath, "Test");
            var install = new InstallService(ExtractPath, package);

            var command = new InstallCommand(install.Object);
            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(package.IsGetContentCalled);
            Assert.IsTrue(package.IsExtractToAsyncCalled);
            Assert.IsTrue(install.IsInstallCalled);
        }

        [TestMethod]
        public void Uninstall()
        {
            var package = new Package(ExtractPath, "Test");
            var install = new InstallService(ExtractPath, null, package, package);

            var command = new UninstallCommand(install.Object, new SelfPackageConfiguration(null));
            Assert.IsTrue(command.CanExecute(package.Object));

            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(package.IsGetContentCalled);
            Assert.IsTrue(package.IsRemoveFromAsyncCalled);
            Assert.IsTrue(install.IsIsInstalledCalled);
            Assert.IsTrue(install.IsUninstallCalled);
        }

        [TestMethod]
        public void Uninstall_Self()
        {
            var package = new Package(ExtractPath, "Test");
            var install = new InstallService(ExtractPath, null, package, package);

            var command = new UninstallCommand(install.Object, new SelfPackageConfiguration("Test"));
            Assert.IsFalse(command.CanExecute(package.Object));
        }

        [TestMethod]
        public void Update()
        {
            var package = new Package(ExtractPath, "Test");
            var viewModel = new PackageUpdateViewModel(package.Object, package.Object, false);
            var install = new InstallService(ExtractPath, package, package, package);
            var selfUpdate = new SelfUpdateService(false, null);

            var command = new UpdateCommand(install.Object, selfUpdate.Object);

            Assert.IsTrue(command.CanExecute(viewModel));

            command.ExecuteAsync(viewModel).Wait();

            Assert.IsFalse(selfUpdate.IsIsSelfUpdateCalled);
            Assert.IsFalse(selfUpdate.IsRunNewInstanceCalled);
            Assert.IsTrue(install.IsUninstallCalled);
            Assert.IsTrue(install.IsInstallCalled);
        }

        [TestMethod]
        public void Update_Self()
        {
            var package = new Package(ExtractPath, "Test");
            var viewModel = new PackageUpdateViewModel(package.Object, package.Object, true);
            var install = new InstallService(ExtractPath, package, package, package);
            var selfUpdate = new SelfUpdateService(false, package);

            var command = new UpdateCommand(install.Object, selfUpdate.Object);

            Assert.IsTrue(command.CanExecute(viewModel));

            command.ExecuteAsync(viewModel).Wait();

            Assert.IsTrue(selfUpdate.IsIsSelfUpdateCalled);
            Assert.IsTrue(selfUpdate.IsUpdateCalled);
            Assert.IsFalse(selfUpdate.IsRunNewInstanceCalled);
            Assert.IsFalse(install.IsUninstallCalled);
            Assert.IsFalse(install.IsInstallCalled);
        }

        [TestMethod]
        public void Update_SelfComplete()
        {
            var package = new Package(ExtractPath, "Test");
            var viewModel = new PackageUpdateViewModel(package.Object, package.Object, true);
            var install = new InstallService(ExtractPath, package, package, package);
            var selfUpdate = new SelfUpdateService(true, package);

            var command = new UpdateCommand(install.Object, selfUpdate.Object);

            Assert.IsTrue(command.CanExecute(viewModel));

            command.ExecuteAsync(viewModel).Wait();

            Assert.IsTrue(selfUpdate.IsIsSelfUpdateCalled);
            Assert.IsFalse(selfUpdate.IsUpdateCalled);
            Assert.IsTrue(selfUpdate.IsRunNewInstanceCalled);
            Assert.IsTrue(install.IsUninstallCalled);
            Assert.IsTrue(install.IsInstallCalled);
        }
    }
}
