using System.Collections.Generic;
using System.Linq;
using Dioxide.Tools.SyntaxCreators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Tools
{
    public class ServiceCollectionRegistryGenerator
    {
        private const string ServiceCollectionExtensinsName = "Microsoft.Extensions.DependencyInjection.ServiceCollectionServiceExtensions";
        private const string IServiceCollectionName = "Microsoft.Extensions.DependencyInjection.IServiceCollection";

        public SyntaxNode TestGenerate(Compilation compilation)
        {
            var types = compilation.GetTypesFromSyntaxTrees(filter: symbols => true).ToArray();
            var addScopeMethod = compilation
                .GetTypeByMetadataName(ServiceCollectionExtensinsName)
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(x => x.Name.Equals(nameof(ServiceCollectionServiceExtensions.AddScoped)))
                .FirstOrDefault();

            var serviceCollectionSymbols = compilation
                .GetTypeByMetadataName(IServiceCollectionName);
           
            var argument = CodeCreator.CreateMethodArgumentThis("services", serviceCollectionSymbols);
            var invokes = new List<ExpressionSyntax>(types.Length);
            foreach (var item in types)
            {
                foreach (var abstractName in item.Interfaces)
                {
                    var arguments = new[]
                    {
                        CodeCreator.TypeOf(abstractName),
                        CodeCreator.TypeOf(item)
                    };
                    var invoke = CodeCreator.Invoke(IdentifierName(argument.Identifier), addScopeMethod, arguments);
                    invokes.Add(invoke);
                }
            }

            var myMethod = TypeCreator.CreateMethod
            (
                methodName: "Registy",
                modificators: CodeCreator.Modificators(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                returnType: CodeCreator.Void(),
                arguments: new[] { argument },
                body: invokes.ToArray()
            );

            var myClass = TypeCreator.CreateClass(
                name: "Registor",
                modificators: CodeCreator.Modificators(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                members: new[] { myMethod }
            );

            var myUnit = TypeCreator.CreateUnit(
               usings: new[] { compilation.GetNameSpace(addScopeMethod) },
               members: new[] { myClass }
            );

            return myUnit;


            //var u1 = UsingStatement(Token( SyntaxKind.UsingKeyword));
            //var xxx = dymanicType.NormalizeWhitespace(elasticTrivia: true).ToFullString();
            //Microsoft.Extensions.DependencyInjection
            //context.AddSource("x1", SourceText.From(xxx, encoding: System.Text.Encoding.UTF8));
        }
    }
}
