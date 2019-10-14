//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Dioxide.Contracts;
//using Dioxide.Visitor;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;

//namespace Dioxide
//{
//    public class DioxideTypeBuilder
//    {
//        private readonly DioxideBuilder _parent;
//        private readonly Type _originType;
//        private readonly IGeneratorDiagnostics _diagnostics;
//        private readonly HashSet<Type> _ctorTypes;
//        private readonly string _typeName;

//        public IEnumerable<Type> Types => _ctorTypes.Append(_originType);
//        public string TypeName => _typeName;

//        public DioxideTypeBuilder(DioxideBuilder parent, Type type, IGeneratorDiagnostics diagnostics)
//        {
//            _parent = parent;
//            _originType = type;
//            _diagnostics = diagnostics;
//            _ctorTypes = new HashSet<Type>();
//            _typeName = $"{type.Name}_{Guid.NewGuid().ToString("N")}";
//        }

//        public DioxideTypeBuilder With<T>()
//        {
//            _ctorTypes.Add(typeof(T));
//            return this;
//        }

//        public DioxideTypeBuilder CreateType<T>()
//            where T : class
//        {
//            return _parent.CreateType<T>();
//        }

//        public BuildResult Build()
//        {
//            return _parent.Build();
//        }

//        internal ClassDeclarationSyntax BuildType(CSharpCompilation compilation)
//        {
//            var visitorBuilder = new VisitorBuilder(compilation);
//            return visitorBuilder.Build(_typeName, _originType, _ctorTypes.ToArray());
//        }

//        internal BuildResultType ExtractType(Assembly assembly)
//        {
//            var type = assembly.GetType(_typeName, true);
//            return new BuildResultType(type, _originType);
//        }
//    }
//}
