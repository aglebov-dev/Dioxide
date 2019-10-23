using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace Dioxide.Proxy.Parts
{
    internal class CtorBuilderResult
    {
        public string OriginFieldIdentifier { get; }
        public ConstructorDeclarationSyntax Ctor { get; }
        public FieldDeclarationSyntax[] Fields { get; }
        public CtorBuilderResult(string originFieldIdentifier, ConstructorDeclarationSyntax ctor, FieldDeclarationSyntax[] fields)
        {
            OriginFieldIdentifier = originFieldIdentifier;
            Ctor = ctor;
            Fields = fields;
        }
    }
}
