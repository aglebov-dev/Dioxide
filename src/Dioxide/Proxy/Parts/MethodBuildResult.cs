using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dioxide.Proxy.Parts
{
    internal class MethodBuildResult
    {
        public MethodDeclarationSyntax[] Methods { get; }

        public MethodBuildResult(MethodDeclarationSyntax[] methods)
        {
            Methods = methods;
        }
    }
}
