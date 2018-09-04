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
        public CallCounter IsSelfUpdateCalled { get; } = new CallCounter();
        public CallCounter UpdateCalled { get; } = new CallCounter();
        public CallCounter RunNewInstanceCalled { get; } = new CallCounter();

        public List<IPackage> UpdatePackages { get; } = new List<IPackage>();

        public SelfUpdateService(bool isSelfUpdate, Package updatePackage)
        {
            Mock<ISelfUpdateService> mock = new Mock<ISelfUpdateService>();

            mock
                .SetupGet(s => s.IsSelfUpdate)
                .Callback(() => IsSelfUpdateCalled.Increment())
                .Returns(isSelfUpdate);

            mock
                .Setup(s => s.Update(It.Is<IPackage>(p => p == UpdatePackages[UpdateCalled])))
                .Callback(() => UpdateCalled.Increment());

            mock
                .Setup(s => s.RunNewInstance(It.Is<IPackage>(p => p == UpdatePackages[RunNewInstanceCalled])))
                .Callback(() => RunNewInstanceCalled.Increment());

            if (updatePackage != null)
                UpdatePackages.Add(updatePackage.Object);

            Object = mock.Object;
        }
    }
}
