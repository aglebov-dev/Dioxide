using System;
using Dioxide.Contracts;

namespace Dioxide.Samples.Proxy
{
    public class InformationVisitor : IProxyInterceptor
    {
        public void Enter(string methodName, IMethodArgs args)
        {
            if (methodName == nameof(IDoSomthing.GoTask))
            {
                var name = args.GetParamByName<string>("name");
                Console.WriteLine($"Welcome {name}");
            }
            else
            {
                Console.WriteLine($"Enter {methodName}");
            }
        }

        public void Exit(string methodName, IMethodArgs args, IMethodResult result)
        {
            if (methodName == nameof(IDoSomthing.GoTask))
            {
                var name = args.GetParamByName<string>("name");
                Console.WriteLine($"Goodbye {name}");
            }
            else
            {
                Console.WriteLine($"Exit {methodName}");
            }
        }

        public void Finaly(string methodName, IMethodArgs args, IMethodResult result)
        {
            if (methodName == nameof(IDoSomthing.GoTask))
            {
                var name = args.GetParamByName<string>("name");
                Console.WriteLine($"Do not see you again {name}");
            }
            else
            {
                Console.WriteLine($"Finaly {methodName}");
            }
        }

        public Exception Catch(string methodName, IMethodArgs args, Exception exception)
        {
            Console.WriteLine($"Method: {methodName}; Error:{exception.Message}");
            return exception;
        }
    }
}
