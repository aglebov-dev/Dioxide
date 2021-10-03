using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools.SyntaxCreators
{
    public static class SymbolsExtensions
    {
        public static SyntaxTree[] GetSyntaxTree(this IEnumerable<string> sources)
        {
            return sources
                .Select(source => ParseSyntaxTree(text: source))
                .ToArray();
        }

        public static INamespaceSymbol GetNameSpace(this Compilation compilation, ISymbol symbol)
        {
            return compilation.GetCompilationNamespace(symbol.ContainingNamespace);
        }

        public static ITypeSymbol GetSymbols<Type>(this Compilation compilation)
        {
            var type = typeof(Type);
            var symbols = compilation.GetTypeByMetadataName(type.FullName);

            if (symbols is null)
            {
                throw new Exception();
            }

            return symbols;
        }

        public static IEnumerable<ITypeSymbol> GetTypesFromSyntaxTrees(this Compilation compilation, Func<ITypeSymbol, bool> filter = null)
        {
            return
                from tree in compilation.SyntaxTrees
                let semantic = compilation.GetSemanticModel(tree)
                from node in tree.GetRoot().DescendantNodesAndSelf()
                where node is ClassDeclarationSyntax
                let symbols = semantic.GetDeclaredSymbol(node) as ITypeSymbol
                where symbols != null
                where filter?.Invoke(symbols) ?? true
                select symbols;
        }
    }
}
