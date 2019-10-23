using System;
using System.Linq;
using System.Collections.Generic;
using Dioxide.Tools;
using Dioxide.Contracts;
using Dioxide.Tools.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Proxy.Parts
{
    internal class CtorBuilder
    {
        private readonly string _visitorFieldType;
        private readonly string _interceptions;
        private readonly INamedTypeSymbol _iVisitorSymbols;

        public CtorBuilder(Compilation compilation)
        {
            _interceptions = Names.GetFieldName(Names.INTERCEPTORS_GROUP_FIELD);
            _iVisitorSymbols = compilation.GetSymbols<IProxyInterceptor>();
            _visitorFieldType = compilation.GetSymbols<InterceptorsGroup>().GlobalTypeName();
        }

        public CtorBuilderResult Create(string typeName, INamedTypeSymbol original, INamedTypeSymbol[] types)
        {
            types = types ?? Array.Empty<INamedTypeSymbol>();
            var ctorIdentifier = Identifier(typeName);
            var ctorModifier = Token(SyntaxKind.PublicKeyword);
            var ctorArgumentNames = Names.GenerateCtorParamNames(original, types);
            var originalFieldName = Names.GetFieldName(ctorArgumentNames.First().Key);
            var ctorArguments = ctorArgumentNames.Select(x => x.Value.ParameterExpression(x.Key)).ToArray();
            var ctorBodyExpressions = CtorBody(ctorArgumentNames.Keys).ToArray();
            var fields = Fields(originalFieldName, original.GlobalTypeName()).ToArray();

            var ctorBody = Block().AddStatements(ctorBodyExpressions);
            var ctor = ConstructorDeclaration(ctorIdentifier)
                .AddModifiers(ctorModifier)
                .AddParameterListParameters(ctorArguments)
                .WithBody(ctorBody);

            return new CtorBuilderResult(originalFieldName, ctor, fields);
        }

        private IEnumerable<StatementSyntax> CtorBody(IEnumerable<string> names)
        {
            var origin = names.First();
            
            yield return Names.GetFieldName(origin).SetStatement(origin);
            yield return _interceptions.SetValueStatement(_visitorFieldType.NewExpression());

            var addToVisitorsList =
                from x in names.Skip(1)
                let asExpression = x.AsExpression(_iVisitorSymbols)
                select _interceptions.InvokeSyncStatement(Names.INTERCEPTORS_GROUP_ADD_METHOD, asExpression);

            foreach (var item in addToVisitorsList)
            {
                yield return item;
            }
        }

        private IEnumerable<FieldDeclarationSyntax> Fields(string originalName, string originalType)
        {
            yield return _interceptions.FieldExpression(_visitorFieldType);
            yield return originalName.FieldExpression(originalType);
        }
    }
}
