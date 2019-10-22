using System;
using Dioxide.Contracts;

namespace Dioxide.InternalTypes
{
    internal class TypeBuilderResult : ITypeBuilderResult
    {
        public GenerateType GenerateType { get; }
        public Type OriginType { get; }
        public Type GeneratedType { get; }

        public TypeBuilderResult(GenerateType generateType, Type originType, Type generatedType)
        {
            GenerateType = generateType;
            OriginType = originType;
            GeneratedType = generatedType;
        }
    }
}
