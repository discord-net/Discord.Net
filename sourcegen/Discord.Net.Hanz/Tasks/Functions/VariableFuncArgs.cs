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
    public static bool IsTargetMethod(IMethodSymbol symbol)
    {
        return symbol.Parameters
            .Any(x => x.GetAttributes()
                .Any(y => y.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute")
            );
    }

    private static int IndexOfVarArgsParameter(IMethodSymbol symbol)
    {
        return symbol.Parameters.IndexOf(
            symbol.Parameters.First(x => x
                .GetAttributes()
                .Any(y => y.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute"))
        );
    }

    public static void Apply(
        ref MethodDeclarationSyntax syntax,
        InvocationExpressionSyntax invocationExpression,
        FunctionGenerator.MethodTarget target
    )
    {
        var varargIndex = IndexOfVarArgsParameter(target.MethodSymbol);

        var extraArgs = invocationExpression.ArgumentList.Arguments
                            .Select((x, i) => (Argument: x, Index: i))
                            .Count(x =>
                                x.Index <= varargIndex || x.Argument.NameColon is null
                            )
                        - target.MethodSymbol.Parameters.Count(x => !x.IsOptional);

        if (target.MethodSymbol.IsExtensionMethod)
            extraArgs++;

        if (extraArgs <= 0)
        {
            Hanz.Logger.Warn("No extra args");
            return;
        }

        string? varargFuncParameterIdentifier = null;

        var offset = target.MethodSymbol.IsExtensionMethod ? 1 : 0;

        for (var index = offset; index < target.MethodSymbol.Parameters.Length; index++)
        {
            var parameterNode = target.MethodSyntax.ParameterList.Parameters[index];

            var parameter = target.MethodSymbol.Parameters[index];

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
                    Hanz.Logger.Log(LogLevel.Error,
                        $"Somethings really sussy bro: {parameterNode} | {parameter}");
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

                syntax = syntax.ReplaceNode(
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

                if (syntax.TypeParameterList is not null)
                {
                    syntax = syntax.ReplaceNode(
                        syntax.TypeParameterList,
                        SyntaxFactory.TypeParameterList(
                            syntax
                                .TypeParameterList
                                .Parameters
                                .AddRange(newTypeArgs)
                        )
                    );
                }
                else
                {
                    syntax =
                        syntax.WithTypeParameterList(
                            SyntaxFactory.TypeParameterList(
                                SyntaxFactory.SeparatedList(newTypeArgs)
                            )
                        );
                }

                var newParameters = SyntaxFactory
                    .ParameterList(
                        SyntaxFactory.SeparatedList([
                            ..syntax.ParameterList.Parameters
                                .Where(x => x.Default is null),
                            ..Enumerable.Range(0, extraArgs)
                                .Select(x => SyntaxFactory.Parameter(
                                        new SyntaxList<AttributeListSyntax>(),
                                        new SyntaxTokenList(),
                                        SyntaxFactory.IdentifierName($"VARG{x}")
                                            .WithTrailingTrivia(SyntaxFactory.Whitespace(" ")),
                                        SyntaxFactory.Identifier($"vararg{x}"),
                                        null
                                    )
                                ),
                            ..syntax.ParameterList.Parameters
                                .Where(x => x.Default is not null)
                        ])
                    );

                syntax = syntax.ReplaceNode(
                    syntax.ParameterList,
                    newParameters
                );
            }
        }

        if (varargFuncParameterIdentifier is null) return;

        syntax = syntax.ReplaceNodes(
            syntax.Body!
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(x =>
                    x.Expression is IdentifierNameSyntax identifierNameSyntax &&
                    identifierNameSyntax.Identifier.ValueText == varargFuncParameterIdentifier
                ),
            (node, _) =>
            {
                var newArgFuncList = node.ArgumentList.Arguments
                    .InsertRange(
                        0,
                        Enumerable.Range(0, extraArgs)
                            .Select(x => SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName($"vararg{x}")
                            ))
                    );

                return SyntaxFactory.InvocationExpression(
                    node.Expression,
                    SyntaxFactory.ArgumentList(
                        newArgFuncList
                    )
                );
            }
        );
    }
}
