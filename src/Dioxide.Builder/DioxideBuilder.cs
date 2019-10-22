using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Dioxide.Contracts;

namespace Dioxide.Builder
{
    public class DioxideBuilder
    {
        private readonly Dictionary<Type, DioxideTypeBuilder> _typeBuilders;
        private readonly IGeneratorDiagnostics _generatorDiagnostics;

        public DioxideBuilder(IGeneratorDiagnostics generatorDiagnostics = null)
        {
            _typeBuilders = new Dictionary<Type, DioxideTypeBuilder>();
            _generatorDiagnostics = generatorDiagnostics;
        }

        public DioxideTypeBuilder CreateType<T>()
            where T: class
        {
            var type = typeof(T);
            if (_typeBuilders.TryGetValue(type, out var builder) == false)
            {
                _typeBuilders[type] = builder = new DioxideTypeBuilder(this, type, _generatorDiagnostics);
            }

            return builder;
        }

        public BuildResult Build()
        {
            var assemblyBuilder = new AssemblyBuilder(_generatorDiagnostics);
            var compilation = new CompilationBuilder(_generatorDiagnostics, Array.Empty<Assembly>()).Create();

            var assemblyMembers = _typeBuilders.Values.Select(builder => builder.BuildType(compilation)).ToArray();
            var assembly = assemblyBuilder.CraeteAssembly(compilation, assemblyMembers);

            if (assembly != null)
            {
                var types =
                    from x in _typeBuilders
                    let fullName = $"{assemblyBuilder.AssemblyNamespace}.{x.Value.TypeName}"
                    let type = assembly.GetType(fullName)
                    select new BuildResultType(type, x.Key);

                return new BuildResult(true, string.Empty, types.ToArray());
            }

            return new BuildResult(false, "Assembly can't create", Array.Empty<BuildResultType>());
        }
    }
}
