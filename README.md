# Dioxide

## Getting Started

###Decorator

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
