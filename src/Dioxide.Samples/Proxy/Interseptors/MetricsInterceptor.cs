using System;
using System.Diagnostics;
using Dioxide.Contracts;
using Dioxide.Samples.Dependencies;

namespace Dioxide.Samples.Proxy.Interseptors
{
    public class MetricsInterceptor : IProxyInterceptor
    {
        private Stopwatch stopwatch;
        private readonly ILogger _logger;

        public MetricsInterceptor(ILogger logger)
        {
            _logger = logger;
        }

        public void Enter(string methodName, IMethodArgs args)
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void Exit(string methodName, IMethodArgs args, IMethodResult result)
        {
            _logger.Log($"Successful method {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void Finaly(string methodName, IMethodArgs args, IMethodResult result) 
        { 
            stopwatch.Stop();  
        }

        public Exception Catch(string methodName, IMethodArgs args, Exception exception)
        {
            _logger.Log($"Failed method {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
            return exception;
        }
    }
}
