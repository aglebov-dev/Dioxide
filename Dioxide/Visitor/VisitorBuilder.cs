using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dioxide.Visitor.Parts;
using Dioxide.Tools.Extensions;

namespace Dioxide.Visitor
{
    public class VisitorBuilder
    {
        private readonly Compilation _compilation;
        private readonly CtorBuilder _ctorBuilder;
        private readonly MethodBuilder _methodsBuildere;

        public VisitorBuilder(Compilation compilation)
        {
            _compilation = compilation;
            _ctorBuilder = new CtorBuilder(compilation);
            _methodsBuildere = new MethodBuilder(compilation);
        }

        public ClassDeclarationSyntax Build(string typeName, Type origin, params Type[] ctorArguments)
        {
            var originType = origin.GetSymbols(_compilation);
            var arguments = ctorArguments.Select(x => x.GetSymbols(_compilation)).Where(x => x != null).ToArray();
            var typeBuilder = new TypeBuilder();

            var ctorResult = _ctorBuilder.Create(typeName, originType, arguments);
            var methodsresult = _methodsBuildere.CreateMethods(originType, ctorResult);

            var declaration = typeBuilder.CreateClassDeclaration
            (
                typeName: typeName,
                baseTypes: new[] { originType },
                ctor: ctorResult.Ctor,
                fields: ctorResult.Fields,
                methods: methodsresult.Methods
            );

            return declaration;
        }
    }
}
