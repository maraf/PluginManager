using Neptuo;
using Neptuo.Exceptions.Handlers;
using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Exceptions
{
    internal class PackageInstallExceptionHandler :
        IExceptionHandler<IExceptionHandlerContext<PackageFileExtractionException>>,
        IExceptionHandler<IExceptionHandlerContext<PackageFileRemovalException>>
    {
        private readonly Navigator navigator;

        public PackageInstallExceptionHandler(Navigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        void IExceptionHandler<IExceptionHandlerContext<PackageFileExtractionException>>.Handle(IExceptionHandlerContext<PackageFileExtractionException> context)
        {
            navigator.Notify("Package Install Error", $"Error extracting file to '{context.Exception.FilePath}'", Navigator.MessageType.Error);
            context.IsHandled = true;
        }

        void IExceptionHandler<IExceptionHandlerContext<PackageFileRemovalException>>.Handle(IExceptionHandlerContext<PackageFileRemovalException> context)
        {
            navigator.Notify("Package Removal Error", $"Error deleting file to '{context.Exception.FilePath}'", Navigator.MessageType.Error);
            context.IsHandled = true;
        }
    }
}
