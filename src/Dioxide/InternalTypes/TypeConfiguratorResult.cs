using System;
using Dioxide.Contracts;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dioxide.InternalTypes
{
    internal class TypeConfiguratorResult
    {
        public GenerateType GenerateType { get; }
        public string TypeName { get; }
        public Type OriginType { get; }
        public ClassDeclarationSyntax Syntax { get; }
        public TypeConfiguratorResult(GenerateType generateType, string typeName, Type originType, ClassDeclarationSyntax syntax)
        {
            GenerateType = generateType;
            TypeName = typeName;
            OriginType = originType;
            Syntax = syntax;
        }
    }
}
