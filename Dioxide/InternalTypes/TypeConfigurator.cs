using Dioxide.Contracts;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Dioxide.InternalTypes
{
    internal class TypeConfigurator
    {
        private readonly HashSet<GenerateType> _types;
        public DioxideDecoratorConfigurator VisitorConfigurator { get; }

        public TypeConfigurator(Type originType)
        {
            _types = new HashSet<GenerateType>();
            VisitorConfigurator = new DioxideDecoratorConfigurator(originType);
        }

        internal TypeConfigurator AppendGenerateType(GenerateType type)
        {
            _types.Add(type);
            return this;
        }

        internal IEnumerable<TypeConfiguratorResult> GenerateTypes(Compilation compilation)
        {
            if (_types.Contains(GenerateType.Decorator))
            {
                yield return new TypeConfiguratorResult
                (
                   generateType: GenerateType.Decorator,
                   syntax: VisitorConfigurator.BuildType(compilation),
                   typeName: VisitorConfigurator.TypeName,
                   originType: VisitorConfigurator.OriginType
                );
            }
        }
    }
}
