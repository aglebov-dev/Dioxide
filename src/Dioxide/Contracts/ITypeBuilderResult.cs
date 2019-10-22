using System;

namespace Dioxide.Contracts
{
    public interface ITypeBuilderResult
    {
        GenerateType GenerateType { get; }
        Type OriginType { get; }
        Type GeneratedType { get; }
    }
}
