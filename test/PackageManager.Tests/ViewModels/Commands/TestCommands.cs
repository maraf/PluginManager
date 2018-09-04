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

            Assert.IsTrue(package.GetContentCalled);
            Assert.IsTrue(package.ExtractToAsyncCalled);
            Assert.IsTrue(install.InstallCalled);
        }

        [TestMethod]
        public void Uninstall()
        {
            var package = new Package(ExtractPath, "Test");
            var install = new InstallService(ExtractPath, null, package, package);

            var command = new UninstallCommand(install.Object, new SelfPackageConfiguration(null));
            Assert.IsTrue(command.CanExecute(package.Object));

            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(package.GetContentCalled);
            Assert.IsTrue(package.RemoveFromAsyncCalled);
            Assert.IsTrue(install.InstalledCalled);
            Assert.IsTrue(install.UninstallCalled);
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

            Assert.IsFalse(selfUpdate.IsSelfUpdateCalled);
            Assert.IsFalse(selfUpdate.RunNewInstanceCalled);
            Assert.IsTrue(install.UninstallCalled);
            Assert.IsTrue(install.InstallCalled);
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

            Assert.IsTrue(selfUpdate.IsSelfUpdateCalled);
            Assert.IsTrue(selfUpdate.UpdateCalled);
            Assert.IsFalse(selfUpdate.RunNewInstanceCalled);
            Assert.IsFalse(install.UninstallCalled);
            Assert.IsFalse(install.InstallCalled);
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

            Assert.IsTrue(selfUpdate.IsSelfUpdateCalled);
            Assert.IsFalse(selfUpdate.UpdateCalled);
            Assert.IsTrue(selfUpdate.RunNewInstanceCalled);
            Assert.IsTrue(install.UninstallCalled);
            Assert.IsTrue(install.InstallCalled);
        }

        [TestMethod]
        public void UpdateAll()
        {
            var packageA = new Package(ExtractPath, "A");
            var packageB = new Package(ExtractPath, "B");
            var packageC = new Package(ExtractPath, "C");

            PackageUpdateViewModel ToUpdate(Package package) => new PackageUpdateViewModel(package.Object, package.Object, false);

            var install = new InstallService(ExtractPath, packageA, packageA, packageA);
            install.InstallPackages.Add(packageB.Object);
            install.InstallPackages.Add(packageC.Object);
            install.UninstallPackages.Add(packageB.Object);
            install.UninstallPackages.Add(packageC.Object);
            install.InstalledPackages.Add(packageB.Object);
            install.InstalledPackages.Add(packageC.Object);

            var selfUpdate = new SelfUpdateService(false, packageA);
            selfUpdate.UpdatePackages.Add(packageB.Object);
            selfUpdate.UpdatePackages.Add(packageC.Object);

            var innerCommand = new UpdateCommand(install.Object, selfUpdate.Object);

            var viewModel = new UpdateAllCommandViewModel(innerCommand, new List<PackageUpdateViewModel>() { ToUpdate(packageA), ToUpdate(packageB), ToUpdate(packageC) });
            var command = new UpdateAllCommand(viewModel.Object);

            Assert.IsTrue(command.CanExecute());

            command.ExecuteAsync().Wait();

            Assert.AreEqual(3, install.InstallCalled);
            Assert.AreEqual(3, install.UninstallCalled);
            Assert.AreEqual(3, install.InstalledCalled);

            Assert.AreEqual(0, selfUpdate.IsSelfUpdateCalled);
        }
    }
}
