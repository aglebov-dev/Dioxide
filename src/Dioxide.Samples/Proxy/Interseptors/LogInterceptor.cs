using System;
using Dioxide.Contracts;
using Dioxide.Samples.Dependencies;

namespace Dioxide.Samples.Proxy.Interseptors
{
    public class LogInterceptor : IProxyInterceptor
    {
        private readonly ILogger _logger;

        public LogInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Enter(string methodName, IMethodArgs args)
        {
            _logger.Log($"Enter {methodName}");
        }

        public void Exit(string methodName, IMethodArgs args, IMethodResult result)
        {
            _logger.Log($"Exit {methodName}");
        }

        public void Finaly(string methodName, IMethodArgs args, IMethodResult result)
        {
            _logger.Log($"Finaly {methodName}");
        }

        public Exception Catch(string methodName, IMethodArgs args, Exception exception)
        {
            _logger.Log($"Catch {methodName}");
            return exception;
        }
    }
}
