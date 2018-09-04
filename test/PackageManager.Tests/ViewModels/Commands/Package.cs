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
        public CallCounter GetContentCalled { get; } = new CallCounter();
        public CallCounter ExtractToAsyncCalled { get; } = new CallCounter();
        public CallCounter RemoveFromAsyncCalled { get; } = new CallCounter();
        public IPackage Object { get; }

        public Package(string extractPath, string id)
        {
            Mock<IPackageContent> contentMock = new Mock<IPackageContent>();
            contentMock
                .Setup(pc => pc.ExtractToAsync(It.Is<string>(s => s == extractPath), It.IsAny<CancellationToken>()))
                .Callback(() => ExtractToAsyncCalled.Increment())
                .Returns(() => Task.CompletedTask);

            contentMock
                .Setup(pc => pc.RemoveFromAsync(It.Is<string>(s => s == extractPath), It.IsAny<CancellationToken>()))
                .Callback(() => RemoveFromAsyncCalled.Increment())
                .Returns(() => Task.CompletedTask);

            Mock<IPackage> mock = new Mock<IPackage>();
            mock
                .Setup(p => p.GetContentAsync(It.IsAny<CancellationToken>()))
                .Callback(() => GetContentCalled.Increment())
                .Returns(() => Task.FromResult(contentMock.Object));

            mock
                .SetupGet(p => p.Id)
                .Returns(id);

            Object = mock.Object;
        }
    }
}
