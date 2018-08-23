using Neptuo;
using Neptuo.Exceptions.Handlers;
using NuGet.Protocol.Core.Types;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Exceptions
{
    internal class NuGetFatalProtocolExceptionHandler : IExceptionHandler<IExceptionHandlerContext<FatalProtocolException>>
    {
        private readonly Navigator navigator;

        public NuGetFatalProtocolExceptionHandler(Navigator navigator)
        {
            Ensure.NotNull(navigator, "navigator");
            this.navigator = navigator;
        }

        void IExceptionHandler<IExceptionHandlerContext<FatalProtocolException>>.Handle(IExceptionHandlerContext<FatalProtocolException> context)
        {
            if (context.Exception.InnerException is HttpRequestException && context.Exception.InnerException.InnerException is WebException webException)
            {
                navigator.Message("Communcation Error", GetMessage(webException), Navigator.MessageType.Error);
                context.IsHandled = true;
            }
        }

        private string GetMessage(WebException exception)
        {
            switch (exception.Status)
            {
                case WebExceptionStatus.NameResolutionFailure:
                    return "Error resolving the host name.";
                case WebExceptionStatus.ConnectFailure:
                    return "Error opening the connection to the server.";
                case WebExceptionStatus.ReceiveFailure:
                    return "Error receiving data over the connection.";
                case WebExceptionStatus.RequestCanceled:
                    return "Request has been canceled.";
                case WebExceptionStatus.ConnectionClosed:
                    return "The connection has been unexpectedly closed.";
                case WebExceptionStatus.Timeout:
                    return "The connection has timed out.";
                case WebExceptionStatus.ProxyNameResolutionFailure:
                    return "Error resolving proxy name.";
                case WebExceptionStatus.Success:
                case WebExceptionStatus.SendFailure:
                case WebExceptionStatus.PipelineFailure:
                case WebExceptionStatus.ProtocolError:
                case WebExceptionStatus.TrustFailure:
                case WebExceptionStatus.SecureChannelFailure:
                case WebExceptionStatus.ServerProtocolViolation:
                case WebExceptionStatus.KeepAliveFailure:
                case WebExceptionStatus.Pending:
                case WebExceptionStatus.UnknownError:
                case WebExceptionStatus.MessageLengthLimitExceeded:
                case WebExceptionStatus.CacheEntryNotFound:
                case WebExceptionStatus.RequestProhibitedByCachePolicy:
                case WebExceptionStatus.RequestProhibitedByProxy:
                default:
                    return "An unknown error has occured while communicating over the wire.";
            }
        }
    }
}
