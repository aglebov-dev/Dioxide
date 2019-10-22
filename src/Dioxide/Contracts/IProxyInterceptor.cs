using System;

namespace Dioxide.Contracts
{
    public interface IProxyInterceptor
    {
        void Enter(string methodName, IMethodArgs args);
        void Exit(string methodName, IMethodArgs args, IMethodResult result);
        void Finaly(string methodName, IMethodArgs args, IMethodResult result);
        Exception Catch(string methodName, IMethodArgs args, Exception exception);
    }
}
