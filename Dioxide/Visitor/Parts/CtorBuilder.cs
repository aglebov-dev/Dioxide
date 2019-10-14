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

namespace Dioxide.Visitor.Parts
{
    internal class CtorBuilderResult
    {
        public string OriginFieldIdentifier { get; }
        public string VisitorFieldIdentifier { get; }
        public ConstructorDeclarationSyntax Ctor { get; }
        public FieldDeclarationSyntax[] Fields { get; }
        public CtorBuilderResult(string originFieldIdentifier, string visitorFieldIdentifier, ConstructorDeclarationSyntax ctor, FieldDeclarationSyntax[] fields)
        {
            OriginFieldIdentifier = originFieldIdentifier;
            VisitorFieldIdentifier = visitorFieldIdentifier;
            Ctor = ctor;
            Fields = fields;
        }
    }

    internal class CtorBuilder
    {
        private readonly string _visitorFieldIdentifier;
        private readonly string _visitorFieldType;
        private readonly INamedTypeSymbol _iVisitorSymbols;

        public CtorBuilder(Compilation compilation)
        {
            _iVisitorSymbols = compilation.GetSymbols<IVisitor>();
            _visitorFieldIdentifier = "_visitors";
            _visitorFieldType = compilation.GetSymbols<VisitorsGroup>().GlobalTypeName();
        }

        public CtorBuilderResult Create(string typeName, INamedTypeSymbol original, INamedTypeSymbol[] types)
        {
            types = types ?? Array.Empty<INamedTypeSymbol>();
            var ctorIdentifier = Identifier(typeName);
            var ctorModifier = Token(SyntaxKind.PublicKeyword);
            var ctorArgumentNames = NameGenerator.GenerateNames(original, types);
            var originalFieldName = NameGenerator.GetFieldName(ctorArgumentNames.First().Key);
            var ctorArguments = ctorArgumentNames.Select(x => x.Value.ParameterExpression(x.Key)).ToArray();
            var ctorBodyExpressions = CtorBody(ctorArgumentNames.Keys).ToArray();
            var fields = Fields(originalFieldName, original.GlobalTypeName()).ToArray();

            var ctorBody = Block().AddStatements(ctorBodyExpressions);
            var ctor = ConstructorDeclaration(ctorIdentifier)
                .AddModifiers(ctorModifier)
                .AddParameterListParameters(ctorArguments)
                .WithBody(ctorBody);

            return new CtorBuilderResult(originalFieldName, _visitorFieldIdentifier, ctor, fields);
        }

        private IEnumerable<StatementSyntax> CtorBody(IEnumerable<string> names)
        {
            var origin = names.First();

            yield return NameGenerator.GetFieldName(origin).SetStatement(origin);
            yield return _visitorFieldIdentifier.SetStatement(_visitorFieldType.NewExpression());

            var addToVisitorsList =
                from x in names.Skip(1)
                let asExpression = x.AsExpression(_iVisitorSymbols)
                select _visitorFieldIdentifier.InvokeSyncStatement("Add", asExpression);

            foreach (var item in addToVisitorsList)
            {
                yield return item;
            }
        }

        private IEnumerable<FieldDeclarationSyntax> Fields(string originalName, string originalType)
        {
            yield return _visitorFieldIdentifier.FieldExpression(_visitorFieldType);
            yield return originalName.FieldExpression(originalType);
        }
    }
}
