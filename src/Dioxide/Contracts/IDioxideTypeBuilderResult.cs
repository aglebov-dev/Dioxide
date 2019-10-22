using System;

namespace Dioxide.Contracts
{
    public interface IDioxideTypeBuilderResult
    {
        GenerateType GenerateType { get; }
        Type OriginType { get; }
        Type GeneratedType { get; }
    }
}
