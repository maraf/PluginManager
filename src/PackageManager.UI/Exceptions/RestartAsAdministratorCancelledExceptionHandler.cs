using Neptuo;
using Neptuo.Exceptions.Handlers;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Exceptions
{
    public class RestartAsAdministratorCancelledExceptionHandler : IExceptionHandler<IExceptionHandlerContext<RestartAsAdministratorCancelledException>>
    {
        private readonly Navigator navigator;
        private readonly App application;

        internal RestartAsAdministratorCancelledExceptionHandler(Navigator navigator, App application)
        {
            Ensure.NotNull(navigator, "navigator");
            Ensure.NotNull(application, "application");
            this.navigator = navigator;
            this.application = application;
        }

        public void Handle(IExceptionHandlerContext<RestartAsAdministratorCancelledException> context)
        {
            context.IsHandled = true;

            navigator.Notify("Unauthorized", "The operation requires elevated privilege which has not been given.", Navigator.MessageType.Error);
            application.Shutdown();
        }
    }
}
