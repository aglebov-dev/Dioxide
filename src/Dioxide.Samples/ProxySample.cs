using Autofac;
using Dioxide.Autofac;
using Dioxide.Samples.Dependencies;
using Dioxide.Samples.Proxy;
using Dioxide.Samples.Proxy.Interseptors;
using System;

namespace Dioxide.Samples
{
    public class ProxySample: Contracts.IGeneratorDiagnostics
    {
        public Action<string> CompilationResult => Console.WriteLine;

        public static void Sample()
        {
            new DioxideTypeBuilder(default).Build();

            var result = new DioxideTypeBuilder(new ProxySample())
              .GenerateDecorator<IDoSomthing>(x =>
              {
                  x.With<LogInterceptor>();
                  x.With<MetricsInterceptor>();
                  x.With<ForgiveMistakesInterceptor>();
              })
              .Build();


            var builder = new ContainerBuilder();

            builder.RegistryDioxideTypes(result);
            builder.RegisterType<LogInterceptor>().InstancePerDependency();
            builder.RegisterType<MetricsInterceptor>().InstancePerDependency();
            builder.RegisterType<ForgiveMistakesInterceptor>().InstancePerDependency();
            builder.RegisterType<DoSomthing>().As<IDoSomthing>().InstancePerLifetimeScope();

            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            var type = builder.Build().Resolve<IDoSomthing>();

            try
            {
                var value = type.GetInteger(100, 200, "xyz");
                Console.WriteLine($"result is {value}");
            }
            catch
            {

            }
        }
    }
}
