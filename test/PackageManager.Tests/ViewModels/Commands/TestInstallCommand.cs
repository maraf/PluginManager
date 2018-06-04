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
    public class TestInstallCommand
    {
        [TestMethod]
        public void Base()
        {
            bool isGetContentCalled = false;
            bool isExtractToAsyncCalled = false;
            bool isInstallCalled = false;

            Mock<IPackageContent> packageContent = new Mock<IPackageContent>();
            packageContent
                .Setup(pc => pc.ExtractToAsync(It.Is<string>(s => s == @"D:\"), It.IsAny<CancellationToken>()))
                .Callback(() => isExtractToAsyncCalled = true)
                .Returns(() => Task.CompletedTask);

            Mock<IPackage> package = new Mock<IPackage>();
            package
                .Setup(p => p.GetContentAsync(It.IsAny<CancellationToken>()))
                .Callback(() => isGetContentCalled = true)
                .Returns(() => Task.FromResult(packageContent.Object));

            Mock<IInstallService> install = new Mock<IInstallService>();
            install
                .Setup(i => i.Install(It.Is<IPackage>(p => p == package.Object)))
                .Callback(() => isInstallCalled = true);

            install
                .Setup(i => i.Path)
                .Returns(() => @"D:\");

            InstallCommand command = new InstallCommand(install.Object);
            command.ExecuteAsync(package.Object).Wait();

            Assert.IsTrue(isGetContentCalled);
            Assert.IsTrue(isExtractToAsyncCalled);
            Assert.IsTrue(isInstallCalled);
        }
    }
}
