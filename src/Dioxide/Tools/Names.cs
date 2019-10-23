using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Dioxide.Contracts;
using System;

namespace Dioxide.Tools
{
    internal static class Names
    {
        private const string ORIGIN_GROUP_VARIABLE           = "client";
        
        public const string INTERCEPTORS_GROUP_FIELD        = "interseptors";
        public const string METHOD_RESULT_SET_METHOD        = nameof(Contracts.MethodResult.Set);
        public const string METHOD_ARGS_SET_METHOD          = nameof(Contracts.MethodArgs.Set);
        public const string METHOD_RESULT_HAS_RESULT_METHOD = nameof(Contracts.MethodResult.HasResult);
        public const string GET_RESULT_OR_DEFAULT           = nameof(Contracts.MethodResult.GetResultOrDefault);
        public const string INTERCEPTORS_GROUP_ADD_METHOD   = nameof(Contracts.InterceptorsGroup.Add);
        public const string ENTER_METHOD                    = "_" + INTERCEPTORS_GROUP_FIELD + "." + nameof(InterceptorsGroup.Enter);
        public const string EXIT_METHOD                     = "_" + INTERCEPTORS_GROUP_FIELD + "." + nameof(InterceptorsGroup.Exit);
        public const string CATCH_METHOD                    = "_" + INTERCEPTORS_GROUP_FIELD + "." + nameof(InterceptorsGroup.Catch);
        public const string FINALY_METHOD                   = "_" + INTERCEPTORS_GROUP_FIELD + "." + nameof(InterceptorsGroup.Finaly);
        public const string CATCH_OVERRIDE_RESULT_METHOD    = "_" + INTERCEPTORS_GROUP_FIELD + "." + nameof(InterceptorsGroup.CatchOverrideResult);

        public static IEnumerable<string> GetReservedFieldNames()
        {
            yield return GetFieldName(ORIGIN_GROUP_VARIABLE);
            yield return GetFieldName(INTERCEPTORS_GROUP_FIELD);
        }

        public static string GetFieldName(string name) => $"_{name}";

        public static Dictionary<string, INamedTypeSymbol> GenerateCtorParamNames(INamedTypeSymbol original, IEnumerable<INamedTypeSymbol> types)
        {
            types = types ?? Enumerable.Empty<INamedTypeSymbol>();

            var names = new HashSet<string>(new[] { ORIGIN_GROUP_VARIABLE });
            var result = new Dictionary<string, INamedTypeSymbol>() { [ORIGIN_GROUP_VARIABLE] = original };

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
