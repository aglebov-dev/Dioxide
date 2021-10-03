using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools.SyntaxCreators
{
    public static class CodeCreator
    {
        /// <summary>Make typeof(...)</summary>
        public static ExpressionSyntax TypeOf(ITypeSymbol namedType)
        {
            var interfaceName = namedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return TypeOfExpression(
                Token(SyntaxKind.TypeOfKeyword),
                Token(SyntaxKind.OpenParenToken),
                IdentifierName(interfaceName),
                Token(SyntaxKind.CloseParenToken)
            );
        }

        /// <summary>Make invoke member.Method()</summary>
        public static ExpressionSyntax Invoke(ExpressionSyntax member, IMethodSymbol method, IEnumerable<ExpressionSyntax> arguments)
        {
            var invoke = MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                member,
                Token(SyntaxKind.DotToken),
                IdentifierName(method.Name)
            );
            var args = arguments.Select(Argument).ToArray();
            var expression = InvocationExpression(invoke, ArgumentList(SeparatedList(args)));

            return expression;
        }

        /// <summary>Make invoke this.Method()</summary>
        public static ExpressionSyntax Invoke(IMethodSymbol method, IEnumerable<ExpressionSyntax> arguments)
        {
            var @this = IdentifierName(Token(SyntaxKind.ThisKeyword));
            return Invoke(@this, method, arguments);
        }

        /// <summary>Make method argument like this .(this argumentType argumentName)</summary>
        public static ParameterSyntax CreateMethodArgumentThis(string argumentName, ITypeSymbol argumentType)
        {
            var typeName = argumentType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var name = Identifier(argumentName);
            var type = IdentifierName(typeName);
            var modificator = Token(SyntaxKind.ThisKeyword);

            return Parameter(name)
               .WithModifiers(TokenList(modificator))
               .WithType(type);
        }

        public static TypeSyntax Void()
        {
            return PredefinedType(Token(SyntaxKind.VoidKeyword));
        }

        public static TypeSyntax Type(ITypeSymbol typeSymbol)
        {
            var name = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return IdentifierName(name);
        }

        public static SyntaxTokenList Modificators(params SyntaxKind[] kinds)
        {
            var tokens = kinds.Select(Token).ToArray();
            return TokenList(tokens);
        }
    }
}
