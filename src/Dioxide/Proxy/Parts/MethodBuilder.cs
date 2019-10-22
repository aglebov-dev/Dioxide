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
    internal class MethodBuildResult
    {
        public MethodDeclarationSyntax[] Methods { get; }

        public MethodBuildResult(MethodDeclarationSyntax[] methods)
        {
            Methods = methods;
        }
    }

    internal class MethodBuilder
    {
        private readonly Identifiers _idHelper;
        private readonly INamedTypeSymbol _resultSymbols;
        private readonly INamedTypeSymbol _argsSymbols;
        private readonly INamedTypeSymbol _exceptionSymbols;
        private readonly HashSet<INamedTypeSymbol> _voidAsyncTypes;
        private readonly HashSet<INamedTypeSymbol> _asyncTypes;

        public MethodBuilder(Compilation compilation)
        {
            _idHelper = new Identifiers();
            _resultSymbols = compilation.GetSymbols<MethodResult>();
            _argsSymbols = compilation.GetSymbols<MethodArgs>();
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

                var helper = new InterceptorsGroupHelper(ctorResult.VisitorFieldIdentifier);
                var methods = innerMethods.Concat(externalMethods).ToArray();

                var s = methods.Select(x => CreateMethod(x, ctorResult, helper)).ToArray();

                return new MethodBuildResult(s);
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

        private MethodDeclarationSyntax CreateMethod(IMethodSymbol methodSymbol, CtorBuilderResult ctorResult, InterceptorsGroupHelper helper)
        {
            var beforTryBody = BeforeTryBlock(methodSymbol);
            var tryBody = TryBlock(methodSymbol, ctorResult, helper).ToArray();
            var catchBody = CatchBlock(methodSymbol, helper).ToArray();
            var finalyBody = FinalyBlock(methodSymbol, helper).ToArray();

            var catchClause = CatchClause()
                .WithDeclaration(CatchDeclaration(_exceptionSymbols.ToQualifiedTypeName())
                .WithIdentifier(Identifier(_idHelper.ExceptionVar)))
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
            var methodParameters = methodSymbol.Parameters.Select(x => x.Type.ParameterExpression(x.Name)).ToArray();
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

        private IEnumerable<StatementSyntax> BeforeTryBlock(IMethodSymbol methodSymbol)
        {
            var isResult = IsResult(methodSymbol);
            yield return _idHelper.MethodVar.VarEqualsStringStatement(methodSymbol.Name);
            yield return _idHelper.ArgsVar.VarEqualsStatement(_argsSymbols.GlobalTypeName().NewExpression());
            yield return _idHelper.ResultVar.VarEqualsStatement(_resultSymbols.GlobalTypeName().NewExpression());

            var arguments = methodSymbol.Parameters.Select(x => new { Name = x.Name.AsLiteral(), Variable = IdentifierName(x.Name) });
            foreach (var argument in arguments)
            {
                yield return _idHelper.ArgsVar.InvokeSyncStatement(_idHelper.MethodArgsSet, argument.Name, argument.Variable);
            }
        }

        private IEnumerable<StatementSyntax> TryBlock(IMethodSymbol methodSymbol, CtorBuilderResult ctorResult, InterceptorsGroupHelper helper)
        {
            var isResult = IsResult(methodSymbol);
            var isAsync = IsAsync(methodSymbol);

            yield return helper.EnterMethod.CallMethodSyncStatement(
                IdentifierName(_idHelper.MethodVar),
                IdentifierName(_idHelper.ArgsVar)
            );

            var arguments = methodSymbol.Parameters.Select(x => IdentifierName(x.Name)).ToArray();
            if (isResult)
            {
                var resultExpression = isAsync
                    ? ctorResult.OriginFieldIdentifier.InvokeAwaitExpression(methodSymbol.Name, arguments)
                    : ctorResult.OriginFieldIdentifier.InvokeSyncExpression(methodSymbol.Name, arguments);

                yield return _idHelper.MethodResultVar.VarEqualsStatement(resultExpression);
                yield return _idHelper.ResultVar.InvokeSyncStatement(_idHelper.MethodResultSet, IdentifierName(_idHelper.MethodResultVar));
            }
            else
            {
                yield return isAsync
                    ? ctorResult.OriginFieldIdentifier.InvokeAwaitStatement(methodSymbol.Name, arguments)
                    : ctorResult.OriginFieldIdentifier.InvokeSyncStatement(methodSymbol.Name, arguments);
            }

            yield return helper.ExitMethod.CallMethodSyncStatement(
                IdentifierName(_idHelper.MethodVar),
                IdentifierName(_idHelper.ArgsVar),
                IdentifierName(_idHelper.ResultVar)
            );

            if (isResult)
            {
                yield return ReturnStatement(IdentifierName(_idHelper.MethodResultVar));
            }
        }

        private IEnumerable<StatementSyntax> CatchBlock(IMethodSymbol methodSymbol, InterceptorsGroupHelper helper)
        {
            var isResult = IsResult(methodSymbol);
            if (isResult)
            {
                foreach (var item in CatchOverrideResultBlock(methodSymbol, helper))
                {
                    yield return item;
                }

            }

            var ifExpr = BinaryExpression(
                SyntaxKind.EqualsExpression, 
                IdentifierName(_idHelper.CarchResultVar),
                LiteralExpression(SyntaxKind.NullLiteralExpression)
            );

            var exp = helper.CatchMethod.CallMethodSyncExpression(
                IdentifierName(_idHelper.MethodVar),
                IdentifierName(_idHelper.ArgsVar),
                IdentifierName(_idHelper.ExceptionVar)
            );

            yield return _idHelper.CarchResultVar.VarEqualsStatement(exp);
            yield return IfStatement(ifExpr,
                Block(ThrowStatement()),
                ElseClause(Block(ThrowStatement(IdentifierName(_idHelper.CarchResultVar))))
            );
        }

        private IEnumerable<StatementSyntax> CatchOverrideResultBlock(IMethodSymbol methodSymbol, InterceptorsGroupHelper helper)
        {
            var overrideResult = "overrideResult";

            var isAsync = IsAsync(methodSymbol);
            var returnType = methodSymbol.ReturnType.ToQualifiedTypeName();

            var overrideResultExpr = helper.CatchOverrideResultMethod.CallMethodSyncExpression(
                IdentifierName(_idHelper.MethodVar),
                IdentifierName(_idHelper.ArgsVar),
                IdentifierName(_idHelper.ExceptionVar)
            );

            var hasResultInvokeExpr = overrideResult.InvokeSyncGenericExpression(
                method: _idHelper.HasResult, 
                genericType: returnType
            );

            var getResultOrDefaultInvokeExpr = overrideResult.InvokeSyncGenericExpression(
                method: _idHelper.GetResultOrDefault,
                genericType: returnType
            );

            var returnExpr = isAsync
                ? AwaitExpression(getResultOrDefaultInvokeExpr)
                : getResultOrDefaultInvokeExpr;

            yield return overrideResult.VarEqualsStatement(overrideResultExpr);
            yield return IfStatement(hasResultInvokeExpr,
                 Block(ReturnStatement(returnExpr))
            );
        }

        private IEnumerable<StatementSyntax> FinalyBlock(IMethodSymbol methodSymbol, InterceptorsGroupHelper helper)
        {
            yield return helper.FinalyMethod.CallMethodSyncStatement(
                IdentifierName(_idHelper.MethodVar),
                IdentifierName(_idHelper.ArgsVar),
                IdentifierName(_idHelper.ResultVar)
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
