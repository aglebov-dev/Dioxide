using System;
using System.Diagnostics;
using Dioxide.Contracts;

namespace Dioxide.Samples.Proxy
{
    public class MetricsVisitor : IProxyInterceptor
    {
        private Stopwatch stopwatch;

        public void Enter(string methodName, IMethodArgs args)
        {
            stopwatch = Stopwatch.StartNew();
        }

        public void Exit(string methodName, IMethodArgs args, IMethodResult result)
        {
            Console.WriteLine($"Successful method {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void Finaly(string methodName, IMethodArgs args, IMethodResult result) { }

        public Exception Catch(string methodName, IMethodArgs args, Exception exception)
        {
            Console.WriteLine($"Failed method {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
            return exception;
        }
    }
}
