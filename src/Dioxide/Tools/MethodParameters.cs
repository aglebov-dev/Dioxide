using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dioxide.Tools.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Dioxide.Tools
{
    internal class Parameter
    {
        public ParameterSyntax Syntax { get; set; }
        public string OriginalName { get; set; }
        public string Name { get; set; }
    }

    internal class MethodParameters
    {
        private static HashSet<string> _hash = new HashSet<string>(Names.GetReservedFieldNames());

        public Parameter[] Parameters { get; }

        public MethodParameters(IMethodSymbol methodSymbol)
        {
            var query =
                from x in methodSymbol.Parameters
                let name = GetVariant(x.Name)
                let syntax = x.Type.ParameterExpression(name)
                select new Parameter { Name = name, OriginalName = x.Name, Syntax = syntax };

            Parameters = query.ToArray();
        }

        private string GetVariant(string value)
        {
            var index = 0;
            var current = value;
            while (_hash.Contains(current))
            {
                current = $"{value}{++index}";
            }

            return current;
        }
    }
}
