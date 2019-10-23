using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Dioxide.Tools
{
    internal class MethodVariables
    {
        private readonly HashSet<string> _hash;

        public string EXCEPTION_VARIABLE;
        public string METHOD_NAME_VARIABLE;
        public string ARGS_VARIABLE;
        public string RESULT_VARIABLE;
        public string METHOD_RESULT_VARIABLE;
        public string METHOD_CATCH_RESULT_VARIABLE;
        public string CATCH_BLOCK_EXCEPTION_VARIABLE;

        public MethodVariables(IMethodSymbol methodSymbol)
        {
            _hash = new HashSet<string>(methodSymbol.Parameters.Select(x => x.Name));

            EXCEPTION_VARIABLE             = GetVariant("exception");
            METHOD_NAME_VARIABLE           = GetVariant("method");
            ARGS_VARIABLE                  = GetVariant("args");
            RESULT_VARIABLE                = GetVariant("result");
            METHOD_RESULT_VARIABLE         = GetVariant("methodResult");
            METHOD_CATCH_RESULT_VARIABLE   = GetVariant("cathMethodResult");
            CATCH_BLOCK_EXCEPTION_VARIABLE = GetVariant("ex");
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
