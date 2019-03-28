using Neptuo;
using Neptuo.Exceptions.Handlers;
using NuGet.Configuration;
using NuGet.Packaging;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Exceptions
{
    internal class UnauthorizedExceptionHandler : 
        IExceptionHandler<IExceptionHandlerContext<UnauthorizedAccessException>>, 
        IExceptionHandler<IExceptionHandlerContext<NuGetConfigurationException>>, 
        IExceptionHandler<IExceptionHandlerContext<PackagesConfigWriterException>>
    {
        private readonly ProcessService processService;

        public UnauthorizedExceptionHandler(ProcessService processService)
        {
            Ensure.NotNull(processService, "processService");
            this.processService = processService;
        }

        private void HandleInternal<T>(IExceptionHandlerContext<T> context)
            where T : Exception
        {
            processService.RestartAsAdministrator();
            context.IsHandled = true;
        }

        void IExceptionHandler<IExceptionHandlerContext<UnauthorizedAccessException>>.Handle(IExceptionHandlerContext<UnauthorizedAccessException> context) => HandleInternal(context);
        void IExceptionHandler<IExceptionHandlerContext<PackagesConfigWriterException>>.Handle(IExceptionHandlerContext<PackagesConfigWriterException> context) => HandleInternal(context);
        void IExceptionHandler<IExceptionHandlerContext<NuGetConfigurationException>>.Handle(IExceptionHandlerContext<NuGetConfigurationException> context) => HandleInternal(context);
    }
}
