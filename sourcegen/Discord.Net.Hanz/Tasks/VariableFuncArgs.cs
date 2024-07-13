using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class VariableFuncArgs : IGenerationCombineTask<VariableFuncArgs.GenerationTarget>
{
    private static List<MethodTarget> _methods = new();

    public class MethodTarget(
        string simpleName,
        string fullName,
        MethodDeclarationSyntax methodSyntax,
        IMethodSymbol methodSymbol)
    {
        public string SimpleName { get; } = simpleName;
        public string FullName { get; } = fullName;
        public MethodDeclarationSyntax MethodSyntax { get; set; } = methodSyntax;
        public IMethodSymbol MethodSymbol { get; set; } = methodSymbol;

        public bool CanBeCalledFor(ITypeSymbol node, SemanticModel semanticModel)
        {
            if (MethodSymbol.ContainingType.Equals(node, SymbolEqualityComparer.Default))
                return true;

            if (semanticModel.Compilation.ClassifyCommonConversion(node, MethodSymbol.ContainingType).Exists)
                return true;

            if (MethodSymbol.IsExtensionMethod)
            {
                var extensionParameter = MethodSymbol.Parameters.FirstOrDefault();

                if (extensionParameter is null)
                    return false;

                var canUse = semanticModel.Compilation.ClassifyCommonConversion(node, extensionParameter.Type);
                var isEq = node.Equals(extensionParameter.Type, SymbolEqualityComparer.Default);

                if (canUse.Exists || isEq)
                    return true;
            }

            return false;
        }
    }

    public class GenerationTarget(
        SemanticModel semanticModel,
        MethodTarget method,
        ArgumentListSyntax arguments
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public MethodTarget Method { get; } = method;
        public ArgumentListSyntax Arguments { get; } = arguments;
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        switch (node)
        {
            case MethodDeclarationSyntax method:
                return method.ParameterList.Parameters.Any(x => x.AttributeLists.Count > 0);
            case InvocationExpressionSyntax invocation:
                var methodName = invocation.Expression.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault();
                if (methodName is null) return false;

                return _methods.Any(x => x.SimpleName == methodName.Identifier.ValueText);
            default: return false;
        }
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        switch (context.Node)
        {
            case MethodDeclarationSyntax method:
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(method);

                if (methodSymbol is null)
                    return null;

                var existing = _methods.FirstOrDefault(x => x.FullName == methodSymbol.ToDisplayString());

                var hasVarFuncParameter = methodSymbol.Parameters
                    .Any(x => x.GetAttributes()
                        .Any(y => y.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute")
                    );

                if (existing is not null && hasVarFuncParameter)
                {
                    existing.MethodSyntax = method;
                    existing.MethodSymbol = methodSymbol;
                }
                else if (existing is not null && !hasVarFuncParameter)
                {
                    _methods.Remove(existing);
                }
                else if (hasVarFuncParameter)
                {
                    _methods.Add(new MethodTarget(
                        method.Identifier.ValueText,
                        methodSymbol.ToDisplayString(),
                        method,
                        methodSymbol
                    ));
                }

                return null;
            case InvocationExpressionSyntax invocation:
                var invokeType = invocation.Expression switch
                {
                    MemberAccessExpressionSyntax member => context.SemanticModel.GetTypeInfo(member.Expression).Type,
                    _ => null
                };

                if (invokeType is null) return null;

                var target = _methods.FirstOrDefault(x => x.CanBeCalledFor(invokeType, context.SemanticModel));

                if (target is null)
                {
                    return null;
                }

                return new GenerationTarget(
                    context.SemanticModel,
                    target,
                    invocation.ArgumentList
                );
        }
        return null;
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
    {
        var processed = new Dictionary<string, HashSet<int>>();

        try
        {
            foreach (var target in targets.Distinct())
            {
                if (target is null)
                {
                    continue;
                }

                if (target.Method.MethodSyntax.Parent is not TypeDeclarationSyntax typeDeclarationSyntax)
                    return;

                if (target.Method.MethodSyntax.Body is null)
                {
                    Hanz.Logger.Warn("Syntax couldn't be resolved");
                    continue;
                }

                var newFunctionSyntax = target.Method.MethodSyntax;

                var extraArgs = target.Arguments.Arguments.Count - target.Method.MethodSymbol.Parameters.Length;

                if (target.Method.MethodSymbol.IsExtensionMethod)
                    extraArgs++;

                if (extraArgs <= 0)
                {
                    Hanz.Logger.Warn("No extra args");
                    continue;
                }

                if (processed.TryGetValue(target.Method.MethodSymbol.ToDisplayString(), out var generatedExtras) &&
                    generatedExtras.Contains(extraArgs))
                {
                    continue;
                }

                string? varargFuncParameterIdentifier = null;

                var offset = target.Method.MethodSymbol.IsExtensionMethod ? 1 : 0;

                for (var index = offset; index < target.Method.MethodSymbol.Parameters.Length; index++)
                {
                    var parameterNode = target.Method.MethodSyntax.ParameterList.Parameters[index];

                    var parameter = target.Method.MethodSymbol.Parameters[index];

                    if (parameter.GetAttributes().Any(x =>
                            x.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute"))
                    {
                        var funcGenericTypes = parameterNode
                            .DescendantNodes()
                            .OfType<TypeArgumentListSyntax>()
                            .FirstOrDefault()
                            ?.ChildNodes()
                            .OfType<IdentifierNameSyntax>()
                            .Select(x => x.Identifier.ValueText);

                        if (funcGenericTypes is null)
                        {
                            Hanz.Logger.Log(LogLevel.Error, $"Somethings really sussy bro: {parameterNode} | {parameter}");
                            return;
                        }

                        varargFuncParameterIdentifier = parameterNode.Identifier.ValueText;

                        var typeArgsList = (parameterNode.Type as GenericNameSyntax)!
                            .TypeArgumentList
                            .Arguments
                            .InsertRange(
                                0,
                                Enumerable.Range(0, extraArgs)
                                    .Select(x => SyntaxFactory.IdentifierName($"VARG{x}"))
                            );

                        newFunctionSyntax = newFunctionSyntax.ReplaceNode(
                            parameterNode,
                            SyntaxFactory.Parameter(
                                new SyntaxList<AttributeListSyntax>(),
                                parameterNode.Modifiers,
                                SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("Func"),
                                    SyntaxFactory.TypeArgumentList(typeArgsList)
                                ),
                                parameterNode.Identifier,
                                parameterNode.Default
                            )
                        );

                        var newTypeArgs = Enumerable.Range(0, extraArgs)
                            .Select(x => SyntaxFactory.TypeParameter($"VARG{x}"));

                        if (newFunctionSyntax.TypeParameterList is not null)
                        {
                            newFunctionSyntax = newFunctionSyntax.ReplaceNode(
                                newFunctionSyntax.TypeParameterList,
                                SyntaxFactory.TypeParameterList(
                                    newFunctionSyntax
                                        .TypeParameterList
                                        .Parameters
                                        .AddRange(newTypeArgs)
                                )
                            );
                        }
                        else
                        {
                            newFunctionSyntax =
                                newFunctionSyntax.WithTypeParameterList(
                                    SyntaxFactory.TypeParameterList(
                                            SyntaxFactory.SeparatedList(newTypeArgs)
                                        )
                                );
                        }

                        var newParameters = newFunctionSyntax.ParameterList.Parameters.AddRange(
                            Enumerable.Range(0, extraArgs)
                                .Select(x => SyntaxFactory.Parameter(
                                        new SyntaxList<AttributeListSyntax>(),
                                        new SyntaxTokenList(),
                                        SyntaxFactory.IdentifierName($"VARG{x}")
                                            .WithTrailingTrivia(SyntaxFactory.Whitespace(" ")),
                                        SyntaxFactory.Identifier($"vararg{x}"),
                                        null
                                    )
                                )
                        );

                        newFunctionSyntax = newFunctionSyntax.ReplaceNode(
                            newFunctionSyntax.ParameterList,
                            SyntaxFactory
                                .ParameterList(newParameters)
                        );
                    }
                }

                if (varargFuncParameterIdentifier is null) continue;

                foreach (var node in newFunctionSyntax.Body!
                             .DescendantNodes()
                             .OfType<InvocationExpressionSyntax>()
                             .Where(x =>
                                 x.Expression is IdentifierNameSyntax identifierNameSyntax &&
                                 identifierNameSyntax.Identifier.ValueText == varargFuncParameterIdentifier
                             ))
                {
                    var newArgFuncList = node.ArgumentList.Arguments
                        .InsertRange(
                            0,
                            Enumerable.Range(0, extraArgs)
                                .Select(x => SyntaxFactory.Argument(
                                    SyntaxFactory.IdentifierName($"vararg{x}")
                                ))
                        );

                    var newInvocation = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(varargFuncParameterIdentifier),
                        SyntaxFactory.ArgumentList(
                            newArgFuncList
                        )
                    );

                    newFunctionSyntax = newFunctionSyntax.ReplaceNode(node, newInvocation);
                }

                typeDeclarationSyntax = typeDeclarationSyntax
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>([newFunctionSyntax]))
                    .WithAttributeLists([])
                    .WithBaseList(null);

                context.AddSource(
                    $"VarArgFuncs/{target.Method.MethodSymbol.ContainingType.Name}_{target.Method.MethodSymbol.Name}_VARGS{extraArgs}",
                    $$"""
                      {{string.Join("\n", target.Method.MethodSyntax.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>())}}
                      namespace {{target.Method.MethodSymbol.ContainingType.ContainingNamespace}};

                      {{typeDeclarationSyntax.NormalizeWhitespace()}}
                      """
                );

                if (!processed.TryGetValue(target.Method.MethodSymbol.ToDisplayString(), out generatedExtras))
                    processed[target.Method.MethodSymbol.ToDisplayString()] = generatedExtras = [];

                generatedExtras.Add(extraArgs);
            }
        }
        catch (Exception x)
        {
            Hanz.Logger.Log(LogLevel.Error, x.ToString());
        }
    }

    private static bool IsNotGeneratedMethodInfo(SyntaxNode node)
    {
        if (node is not MethodDeclarationSyntax method) return false;

        return method.AttributeLists.SelectMany(x => x.Attributes).All(x => x.Name.ToString() != "CompilerGenerated");
    }
}
