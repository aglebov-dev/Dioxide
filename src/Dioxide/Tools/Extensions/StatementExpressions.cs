using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools.Extensions
{
    internal static class StatementExpressions
    {
        public static StatementSyntax SetStatement(this string identifier, string value)
        {
            var rigth = IdentifierName(value);
            return identifier.SetValueStatement(rigth);
        }

        public static StatementSyntax InvokeSyncStatement(this string name, string method, params ExpressionSyntax[] arguments)
        {
            var exp = name.InvokeSyncExpression(method, arguments);
            return ExpressionStatement(exp);
        }

        public static StatementSyntax InvokeAwaitStatement(this string name, string method, params ExpressionSyntax[] arguments)
        {

            var exp = name.InvokeAwaitExpression(method, arguments);
            return ExpressionStatement(exp);
        }

        public static StatementSyntax CallMethodSyncStatement(this string method, params ExpressionSyntax[] arguments)
        {
            var exp = method.CallMethodSyncExpression(arguments);
            return ExpressionStatement(exp);
        }

        public static StatementSyntax VarEqualsStatement(this string name, ExpressionSyntax expr)
        {
            var equals = EqualsValueClause(expr);
            var variable = VariableDeclarator(Identifier(name)).WithInitializer(equals);
            var list = SingletonSeparatedList(variable);
            var declaration = VariableDeclaration(IdentifierName("var")).WithVariables(list);

            return LocalDeclarationStatement(declaration);
        }

        public static StatementSyntax VarEqualsStringStatement(this string name, string literal)
        {
            var literalExpression = literal.AsLiteral();
            return name.VarEqualsStatement(literalExpression);
        }

        public static StatementSyntax SetValueStatement(this string identifier, ExpressionSyntax expr)
        {
            var letf = IdentifierName(identifier);
            var memberAccessExpression = AssignmentExpression(
               SyntaxKind.SimpleAssignmentExpression,
               letf,
               Token(SyntaxKind.EqualsToken),
               expr
            );

            return ExpressionStatement(memberAccessExpression);
        }
    }
}
