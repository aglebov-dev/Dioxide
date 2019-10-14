using System;
using System.Collections.Generic;

namespace Dioxide.Contracts
{
    public class VisitorsGroup
    {
        private readonly LinkedList<IVisitor> _visitors;
        public VisitorsGroup()
        {
            _visitors = new LinkedList<IVisitor>();
        }
        public void Add(IVisitor visitor)
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
        public void Finaly(string methodName, IMethodArgs args, IMethodResult result)
        {
            foreach (var visitor in _visitors)
            {
                visitor.Finaly(methodName, args, result);
            }
        }
    }
}
