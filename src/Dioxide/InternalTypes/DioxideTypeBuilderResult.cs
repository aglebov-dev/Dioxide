using System;
using Dioxide.Contracts;

namespace Dioxide.InternalTypes
{
    internal class DioxideTypeBuilderResult : IDioxideTypeBuilderResult
    {
        public GenerateType GenerateType { get; }
        public Type OriginType { get; }
        public Type GeneratedType { get; }

        public DioxideTypeBuilderResult(GenerateType generateType, Type originType, Type generatedType)
        {
            GenerateType = generateType;
            OriginType = originType;
            GeneratedType = generatedType;
        }
    }
}
