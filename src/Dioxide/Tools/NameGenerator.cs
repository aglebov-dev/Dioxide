using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Dioxide.Tools
{
    internal static class NameGenerator
    {
        public static string GetFieldName(string name) => $"_{name}";

        public static Dictionary<string, INamedTypeSymbol> GenerateNames(INamedTypeSymbol original, IEnumerable<INamedTypeSymbol> types)
        {
            types = types ?? Enumerable.Empty<INamedTypeSymbol>();
            types = new[] { original }.Concat(types);

            var names = new HashSet<string>();

            var result = new Dictionary<string, INamedTypeSymbol>();

            foreach (var type in types)
            {
                var firstChar = char.ToLower(type.Name[0]);

                var index = 0;
                var name = firstChar + type.Name.Substring(1);
                while (names.Contains(name))
                {
                    name = $"{name}{++index}";
                }

                names.Add(name);

                result[name] = type;
            }

            return result;
        }
    }
}
