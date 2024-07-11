using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public static class VariableFuncArgs
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        IMethodSymbol method,
        ArgumentListSyntax arguments
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public IMethodSymbol Method { get; } = method;
        public ArgumentListSyntax Arguments { get; } = arguments;
    }

    public static bool IsValid(SyntaxNode node)
    {
        if (node is not InvocationExpressionSyntax invocation) return false;

        return invocation.ArgumentList.Arguments.Count > 0;
    }

    public static GenerationTarget? GetGenerationTarget(GeneratorSyntaxContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation) return null;

        var symbolInfo = ModelExtensions.GetSymbolInfo(context.SemanticModel, invocation);

        if (symbolInfo.Symbol is null)
        {
            foreach (var candidate in symbolInfo.CandidateSymbols)
            {
                if (candidate is not IMethodSymbol candidateMethodSymbol) continue;

                foreach (var parameter in candidateMethodSymbol.Parameters)
                {
                    if (!parameter.GetAttributes().Any(x =>
                            x.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute")
                       ) continue;

                    return new GenerationTarget(context.SemanticModel, candidateMethodSymbol, invocation.ArgumentList);
                }
            }
        }

        return null;
    }

    public static void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
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

                var declaringSyntax = target.Method.DeclaringSyntaxReferences
                        .FirstOrDefault(x => IsNotGeneratedMethodInfo(x.GetSyntax()))
                        ?.GetSyntax()
                    as MethodDeclarationSyntax;


                if (declaringSyntax is null || declaringSyntax.Body is null) continue;

                var newFunctionSyntax = declaringSyntax;


                var extraArgs = target.Arguments.Arguments.Count - target.Method.Parameters.Length;

                if (processed.TryGetValue(target.Method.ToDisplayString(), out var generatedExtras) &&
                    generatedExtras.Contains(extraArgs))
                {
                    continue;
                }

                string? varargFuncParameterIdentifier = null;

                var offset = target.Method.IsExtensionMethod ? 1 : 0;

                for (var index = 0; index < target.Method.Parameters.Length; index++)
                {
                    var parameterNode = declaringSyntax.ParameterList.Parameters[index + offset];

                    var parameter = target.Method.Parameters[index];
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
                            Hanz.Logger.Log(LogLevel.Error, "Somethings really sussy bro");
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
                            ).NormalizeWhitespace()
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
                                ).NormalizeWhitespace()
                            );
                        }
                        else
                        {
                            newFunctionSyntax =
                                newFunctionSyntax.WithTypeParameterList(
                                    SyntaxFactory.TypeParameterList(
                                            SyntaxFactory.SeparatedList(newTypeArgs)
                                        )
                                        .NormalizeWhitespace()
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
                                    ).NormalizeWhitespace()
                                )
                        );

                        newFunctionSyntax = newFunctionSyntax.ReplaceNode(
                            newFunctionSyntax.ParameterList,
                            SyntaxFactory
                                .ParameterList(newParameters)
                                .NormalizeWhitespace()
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
                    ).NormalizeWhitespace();

                    newFunctionSyntax = newFunctionSyntax.ReplaceNode(node, newInvocation);
                }

                context.AddSource(
                    $"VarArgFuncs/{target.Method.ContainingType.Name}_{target.Method.Name}_VARGS{extraArgs}",
                    $$"""
                      {{string.Join("\n", declaringSyntax.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>())}}
                      namespace {{target.Method.ContainingType.ContainingNamespace}};

                      public{{(target.Method.ContainingType.IsStatic ? " static" : "")}} partial class {{target.Method.ContainingType.Name}}
                      {
                          [System.Runtime.CompilerServices.CompilerGenerated]
                          {{newFunctionSyntax}}
                      }
                      """
                );

                if (!processed.TryGetValue(target.Method.ToDisplayString(), out generatedExtras))
                    processed[target.Method.ToDisplayString()] = generatedExtras = [];

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
