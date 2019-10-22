using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dioxide.Tools.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Visitor.Parts
{
    internal class TypeBuilder
    {
        public ClassDeclarationSyntax CreateClassDeclaration(
            string typeName,
            INamedTypeSymbol[] baseTypes,
            ConstructorDeclarationSyntax ctor,
            FieldDeclarationSyntax[] fields,
            MethodDeclarationSyntax[] methods)
        {
            baseTypes = baseTypes ?? Array.Empty<INamedTypeSymbol>();
            ctor = ctor ?? ConstructorDeclaration(typeName).WithBody(Block());
            fields = fields ?? Array.Empty<FieldDeclarationSyntax>();
            methods = methods ?? Array.Empty<MethodDeclarationSyntax>();

            var members = new SyntaxList<MemberDeclarationSyntax>()
               .AddRange(fields)
               .Add(ctor)
               .AddRange(methods)
               ;

            var modifiers = GetModifiers();
            var simpleBaseTypes = baseTypes.Select(CreateBaseTypeSyntax).ToArray();
            var dymanicType = ClassDeclaration(typeName)
               .AddModifiers(modifiers)
               .WithMembers(members);

            if (simpleBaseTypes.Length > 0)
            {
                dymanicType = dymanicType.AddBaseListTypes(simpleBaseTypes);
            }

            return dymanicType;
        }

        private SyntaxToken[] GetModifiers()
        {
            return new[] {
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.SealedKeyword)
            };
        }

        private SimpleBaseTypeSyntax CreateBaseTypeSyntax(INamedTypeSymbol symbols)
        {
            var identifier = symbols.ToQualifiedTypeName();
            return SimpleBaseType(identifier);
        }
    }
}
