using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;

namespace Dioxide.Tools.Extensions
{
    internal static class CompilationExtensions
    {
        public static INamedTypeSymbol GetSymbols<T>(this Compilation compilation)
        {
            return typeof(T).GetSymbols(compilation);
        }

        public static INamedTypeSymbol GetSymbols(this Type type, Compilation compilation)
        {
            return compilation?.GetTypeByMetadataName(type.FullName);
        }

        public static IMethodSymbol GetMethod(this INamedTypeSymbol symbol, string method)
        {
            return symbol?.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(x => x.Name.Equals(method));
        }


        public static string GlobalTypeName(this ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        public static TypeSyntax ToQualifiedTypeName(this ITypeSymbol symbol)
        {
            if (symbol.SpecialType == SpecialType.System_Void)
            {
                return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
            }

            return SyntaxFactory.ParseTypeName(symbol.GlobalTypeName());
        }
        public static TypeSyntax ToQualifiedTypeName(this string value)
        {
            if ("void".Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword));
            }

            return SyntaxFactory.ParseTypeName(value);
        }
    }
}
