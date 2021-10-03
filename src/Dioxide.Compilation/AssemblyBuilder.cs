using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dioxide.Compilation
{
    public class AssemblyBuilder
    {
        public string AssemblyNamespace { get; }

        public AssemblyBuilder()
        {
            AssemblyNamespace = "Dioxide.Dynamic.X" + Guid.NewGuid().ToString("N");
        }

        public Assembly CraeteAssembly(Microsoft.CodeAnalysis.Compilation compilation, params SyntaxNode[] units)
        {
            var trees = units.Select(unit => unit.SyntaxTree).ToArray();
            var innerCompilation = compilation.AddSyntaxTrees(trees);

            var stream = new MemoryStream();
            var emitResult = innerCompilation.Emit(stream);
            var diagnostics = emitResult.Diagnostics.Select(diagnostic => diagnostic.ToString()).ToArray();
            var errors = string.Join("\n", diagnostics);
            if (!string.IsNullOrWhiteSpace(errors))
            {
                throw new Exception(errors);
            }

            if (emitResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(stream.ToArray());

                return assembly;
            }

            throw new Exception("Error");
        }
    }
}
