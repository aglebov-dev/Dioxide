using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools.Extensions
{
    internal static class ExpressionExtensions
    {
        public static ExpressionSyntax NewExpression(this string type) //TODO args
        {
            var t = type.ToQualifiedTypeName();
            var emptyArguments = ArgumentList(new SeparatedSyntaxList<ArgumentSyntax>());
            return ObjectCreationExpression(t, emptyArguments, null);
        }

        public static ExpressionSyntax AsExpression(this string name, INamedTypeSymbol type)
        {
            var asType = type.ToQualifiedTypeName();
            return BinaryExpression(SyntaxKind.AsExpression, IdentifierName(name), asType);
        }

        public static ExpressionSyntax AsLiteral(this string literal)
        {
            var token = Literal(literal);
            return LiteralExpression(SyntaxKind.StringLiteralExpression, token);
        }

        public static ParameterSyntax ParameterExpression(this ITypeSymbol type, string name)
        {
            var identifier = Identifier(name);
            var typeName = type.ToQualifiedTypeName();

            return Parameter(identifier).WithType(typeName);
        }

        public static FieldDeclarationSyntax FieldExpression(this string name, string type)
        {
            var t = type.ToQualifiedTypeName();
            var variableDeclaration =
                VariableDeclaration(t).AddVariables(VariableDeclarator(name));

            var fieldDeclaration = FieldDeclaration(variableDeclaration)
                .AddModifiers(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword));

            return fieldDeclaration;
        }

        public static ExpressionSyntax InvokeSyncExpression(this string name, string method, params ExpressionSyntax[] arguments)
        {
            var expression = MemberAccessExpression
            (
                SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(name),
                Token(SyntaxKind.DotToken),
                IdentifierName(method)
            );

            var argsSyntax = SeparatedList(arguments.Select(Argument));
            var argsList = ArgumentList().WithArguments(argsSyntax);
            var exp = InvocationExpression
            (
                expression,
                argsList
            );

            return exp;
        }

        public static ExpressionSyntax CallMethodSyncExpression(this string method, params ExpressionSyntax[] arguments)
        {
            var argumentsSyntax = arguments.Select(Argument);

            return InvocationExpression(
                method.ToQualifiedTypeName(),
                ArgumentList(SeparatedList(argumentsSyntax))
            );
        }

        public static ExpressionSyntax InvokeAwaitExpression(this string name, string method, params ExpressionSyntax[] arguments)
        {
            var exp = name.InvokeSyncExpression(method, arguments);

            return AwaitExpression(exp);
        }
    }
}
