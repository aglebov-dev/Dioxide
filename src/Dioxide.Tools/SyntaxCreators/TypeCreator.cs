using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools.SyntaxCreators
{
    public static class TypeCreator
    {
        public static MethodDeclarationSyntax CreateMethod(
            string methodName,
            SyntaxTokenList modificators,
            TypeSyntax returnType,
            ParameterSyntax[] arguments,
            ExpressionSyntax[] body = null
        )
        {
            var method = MethodDeclaration(returnType, methodName)
               .WithModifiers(modificators)
               .WithParameterList(ParameterList().AddParameters(arguments));

            if (body != null)
            {
                var statements = body.Select(ExpressionStatement).ToArray();
                var block = Block(statements);
                method = method.WithBody(block);
            }

            return method;
        }

        public static ClassDeclarationSyntax CreateClass(
            string name,
            SyntaxTokenList modificators,
            MemberDeclarationSyntax[] members
        )
        {
            var classMembers = new SyntaxList<MemberDeclarationSyntax>(members);

            return ClassDeclaration(name)
                .WithModifiers(modificators)
                .WithMembers(classMembers);
        }


        public static CompilationUnitSyntax CreateUnit(
            INamespaceSymbol[] usings,
            MemberDeclarationSyntax[] members
        )
        {
            var unitUsings = usings.Select(CreateUsing).ToArray();

            return CompilationUnit()
               .AddUsings(unitUsings)
               .AddMembers(members);
        }

        private static UsingDirectiveSyntax CreateUsing(INamespaceSymbol symbol)
        {
            var name = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return UsingDirective(IdentifierName(name));
        }
    }
}
