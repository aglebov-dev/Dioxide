using System;
using System.Linq;
using System.Collections.Generic;
using Dioxide.Contracts;
using Dioxide.InternalBuilders;
using Dioxide.InternalTypes;

namespace Dioxide
{
    public class DioxideTypeBuilder
    {
        private readonly IGeneratorDiagnostics _diagnostics;

        private readonly Dictionary<Type, TypeConfigurator> _hash;

        public DioxideTypeBuilder(IGeneratorDiagnostics diagnostics)
        {
            _hash = new Dictionary<Type, TypeConfigurator>();
            _diagnostics = diagnostics;
        }

        public DioxideTypeBuilder GenerateDecorator<TInterface>(Action<ITypeConfigurator<IProxyInterceptor>> configurationAction)
        {
            var configurator = GetOrCreateTypeConfigurator<TInterface>();
            configurator.AppendGenerateType(GenerateType.Proxy);
            configurationAction?.Invoke(configurator.VisitorConfigurator);

            return this;
        }

        public DioxideTypeBuilder GenerateDecorator<TInterface>()
        {
            var configurator = GetOrCreateTypeConfigurator<TInterface>();
            configurator.AppendGenerateType(GenerateType.Proxy);

            return this;
        }

        private TypeConfigurator GetOrCreateTypeConfigurator<TInterface>()
        {
            var key = typeof(TInterface);
            if (!_hash.TryGetValue(key, out var configurator))
            {
                _hash[key] = configurator = new TypeConfigurator(key);
            }

            return configurator;
        }

        public IGenerateResult Build()
        {
            var compileBuilder = new CompilationBuilder(_diagnostics);
            var assemblyBuilder = new AssemblyBuilder(_diagnostics);
            var compile = compileBuilder.Create();
            var results = _hash.SelectMany(x => x.Value.GenerateTypes(compile));
            var syntaxes = results.Select(x => x.Syntax).ToArray();
            var assembly = assemblyBuilder.CraeteAssembly(compile, syntaxes);

            if (assembly != null)
            {
                var generateResults =
                    from result in results
                    let type = assembly.GetType($"{assemblyBuilder.AssemblyNamespace}.{result.TypeName}")
                    select new TypeBuilderResult(result.GenerateType, result.OriginType, type);

                return new GenerateResult(true, generateResults.ToArray());
            }

            return new GenerateResult(false, default);
        }
    }
}
