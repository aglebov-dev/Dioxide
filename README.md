# Dioxide

## Getting Started

### Decorator

The decorator uses the IVisitor interface. It is necessary to create an interface implementation that will be called inside the generated type
For sample:
```
using System;
using System.Diagnostics;
using Dioxide.Contracts;
public class MetricsVisitor : IVisitor
{
    private Stopwatch stopwatch;

    public void Enter(string methodName, IMethodArgs args)
    {
        stopwatch = Stopwatch.StartNew();
    }

    public void Exit(string methodName, IMethodArgs args, IMethodResult result)
    {
        Console.WriteLine($"Successful {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
    }

    public void Finaly(string methodName, IMethodArgs args, IMethodResult result) { }

    public Exception Catch(string methodName, IMethodArgs args, Exception exception)
    {
        Console.WriteLine($"Failed {methodName}. Total time: {stopwatch.ElapsedMilliseconds} ms");
        return exception;
    }
}
```
Create the necessary types using the builder
```
public interface IDoSomthing
{
    void DoSomething();
    Task<int> GoTask(string name, int deley);
}
```

```

var buildResult = new DioxideTypeBuilder(default)
    .GenerateDecorator<IDoSomthing>(x =>
    {
        x.With<InformationVisitor>();
        x.With<MetricsVisitor>();
    })
    .Build();
```

Register types in container
```
if (buildResult.IsSuccess)
{
    foreach (var item in buildResult.Types)
    {
        builder.RegisterDecorator(item.GeneratedType, item.OriginType);
    }
}
```

What will be generated (VisitorsGroup - implements the IVisitor interface and combines within itself the calls of the IVisitor implementations)
```
public sealed class DoSomthing_X37aecff11fa7496eb1063f8a5479a955 : IDoSomthing
    {
        private readonly VisitorsGroup _visitors;
        private readonly IDoSomthing _iDoSomthing;

        public DoSomthing_X37aecff11fa7496eb1063f8a5479a955(IDoSomthing iDoSomthing, MetricsVisitor metricsVisitor, InformationVisitor informationVisitor)
        {
            _iDoSomthing = iDoSomthing;
            _visitors = new VisitorsGroup();
            _visitors.Add(metricsVisitor as IVisitor);
            _visitors.Add(informationVisitor as IVisitor);
        }

        public void DoSomething()
        {
            var method = "DoSomething";
            var args = new MethodArgs();
            var result = new MethodResult();
            try
            {
                _visitors.Enter(method, args);
                _iDoSomthing.DoSomething();
                _visitors.Exit(method, args, result);
            }
            catch (System.Exception exception)
            {
                var ex = _visitors.Catch(method, args, exception);
                if (ex == null)
                    throw;
                else
                    throw ex;
            }
            finally
            {
                _visitors.Finaly(method, args, result);
            }
        }

        public async Task<int> GoTask(string name, int delay)
        {
            var method = "GoTask";
            var args = new MethodArgs();
            var result = new MethodResult();
            args.Set("name", name);
            args.Set("delay", delay);
            try
            {
                _visitors.Enter(method, args);
                var methodResult = await _iDoSomthing.GoTask(name, delay);
                result.Set(methodResult);
                _visitors.Exit(method, args, result);
                return methodResult;
            }
            catch (System.Exception exception)
            {
                var ex = _visitors.Catch(method, args, exception);
                if (ex == null)
                    throw;
                else
                    throw ex;
            }
            finally
            {
                _visitors.Finaly(method, args, result);
            }
        }
    }
```
