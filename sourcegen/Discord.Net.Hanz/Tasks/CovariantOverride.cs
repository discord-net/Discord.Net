using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks;

public static class CovariantOverride
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        MethodDeclarationSyntax methodDeclarationSyntax,
        ClassDeclarationSyntax classDeclarationSyntax
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; } = methodDeclarationSyntax;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
    }

    public static bool IsValid(SyntaxNode node)
        => node is MethodDeclarationSyntax {AttributeLists.Count: > 0};

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not MethodDeclarationSyntax {AttributeLists.Count: > 0} method)
            return null;

        if (method.AttributeLists
            .SelectMany(x => x.Attributes)
            .All(x => Attributes.GetAttributeName(x, context.SemanticModel) != "Discord.CovariantOverrideAttribute"))
            return null;

        if (method.Parent is not ClassDeclarationSyntax classDeclaration) return null;

        if (classDeclaration.BaseList is null) return null;

        return new GenerationTarget(context.SemanticModel, method, classDeclaration);
    }

    private class GenerationResult(
        string ns,
        string usingDirectives,
        ClassDeclarationSyntax syntax
    )
    {
        public string Namespace { get; } = ns;
        public string UsingDirectives { get; } = usingDirectives;
        public ClassDeclarationSyntax Syntax { get; set; } = syntax;
    }

    public static void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
    {
        var toGenerate = new Dictionary<string, GenerationResult>();

        try
        {
            foreach (var target in targets)
            {
                if (target?.ClassDeclarationSyntax.BaseList is null) continue;

                var targetTypeSymbol = target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax);

                if(targetTypeSymbol is null) continue;

                var targetReturnType = ModelExtensions
                    .GetTypeInfo(target.SemanticModel, target.MethodDeclarationSyntax.ReturnType).Type;

                if (targetReturnType is null)
                {
                    Hanz.Logger.Warn(
                        $"No return type can be resolved from {target.MethodDeclarationSyntax.ReturnType}");
                    continue;
                }

                if (ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.MethodDeclarationSyntax) is not
                    IMethodSymbol
                    targetMethodSymbol)
                {
                    Hanz.Logger.Warn(
                        $"No method symbol can be resolved from {target.MethodDeclarationSyntax.Identifier}");
                    continue;
                }

                foreach (var baseType in target.ClassDeclarationSyntax.BaseList.Types)
                {
                    var typeInfo = ModelExtensions.GetTypeInfo(target.SemanticModel, baseType.Type).Type;
                    if (typeInfo is not INamedTypeSymbol namedTypeSymbol)
                    {
                        Hanz.Logger.Warn($"No type info could be found for {baseType.Type}");
                        continue;
                    }

                    if (namedTypeSymbol.TypeKind != TypeKind.Class) continue;

                    var members = typeInfo
                        .GetMembers(target.MethodDeclarationSyntax.Identifier.ValueText)
                        .OfType<IMethodSymbol>();

                    foreach (var member in members)
                    {
                        // verify we can override the member
                        if (!member.IsVirtual)
                        {
                            Hanz.Logger.Warn(
                                $"Member {baseType.Type}.{member.Name} shares the same name, but isn't virtual");
                            continue;
                        }

                        if (!IsTypeOrAssignable(target.SemanticModel.Compilation, targetReturnType, member.ReturnType))
                        {
                            Hanz.Logger.Warn($"{member.ReturnType} is not assignable to {targetReturnType}");
                            continue;
                        }

                        if (member.Parameters.Length != targetMethodSymbol.Parameters.Length)
                        {
                            Hanz.Logger.Warn(
                                $"{member} has mismatch parameter count {member.Parameters.Length} <> {targetMethodSymbol.Parameters.Length}");
                            continue;
                        }

                        var covariantParameters = new HashSet<IParameterSymbol>(SymbolEqualityComparer.Default);

                        foreach (var parameter in member.Parameters.Zip(targetMethodSymbol.Parameters,
                                     (a, b) => (Source: a, Target: b)))
                        {
                            if (
                                !parameter.Source.Type.Equals(parameter.Target.Type, SymbolEqualityComparer.Default) &&
                                target.SemanticModel.Compilation
                                    .ClassifyCommonConversion(parameter.Target.Type, parameter.Source.Type).Exists
                            )
                            {
                                covariantParameters.Add(parameter.Target);
                                continue;
                            }

                            if (parameter.Source.Type.Equals(parameter.Target.Type, SymbolEqualityComparer.Default))
                                continue;

                            Hanz.Logger.Warn($"{parameter.Target} is not assignable to {parameter.Source}");
                            goto end_member;
                        }

                        if (!toGenerate.TryGetValue(targetTypeSymbol.ToDisplayString(), out var generateResult))
                            generateResult = new GenerationResult(
                                ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax)!
                                    .ContainingNamespace.ToString(),
                                string.Join("\n",
                                    target.ClassDeclarationSyntax.SyntaxTree.GetRoot().ChildNodes()
                                        .OfType<UsingDirectiveSyntax>()),
                                target.ClassDeclarationSyntax
                                    .WithBaseList(null)
                                    .WithMembers([])
                            );

                        var resultIsAwaitable =
                            targetMethodSymbol.IsAsync ||
                            member.IsAsync ||
                            targetMethodSymbol.IsAwaitableNonDynamic(
                                target.SemanticModel,
                                target.MethodDeclarationSyntax.SpanStart
                            );

                        var shouldReturn = target.MethodDeclarationSyntax
                            .DescendantNodes()
                            .OfType<InvocationExpressionSyntax>()
                            .Any(x => x.Expression is MemberAccessExpressionSyntax
                                      {
                                          Expression: BaseExpressionSyntax,
                                      } memberAccessExpressionSyntax &&
                                      memberAccessExpressionSyntax.Name.Identifier.ValueText == member.Name
                            );

                        var modifiers = target.MethodDeclarationSyntax.Modifiers
                            .Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));

                        var virtualIndex = modifiers.IndexOf(SyntaxKind.VirtualKeyword);

                        if (virtualIndex >= 0)
                            modifiers = modifiers.RemoveAt(virtualIndex);

                        if (resultIsAwaitable && !shouldReturn)
                            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

                        generateResult.Syntax = generateResult.Syntax
                            .AddMembers(
                                SyntaxFactory.MethodDeclaration(
                                    [],
                                    modifiers,
                                    SyntaxFactory.IdentifierName(member.ReturnType.ToDisplayString()),
                                    null,
                                    SyntaxFactory.Identifier(member.Name),
                                    member.TypeParameters.Length > 0
                                        ? SyntaxFactory.TypeParameterList(
                                            SyntaxFactory.SeparatedList(
                                                member.TypeParameters.Select(x => SyntaxFactory.TypeParameter(
                                                    SyntaxFactory.Identifier(x.Name)
                                                ))
                                            )
                                        )
                                        : null,
                                    SyntaxFactory.ParameterList(
                                        SyntaxFactory.SeparatedList(
                                            member.Parameters.Select(x =>
                                                SyntaxFactory.Parameter(
                                                    [],
                                                    [],
                                                    SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                                                    SyntaxFactory.Identifier(x.Name),
                                                    null
                                                )
                                            )
                                        )
                                    ),
                                    [],
                                    CreateMethodBody(
                                        member,
                                        targetMethodSymbol,
                                        covariantParameters,
                                        resultIsAwaitable,
                                        shouldReturn
                                    ),
                                    null
                                )
                            );

                        toGenerate[targetTypeSymbol.ToDisplayString()] = generateResult;

                        end_member: ;
                    }
                }
            }

            foreach (var generationTarget in toGenerate)
            {
                context.AddSource(
                    $"CovariantOverrides/{generationTarget.Key}",
                    $$"""
                      {{generationTarget.Value.UsingDirectives}}

                      namespace {{generationTarget.Value.Namespace}};

                      {{generationTarget.Value.Syntax.NormalizeWhitespace()}}
                      """
                );
            }
        }
        catch (Exception x)
        {
            Hanz.Logger.Log(LogLevel.Error, x.ToString());
        }
    }

    private static BlockSyntax CreateMethodBody(
        IMethodSymbol baseMethod,
        IMethodSymbol targetMethod,
        HashSet<IParameterSymbol> covariantParameters,
        bool isAwaitable,
        bool shouldReturn
    )
    {
        // in the case where 'shouldReturn' is true, we would be returning the awaitable so we don't
        // need to await the actual tasks
        if (shouldReturn)
            isAwaitable = false;

        // generate the if condition
        var condition = covariantParameters.Select(x =>
            (ExpressionSyntax)SyntaxFactory.IsPatternExpression(
                SyntaxFactory.IdentifierName(x.Name),
                SyntaxFactory.Token(SyntaxKind.IsKeyword),
                SyntaxFactory.DeclarationPattern(
                    SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                    SyntaxFactory.SingleVariableDesignation(
                        SyntaxFactory.Identifier($"{x.Name}__COV")
                    )
                )
            )
        ).Aggregate((a, b) => SyntaxFactory.BinaryExpression(SyntaxKind.LogicalAndExpression, a, b));

        ExpressionSyntax invocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.ThisExpression(SyntaxFactory.Token(SyntaxKind.ThisKeyword)),
                SyntaxFactory.Token(SyntaxKind.DotToken),
                SyntaxFactory.IdentifierName(targetMethod.Name)
            ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    targetMethod.Parameters.Select(x =>
                        SyntaxFactory.Argument(
                            SyntaxFactory.IdentifierName(
                                covariantParameters.Contains(x)
                                    ? $"{x.Name}__COV"
                                    : x.Name
                            )
                        )
                    )
                )
            )
        );

        if (isAwaitable)
            invocation = SyntaxFactory.AwaitExpression(invocation);

        var ifClause = SyntaxFactory.IfStatement(
            condition,
            shouldReturn
                ? SyntaxFactory.ReturnStatement(invocation)
                : SyntaxFactory.ExpressionStatement(invocation)
        );

        ExpressionSyntax baseInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.BaseExpression(SyntaxFactory.Token(SyntaxKind.BaseKeyword)),
                SyntaxFactory.Token(SyntaxKind.DotToken),
                SyntaxFactory.IdentifierName(baseMethod.Name)
            ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                    targetMethod.Parameters.Select(x =>
                        SyntaxFactory.Argument(
                            SyntaxFactory.IdentifierName(x.Name)
                        )
                    )
                )
            )
        );

        if (isAwaitable)
            baseInvocation = SyntaxFactory.AwaitExpression(baseInvocation);

        return SyntaxFactory.Block(
            ifClause,
            shouldReturn
                ? SyntaxFactory.ReturnStatement(baseInvocation)
                : SyntaxFactory.ExpressionStatement(baseInvocation)
        );
    }

    private static bool IsTypeOrAssignable(Compilation compilation, ITypeSymbol a, ITypeSymbol b)
    {
        return compilation.HasImplicitConversion(a, b) || a.Equals(b, SymbolEqualityComparer.Default);
    }
}
