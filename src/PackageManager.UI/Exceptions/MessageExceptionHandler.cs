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
    internal class MessageExceptionHandler : IExceptionHandler<IExceptionHandlerContext<Exception>>
    {
        private readonly Navigator navigator;

        public MessageExceptionHandler(Navigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        public void Handle(IExceptionHandlerContext<Exception> context)
        {
            StringBuilder message = new StringBuilder();

            string exceptionMessage = context.Exception.ToString();
            if (exceptionMessage.Length > 800)
                exceptionMessage = exceptionMessage.Substring(0, 800);

            message.AppendLine(exceptionMessage);

            bool result = navigator.Confirm("Unhandled exception - Do you want to kill the aplication?", message.ToString(), Navigator.MessageType.Error);
            if (!result)
                context.IsHandled = true;
        }
    }
}
