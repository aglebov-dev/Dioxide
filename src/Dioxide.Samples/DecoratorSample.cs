using Autofac;
using Dioxide.Autofac;
using Dioxide.Samples.Proxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dioxide.Samples
{
    public class DecoratorSample: Contracts.IGeneratorDiagnostics
    {
        public Action<string> CompilationResult => Console.WriteLine;

        public static void Sample()
        {
            new DioxideTypeBuilder(default).Build();

            var result = new DioxideTypeBuilder(new DecoratorSample())
              .GenerateDecorator<IDoSomthing>(x =>
              {
                  x.With<InformationVisitor>();
                  x.With<MetricsVisitor>();
              })
              .Build();


            var builder = new ContainerBuilder();

            builder.RegistryDioxideTypes(result);
            builder.RegisterType<InformationVisitor>();
            builder.RegisterType<MetricsVisitor>();
            builder.RegisterType<DoSomthing>()
                .As<IDoSomthing>();

            var type = builder.Build().Resolve<IDoSomthing>();

            //try
            //{
            //    type.GoTask("dioxide", 1500).GetAwaiter().GetResult();
            //    type.DoSomething();
            //}
            //catch
            //{
                
            //}
        }
    }
}
