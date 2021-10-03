using System.IO;
using System.Reflection;
using Dioxide.Compilation;
using Dioxide.Tools;
using Dioxide.Tools.SyntaxCreators;
using Microsoft.CodeAnalysis;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Dioxide.UnitTests
{
    public class GeneratorTests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
            var source = new[]
            {
                //EmbeddedResourceReader.ReadString<GeneratorTests>("InhType.cs"), // TODO now not work
                EmbeddedResourceReader.ReadString<GeneratorTests>("SimpleType.cs"),
                //EmbeddedResourceReader.ReadString<GeneratorTests>("TestTypeOfT.cs"), //TODO not auto
            };
            var t = typeof(IServiceCollection);
            var compilation = new CompilationBuilder().CreateDefaultCompilation();
            var path = typeof(IServiceCollection).Assembly.Location;
            var reference = MetadataReference.CreateFromFile(path);
            var trees = source.GetSyntaxTree();

            compilation = compilation
                .AddReferences(reference)
                .AddSyntaxTrees(trees);

            var gen1 = new ServiceCollectionRegistryGenerator();
            var unit = gen1.TestGenerate(compilation);
            var code = unit.NormalizeWhitespace(elasticTrivia: true).ToFullString();

            var assembly = new AssemblyBuilder().CraeteAssembly(compilation, unit);
        }
    }
}
