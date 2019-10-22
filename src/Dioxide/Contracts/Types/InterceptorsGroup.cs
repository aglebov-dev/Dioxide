using System;
using System.Collections.Generic;
using System.Linq;

namespace Dioxide.Contracts
{
    public class InterceptorsGroup
    {
        private readonly LinkedList<IProxyInterceptor> _visitors;
        public InterceptorsGroup()
        {
            _visitors = new LinkedList<IProxyInterceptor>();
        }
        public void Add(IProxyInterceptor visitor)
        {
            if (visitor != null)
            {
                _visitors.AddLast(visitor);
            }
        }
        public void Enter(string methodName, IMethodArgs args)
        {
            foreach (var visitor in _visitors)
            {
                visitor.Enter(methodName, args);
            }
        }
        public void Exit(string methodName, IMethodArgs args, IMethodResult result)
        {
            foreach (var visitor in _visitors)
            {
                visitor.Exit(methodName, args, result);
            }
        }
        public Exception Catch(string methodName, IMethodArgs args, Exception exception)
        {
            foreach (var visitor in _visitors)
            {
                exception = visitor.Catch(methodName, args, exception);
            }

            return exception;
        }
        public IMethodResult CatchOverrideResult(string methodName, IMethodArgs args, Exception exception)
        {
            var lastResult = _visitors
                .OfType<ICatchResultsOverrider>()
                .Select(x => x.CatchOverrideResult(methodName, args, exception))
                .Where(x => x != null)
                .LastOrDefault();

            return lastResult;
        }
        public void Finaly(string methodName, IMethodArgs args, IMethodResult result)
        {
            foreach (var visitor in _visitors)
            {
                visitor.Finaly(methodName, args, result);
            }
        }
    }
}
