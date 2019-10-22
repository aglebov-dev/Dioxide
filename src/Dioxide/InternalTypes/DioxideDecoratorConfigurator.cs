using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Dioxide.Visitor;
using Dioxide.Contracts;
using Microsoft.CodeAnalysis;

namespace Dioxide.InternalTypes
{
    public class DioxideDecoratorConfigurator : IDioxideTypeConfigurator<IVisitor>
    {
        private readonly HashSet<Type> _ctorTypes;

        public string TypeName { get; }
        public Type OriginType { get; }

        public DioxideDecoratorConfigurator(Type originType)
        {
            TypeName = $"{originType.Name}_X{Guid.NewGuid().ToString("N")}";
            OriginType = originType;

            _ctorTypes = new HashSet<Type>();
        }

        IDioxideTypeConfigurator<IVisitor> IDioxideTypeConfigurator<IVisitor>.With<V>()
        {
            var type = typeof(V);
            _ctorTypes.Add(type);

            return this;
        }

        internal ClassDeclarationSyntax BuildType(Compilation compilation)
        {
            var visitorBuilder = new VisitorBuilder(compilation);
            return visitorBuilder.Build(TypeName, OriginType, _ctorTypes.ToArray());
        }
    }
}
