using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Dioxide.Compilation
{
    public class CompilationBuilder
    {
        public Microsoft.CodeAnalysis.Compilation CreateDefaultCompilation(string assemblyName = default)
        {
            PortableExecutableReference[] references = GetDefaultReferences();
            CSharpCompilationOptions options = CreateDefaultCompilationOptions();
            
            return CSharpCompilation.Create
            (
                assemblyName: assemblyName ?? Guid.NewGuid().ToString(),
                options: options,
                references: references,
                syntaxTrees: new SyntaxTree[] { }
            );
        }

        private PortableExecutableReference[] GetDefaultReferences()
        {
            Assembly[] defaultAssemblies = GetDefaultAssemlies();
            string[] paths = GetTrustedAssembyPaths();

            return defaultAssemblies
                .Select(x => x.Location)
                .Concat(paths)
                .Distinct()
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToArray();
        }

        private string[] GetTrustedAssembyPaths()
        {
            return AppContext
                .GetData("TRUSTED_PLATFORM_ASSEMBLIES")
                .ToString()
                .Split(';', StringSplitOptions.RemoveEmptyEntries);
        }

        private Assembly[] GetDefaultAssemlies()
        {
            return new[]
            {
                Assembly.Load("System.Runtime"),
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(object).Assembly,
                typeof(object).GetTypeInfo().Assembly,
            };
        }

        private CSharpCompilationOptions CreateDefaultCompilationOptions()
        {
            var options = new CSharpCompilationOptions
            (
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                platform: Platform.AnyCpu,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false
            );

            options = options.WithAssemblyIdentityComparer(DesktopAssemblyIdentityComparer.Default);

            return options;
        }
    }
}
