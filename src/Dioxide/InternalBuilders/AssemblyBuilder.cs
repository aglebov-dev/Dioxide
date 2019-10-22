using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Dioxide.Contracts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.InternalBuilders
{
    internal class AssemblyBuilder
    {
        private readonly IGeneratorDiagnostics _diagnostics;
        public string AssemblyNamespace { get; }
        public AssemblyBuilder(IGeneratorDiagnostics diagnostics)
        {
            AssemblyNamespace = "Dioxide.Dynamic.X" + Guid.NewGuid().ToString("N");
            _diagnostics = diagnostics;
        }

        public Assembly CraeteAssembly(Compilation compilation, params MemberDeclarationSyntax[] assemblyMembers)
        {
            assemblyMembers = assemblyMembers ?? Array.Empty<MemberDeclarationSyntax>();

            var @namespace = NamespaceDeclaration(IdentifierName(AssemblyNamespace)).AddMembers(assemblyMembers);
            var @compilationUnit = CompilationUnit().AddMembers(@namespace);
            var inner_compilation = compilation.AddSyntaxTrees(@compilationUnit.SyntaxTree);

            var stream = new MemoryStream();
            var emitResult = inner_compilation.Emit(stream);

            var errors = string.Join("\n", emitResult.Diagnostics.Select(x => x.ToString()).ToArray());
            _diagnostics?.CompilationResult(@compilationUnit.NormalizeWhitespace(elasticTrivia: true).ToFullString());
            _diagnostics?.CompilationResult(errors);

            if (emitResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(stream.ToArray());

                return assembly;
            }

            return default;
        }
    }
}
