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

    private static IEnumerable<int> IndexesOfVarArgsParameter(IMethodSymbol symbol)
    {
        return symbol.Parameters
            .Where(x => x
                .GetAttributes()
                .Any(y => y.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute")
            )
            .Select(x => symbol.Parameters.IndexOf(x));
    }

    private static int CalculateVarArgsSize(
        IMethodSymbol method,
        IParameterSymbol parameter,
        int offset,
        IEnumerable<int> indexes,
        InvocationExpressionSyntax invocationExpressionSyntax)
    {
        var index = method.Parameters.IndexOf(parameter);

        if (method.IsExtensionMethod)
            index--;

        if (invocationExpressionSyntax.ArgumentList.Arguments.Count <= index)
            return -1;

        if (indexes.Last() == index)
        {
            // we can count the difference between the remaining non-optional parameters and the specified ones
            return invocationExpressionSyntax.ArgumentList.Arguments
                .Skip(index + 1 + offset)
                .Count(x =>
                    x.NameColon is null ||
                    method.Parameters
                        .Skip(index)
                        .Any(y => y.Name == x.NameColon.Name.Identifier.ValueText)
                ) - method.Parameters.Skip(index + 1).Count(x => !x.IsOptional);
        }

        // we count up to a name colon
        return invocationExpressionSyntax.ArgumentList.Arguments
            .Skip(index + 1 + offset)
            .TakeWhile(x => x.NameColon is null)
            .Count();
    }

    public static void Apply(
        ref MethodDeclarationSyntax syntax,
        InvocationExpressionSyntax invocationExpression,
        FunctionGenerator.MethodTarget target,
        SemanticModel semanticModel,
        Logger logger
    )
    {
        var varargIndexes = IndexesOfVarArgsParameter(target.MethodSymbol).ToArray();

        var totalExtraArgs = invocationExpression.ArgumentList.Arguments
                                 .Select((x, i) => (Argument: x, Index: i))
                                 .Count(x =>
                                     x.Index <= varargIndexes.Min() || x.Argument.NameColon is null
                                 )
                             - target.MethodSymbol.Parameters.Count(x => !x.IsOptional);

        if (target.MethodSymbol.IsExtensionMethod)
            totalExtraArgs++;

        if (totalExtraArgs <= 0)
        {
            return;
        }

        logger.Log($"Processing {target.MethodSymbol} with {totalExtraArgs}");

        var iterationOffset = 0;
        var varargParameters = new Dictionary<string, (int Size, int Nth, int Offset)>();

        var offset = target.MethodSymbol.IsExtensionMethod ? 1 : 0;

        for (var index = offset; index < target.MethodSymbol.Parameters.Length; index++)
        {
            var parameterNode = target.MethodSyntax.ParameterList.Parameters[index];

            var parameter = target.MethodSymbol.Parameters[index];

            var varargsAttribute = parameter.GetAttributes().FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == "Discord.VariableFuncArgsAttribute");

            if (varargsAttribute is null) continue;

            var funcGenericTypes = parameterNode
                .DescendantNodes()
                .OfType<TypeArgumentListSyntax>()
                .FirstOrDefault()
                ?.ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .Select(x => x.Identifier.ValueText);

            if (funcGenericTypes is null)
            {
                logger.Log(LogLevel.Error,
                    $"Somethings really sussy bro: {parameterNode} | {parameter}");
                return;
            }

            var insertIndex = (varargsAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "InsertAt")
                .Value.Value as int?) ?? 0;

            var size = CalculateVarArgsSize(
                target.MethodSymbol,
                parameter,
                iterationOffset,
                varargIndexes,
                invocationExpression
            );
            // var size = target.MethodSymbol.Parameters.Length <= index + 1
            //     ? invocationExpression.ArgumentList.Arguments.Count - (target.MethodSymbol.Parameters.Length - offset)
            //     : invocationExpression.ArgumentList.Arguments
            //         .Skip(index - offset)
            //         .TakeWhile(x => x.NameColon is null)
            //         .Count() - 1;

            var vargOffset = iterationOffset;
            iterationOffset += size;

            varargParameters.Add(parameterNode.Identifier.ValueText, (size, vargOffset, insertIndex));

            logger.Log($"Varg parameter: {parameter.Name} > {vargOffset}..{size}\n");

            var typeArgsList = (parameterNode.Type as GenericNameSyntax)!
                .TypeArgumentList
                .Arguments
                .InsertRange(
                    insertIndex,
                    Enumerable.Range(0, size)
                        .Select(x => SyntaxFactory.IdentifierName($"VARG{vargOffset + x}"))
                );

            syntax = syntax.ReplaceNode(
                syntax.ParameterList.Parameters.First(x => x.Identifier.IsEquivalentTo(parameterNode.Identifier)),
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

            var newTypeArgs = Enumerable.Range(0, size)
                .Select(x => SyntaxFactory.TypeParameter($"VARG{vargOffset + x}"));

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
                syntax = syntax.WithTypeParameterList(
                    SyntaxFactory.TypeParameterList(
                        SyntaxFactory.SeparatedList(newTypeArgs)
                    )
                );
            }

            syntax = syntax.WithParameterList(
                syntax.ParameterList.WithParameters(syntax.ParameterList.Parameters.InsertRange(
                    index + 1 + vargOffset,
                    Enumerable.Range(0, size)
                        .Select(x => SyntaxFactory.Parameter(
                                new SyntaxList<AttributeListSyntax>(),
                                new SyntaxTokenList(),
                                SyntaxFactory.IdentifierName($"VARG{vargOffset + x}")
                                    .WithTrailingTrivia(SyntaxFactory.Whitespace(" ")),
                                SyntaxFactory.Identifier($"vararg{vargOffset + x}"),
                                null
                            )
                        )
                ))
            );
        }

        if (varargParameters.Count == 0) return;

        syntax = syntax.ReplaceNodes(
            syntax.Body!
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Where(x =>
                    x.Expression is IdentifierNameSyntax identifierNameSyntax &&
                    varargParameters.ContainsKey(identifierNameSyntax.Identifier.ValueText)
                ),
            (node, _) =>
            {
                if (node.Expression is not IdentifierNameSyntax identifierNameSyntax)
                    return node;

                var info = varargParameters[identifierNameSyntax.Identifier.ValueText];

                var newArgFuncList = node.ArgumentList.Arguments
                    .InsertRange(
                        info.Offset,
                        Enumerable.Range(0, info.Size)
                            .Select(x => SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName(
                                    $"vararg{x + info.Nth}")
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
