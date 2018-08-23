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
    internal class ShutdownExceptionHandler : IExceptionHandler<Exception>
    {
        private readonly App application;

        public ShutdownExceptionHandler(App application)
        {
            Ensure.NotNull(application, "application");
            this.application = application;
        }

        void IExceptionHandler<Exception>.Handle(Exception exception) => application.Shutdown();
    }
}
