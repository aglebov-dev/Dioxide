using Dioxide.Contracts;
using System;
using System.Threading.Tasks;

namespace Dioxide.Samples.Proxy.Interseptors
{
    public class ForgiveMistakesInterceptor : IProxyInterceptor, ICatchResultsOverrider
    {
        public Exception Catch(string methodName, IMethodArgs args, Exception exception) => exception;

        public void Enter(string methodName, IMethodArgs args) { }

        public void Exit(string methodName, IMethodArgs args, IMethodResult result) { }

        public void Finaly(string methodName, IMethodArgs args, IMethodResult result) { }

        public IMethodResult CatchOverrideResult(string methodName, IMethodArgs args, Exception exception)
        {
            var result = new MethodResult();
            if (methodName.Equals(nameof(IDoSomthing.GoTask)))
            {
                result.Set(Task.FromResult(777));
            }
            else if (methodName.Equals(nameof(IDoSomthing.GetInteger)))
            {
                result.Set(777);
            }

            return result;
        }
    }
}
