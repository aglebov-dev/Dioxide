using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dioxide.Contracts;
using Dioxide.Tools;
using Dioxide.Tools.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dioxide.Proxy.Parts
{
    internal class MethodBuilder
    {
        private readonly INamedTypeSymbol _methodResultSymbols;
        private readonly INamedTypeSymbol _methodArgsSymbols;
        private readonly INamedTypeSymbol _exceptionSymbols;
        private readonly HashSet<INamedTypeSymbol> _voidAsyncTypes;
        private readonly HashSet<INamedTypeSymbol> _asyncTypes;

        public MethodBuilder(Compilation compilation)
        {
            _methodResultSymbols = compilation.GetSymbols<MethodResult>();
            _methodArgsSymbols = compilation.GetSymbols<MethodArgs>();
            _exceptionSymbols = compilation.GetSymbols<Exception>();

            _voidAsyncTypes = new HashSet<INamedTypeSymbol>
            {
                compilation.GetTypeByMetadataName(typeof(Task).FullName),
                compilation.GetTypeByMetadataName(typeof(ValueTask).FullName),
            };
            _asyncTypes = new HashSet<INamedTypeSymbol>
            {
                compilation.GetTypeByMetadataName(typeof(Task).FullName),
                compilation.GetTypeByMetadataName(typeof(ValueTask).FullName),
                compilation.GetTypeByMetadataName(typeof(Task<>).FullName),
                compilation.GetTypeByMetadataName(typeof(ValueTask<>).FullName),
            };
        }

        public MethodBuildResult CreateMethods(INamedTypeSymbol declaration, CtorBuilderResult ctorResult)
        {
            if (IsSupportedType(declaration))
            {
                var innerMethods = GetMethods(declaration);
                var externalMethods = declaration.Interfaces.SelectMany(GetMethods);
                var methods = innerMethods.Concat(externalMethods).ToArray();

                var methodDeclarations = methods
                    .Select(methodSymbols => CreateMethod(methodSymbols, ctorResult))
                    .ToArray();

                return new MethodBuildResult(methodDeclarations);
            }

            throw new ArgumentException("The kind of type must be a interface");
        }

        private bool IsSupportedType(INamedTypeSymbol declaration)
        {
            //TODO added classes (base ctor and ect)
            return declaration.TypeKind == TypeKind.Interface;
        }

        private IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol symbol)
        {
            return symbol.GetMembers()
                .Where(x => x.Kind == SymbolKind.Method)
                .Where(x => x.IsAbstract || x.IsVirtual)
                .OfType<IMethodSymbol>()
                .Where(x => x.MethodKind == MethodKind.Ordinary);
        }

        private MethodDeclarationSyntax CreateMethod(IMethodSymbol methodSymbol, CtorBuilderResult ctorResult)
        {
            var variables = new MethodVariables(methodSymbol);
            var parameters = new MethodParameters(methodSymbol);

            var beforTryBody = BeforeTryBlock(methodSymbol, variables, parameters);
            var tryBody = TryBlock(methodSymbol, ctorResult, variables, parameters).ToArray();
            var catchBody = CatchBlock(methodSymbol, variables).ToArray();
            var finalyBody = FinalyBlock(methodSymbol, variables).ToArray();

            var catchClause = CatchClause()
                .WithDeclaration(CatchDeclaration(_exceptionSymbols.ToQualifiedTypeName())
                .WithIdentifier(Identifier(variables.CATCH_BLOCK_EXCEPTION_VARIABLE)))
                .WithBlock(Block(catchBody));

            var finalyClause = FinallyClause()
                .WithBlock(Block(finalyBody));

            var tryBlock = TryStatement()
                .WithBlock(Block(tryBody))
                .AddCatches(catchClause)
                .WithFinally(finalyClause);

            var statements = beforTryBody.Append(tryBlock).ToArray();

            var @body = Block(statements);

            var returnType = methodSymbol.ReturnType.ToQualifiedTypeName();
            var methodName = Identifier(methodSymbol.Name);
            var methodModifiers = GetModifiers(methodSymbol);
            var methodParameters = parameters.Parameters.Select(x => x.Syntax).ToArray();
            return MethodDeclaration(returnType, methodName)
                .WithModifiers(methodModifiers)
                .AddParameterListParameters(methodParameters)
                .WithBody(@body)
                ;
        }

        private SyntaxTokenList GetModifiers(IMethodSymbol methodSymbol)
        {
            var x = new LinkedList<SyntaxKind>(new[] { SyntaxKind.PublicKeyword });

            if (methodSymbol.IsVirtual)
            {
                x.AddLast(SyntaxKind.OverrideKeyword);
            }
            if (methodSymbol.IsAbstract)
            {
                //interfaces and abstract method from class
            }
            if (_asyncTypes.Contains(methodSymbol.ReturnType.OriginalDefinition))
            {
                x.AddLast(SyntaxKind.AsyncKeyword);
            }

            var tokens = x.Select(Token).ToArray();

            return  TokenList(tokens);
        }

        private IEnumerable<StatementSyntax> BeforeTryBlock(IMethodSymbol methodSymbol, MethodVariables variables, MethodParameters args)
        {
            yield return variables.METHOD_NAME_VARIABLE.VarEqualsStringStatement(methodSymbol.Name);
            yield return variables.ARGS_VARIABLE.VarEqualsStatement(_methodArgsSymbols.GlobalTypeName().NewExpression());
            yield return variables.RESULT_VARIABLE.VarEqualsStatement(_methodResultSymbols.GlobalTypeName().NewExpression());

            foreach (var argument in args.Parameters)
            {
                yield return variables.ARGS_VARIABLE.InvokeSyncStatement(
                    Names.METHOD_ARGS_SET_METHOD,
                    argument.OriginalName.AsLiteral(), 
                    IdentifierName(argument.Name)
                );
            }
        }

        private IEnumerable<StatementSyntax> TryBlock(IMethodSymbol methodSymbol, CtorBuilderResult ctorResult, MethodVariables variables, MethodParameters args)
        {
            var isResult = IsResult(methodSymbol);
            var isAsync = IsAsync(methodSymbol);

            yield return Names.ENTER_METHOD.CallMethodSyncStatement(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE)
            );

            var arguments = args.Parameters.Select(x => IdentifierName(x.Name)).ToArray();
            if (isResult)
            {
                var resultExpression = isAsync
                    ? ctorResult.OriginFieldIdentifier.InvokeAwaitExpression(methodSymbol.Name, arguments)
                    : ctorResult.OriginFieldIdentifier.InvokeSyncExpression(methodSymbol.Name, arguments);

                yield return variables.METHOD_RESULT_VARIABLE.VarEqualsStatement(resultExpression);
                yield return variables.RESULT_VARIABLE.InvokeSyncStatement(Names.METHOD_RESULT_SET_METHOD, IdentifierName(variables.METHOD_RESULT_VARIABLE));
            }
            else
            {
                yield return isAsync
                    ? ctorResult.OriginFieldIdentifier.InvokeAwaitStatement(methodSymbol.Name, arguments)
                    : ctorResult.OriginFieldIdentifier.InvokeSyncStatement(methodSymbol.Name, arguments);
            }

            yield return Names.EXIT_METHOD.CallMethodSyncStatement(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE),
                IdentifierName(variables.RESULT_VARIABLE)
            );

            if (isResult)
            {
                yield return ReturnStatement(IdentifierName(variables.METHOD_RESULT_VARIABLE));
            }
        }

        private IEnumerable<StatementSyntax> CatchBlock(IMethodSymbol methodSymbol, MethodVariables variables)
        {
            var isResult = IsResult(methodSymbol);
            if (isResult)
            {
                foreach (var item in CatchOverrideResultBlock(methodSymbol, variables))
                {
                    yield return item;
                }

            }

            var ifExpr = BinaryExpression(
                SyntaxKind.EqualsExpression, 
                IdentifierName(variables.EXCEPTION_VARIABLE),
                LiteralExpression(SyntaxKind.NullLiteralExpression)
            );

            var exp = Names.CATCH_METHOD.CallMethodSyncExpression(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE),
                IdentifierName(variables.CATCH_BLOCK_EXCEPTION_VARIABLE)
            );

            yield return variables.EXCEPTION_VARIABLE.VarEqualsStatement(exp);
            yield return IfStatement(ifExpr,
                Block(ThrowStatement()),
                ElseClause(Block(ThrowStatement(IdentifierName(variables.EXCEPTION_VARIABLE))))
            );
        }

        private IEnumerable<StatementSyntax> CatchOverrideResultBlock(IMethodSymbol methodSymbol, MethodVariables variables)
        {
            var overrideResult = "overrideResult";

            var isAsync = IsAsync(methodSymbol);
            var returnType = methodSymbol.ReturnType.ToQualifiedTypeName();

            var overrideResultExpr = Names.CATCH_OVERRIDE_RESULT_METHOD.CallMethodSyncExpression(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE),
                IdentifierName(variables.CATCH_BLOCK_EXCEPTION_VARIABLE)
            );

            var hasResultInvokeExpr = overrideResult.InvokeSyncGenericExpression(
                method: Names.METHOD_RESULT_HAS_RESULT_METHOD, 
                genericType: returnType
            );

            var getResultOrDefaultInvokeExpr = overrideResult.InvokeSyncGenericExpression(
                method: Names.GET_RESULT_OR_DEFAULT,
                genericType: returnType
            );

            var returnExpr = isAsync
                ? AwaitExpression(getResultOrDefaultInvokeExpr)
                : getResultOrDefaultInvokeExpr;

            var exitMethod = Names.EXIT_METHOD.CallMethodSyncStatement(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE),
                IdentifierName(overrideResult)
            );

            yield return overrideResult.VarEqualsStatement(overrideResultExpr);
            yield return IfStatement(hasResultInvokeExpr,
                 Block(exitMethod, ReturnStatement(returnExpr))
            );
        }

        private IEnumerable<StatementSyntax> FinalyBlock(IMethodSymbol methodSymbol, MethodVariables variables)
        {
            yield return Names.FINALY_METHOD.CallMethodSyncStatement(
                IdentifierName(variables.METHOD_NAME_VARIABLE),
                IdentifierName(variables.ARGS_VARIABLE),
                IdentifierName(variables.RESULT_VARIABLE)
            );
        }

        public bool IsResult(IMethodSymbol symbol)
        {
            var originalReturnDefinition = symbol.ReturnType.OriginalDefinition;
            return originalReturnDefinition.SpecialType == SpecialType.System_Void
                ? false
                : !_voidAsyncTypes.Contains(originalReturnDefinition);
        }

        public bool IsAsync(IMethodSymbol symbol)
        {
            var originalReturnDefinition = symbol.ReturnType.OriginalDefinition;
            return originalReturnDefinition.SpecialType == SpecialType.System_Void
                ? false
                : _asyncTypes.Contains(symbol.ReturnType.OriginalDefinition);
        }
    }
}
