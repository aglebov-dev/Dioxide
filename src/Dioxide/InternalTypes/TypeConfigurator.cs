using Dioxide.Contracts;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Dioxide.InternalTypes
{
    internal class TypeConfigurator
    {
        private readonly HashSet<GenerateType> _types;
        public DecoratorConfigurator VisitorConfigurator { get; }

        public TypeConfigurator(Type originType)
        {
            _types = new HashSet<GenerateType>();
            VisitorConfigurator = new DecoratorConfigurator(originType);
        }

        internal TypeConfigurator AppendGenerateType(GenerateType type)
        {
            _types.Add(type);
            return this;
        }

        internal IEnumerable<TypeConfiguratorResult> GenerateTypes(Compilation compilation)
        {
            if (_types.Contains(GenerateType.Proxy))
            {
                yield return new TypeConfiguratorResult
                (
                   generateType: GenerateType.Proxy,
                   syntax: VisitorConfigurator.BuildType(compilation),
                   typeName: VisitorConfigurator.TypeName,
                   originType: VisitorConfigurator.OriginType
                );
            }
        }
    }
}
