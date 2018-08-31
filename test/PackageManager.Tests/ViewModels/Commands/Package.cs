using Moq;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class Package
    {
        public bool IsGetContentCalled { get; private set; }
        public bool IsExtractToAsyncCalled { get; private set; }
        public bool IsRemoveFromAsyncCalled { get; private set; }
        public IPackage Object { get; }

        public Package(string extractPath, string id)
        {
            Mock<IPackageContent> contentMock = new Mock<IPackageContent>();
            contentMock
                .Setup(pc => pc.ExtractToAsync(It.Is<string>(s => s == extractPath), It.IsAny<CancellationToken>()))
                .Callback(() => IsExtractToAsyncCalled = true)
                .Returns(() => Task.CompletedTask);

            contentMock
                .Setup(pc => pc.RemoveFromAsync(It.Is<string>(s => s == extractPath), It.IsAny<CancellationToken>()))
                .Callback(() => IsRemoveFromAsyncCalled = true)
                .Returns(() => Task.CompletedTask);

            Mock<IPackage> mock = new Mock<IPackage>();
            mock
                .Setup(p => p.GetContentAsync(It.IsAny<CancellationToken>()))
                .Callback(() => IsGetContentCalled = true)
                .Returns(() => Task.FromResult(contentMock.Object));

            mock
                .SetupGet(p => p.Id)
                .Returns(id);

            Object = mock.Object;
        }
    }
}
