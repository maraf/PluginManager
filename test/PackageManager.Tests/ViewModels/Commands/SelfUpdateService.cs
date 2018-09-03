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
    public class SelfUpdateService
    {
        public ISelfUpdateService Object { get; }
        public bool IsIsSelfUpdateCalled { get; private set; }
        public bool IsUpdateCalled { get; private set; }
        public bool IsRunNewInstanceCalled { get; private set; }

        public SelfUpdateService(bool isSelfUpdate, Package updatePackage)
        {
            Mock<ISelfUpdateService> mock = new Mock<ISelfUpdateService>();

            mock
                .SetupGet(s => s.IsSelfUpdate)
                .Callback(() => IsIsSelfUpdateCalled = true)
                .Returns(isSelfUpdate);

            mock
                .Setup(s => s.Update(It.Is<IPackage>(p => p == updatePackage.Object)))
                .Callback(() => IsUpdateCalled = true);

            mock
                .Setup(s => s.RunNewInstance(It.Is<IPackage>(p => p == updatePackage.Object)))
                .Callback(() => IsRunNewInstanceCalled = true);

            Object = mock.Object;
        }
    }
}
