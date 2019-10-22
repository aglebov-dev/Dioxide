using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Reflection;
using Dioxide.Contracts;

namespace Dioxide.InternalBuilders
{
    internal class CompilationBuilder
    {
        private readonly Assembly[] _assemblies;
        private readonly IGeneratorDiagnostics _diagnostics;

        public CompilationBuilder(IGeneratorDiagnostics diagnostics)
        {
            var defaultAssemblies = new[]
            {
                Assembly.Load("System.Runtime"),
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(List<IProxyInterceptor>).Assembly,
                typeof(object).Assembly,
                typeof(object).GetTypeInfo().Assembly,
                typeof(CompilationBuilder).GetTypeInfo().Assembly
            };

            _assemblies = defaultAssemblies.ToArray();
            _diagnostics = diagnostics;
        }

        public CSharpCompilation Create()
        {
            var paths = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES").ToString().Split(';');

            var references = _assemblies
                .Select(x => x.Location)
                .Concat(paths)
                .Distinct()
                .Select(x => MetadataReference.CreateFromFile(x))
                .ToArray();

            var options = new CSharpCompilationOptions
            (
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                platform: Platform.AnyCpu,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false
            );

            options = options.WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);

            var compilation = CSharpCompilation.Create
            (
                assemblyName: Guid.NewGuid().ToString(),
                options: options,
                references: references,
                syntaxTrees: new SyntaxTree[] { }
            );

            return compilation;
        }
    }
}
