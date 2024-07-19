using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public sealed class FunctionGenerator : IGenerationCombineTask<FunctionGenerator.GenerationTarget>
{
    public class MethodTarget(
        MethodDeclarationSyntax methodSyntax,
        IMethodSymbol methodSymbol) : IEquatable<MethodTarget>
    {
        public MethodDeclarationSyntax MethodSyntax { get; } = methodSyntax;
        public IMethodSymbol MethodSymbol { get; } = methodSymbol;

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

        public bool Equals(MethodTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return MethodSyntax.IsEquivalentTo(other.MethodSyntax) &&
                   MethodSymbol.Equals(other.MethodSymbol, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MethodTarget)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (MethodSyntax.GetHashCode() * 397) ^ SymbolEqualityComparer.Default.GetHashCode(MethodSymbol);
            }
        }

        public static bool operator ==(MethodTarget? left, MethodTarget? right) => Equals(left, right);

        public static bool operator !=(MethodTarget? left, MethodTarget? right) => !Equals(left, right);
    }

    public abstract class GenerationTarget(
        SemanticModel semanticModel
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public abstract bool Equals(GenerationTarget other);
    }

    public sealed class MethodDeclarationGenerationTarget(MethodTarget method, SemanticModel semanticModel)
        : GenerationTarget(semanticModel), IEquatable<MethodDeclarationGenerationTarget>
    {
        public MethodTarget Method { get; } = method;

        public bool Equals(MethodDeclarationGenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Method.Equals(other.Method);
        }

        public override bool Equals(GenerationTarget other) =>
            other is MethodDeclarationGenerationTarget otherMethod && Equals(otherMethod);

        public override bool Equals(object? obj) => ReferenceEquals(this, obj) ||
                                                    obj is MethodDeclarationGenerationTarget other && Equals(other);

        public override int GetHashCode() => Method.GetHashCode();

        public static bool operator ==(MethodDeclarationGenerationTarget? left,
            MethodDeclarationGenerationTarget? right) => Equals(left, right);

        public static bool operator !=(MethodDeclarationGenerationTarget? left,
            MethodDeclarationGenerationTarget? right) => !Equals(left, right);
    }

    public sealed class InvocationGenerationTarget(
        InvocationExpressionSyntax invocationExpression,
        IMethodSymbol? targetMethod,
        SemanticModel semanticModel
    ) : GenerationTarget(semanticModel), IEquatable<InvocationGenerationTarget>
    {
        public InvocationExpressionSyntax InvocationExpression { get; } = invocationExpression;
        public IMethodSymbol? TargetMethod { get; } = targetMethod;

        public bool Equals(InvocationGenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return InvocationExpression.IsEquivalentTo(other.InvocationExpression) &&
                   SymbolEqualityComparer.Default.Equals(TargetMethod, other.TargetMethod);
        }

        public override bool Equals(GenerationTarget other) =>
            other is InvocationGenerationTarget otherThis && Equals(otherThis);

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is InvocationGenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (InvocationExpression.GetHashCode() * 397) ^ (TargetMethod != null
                    ? SymbolEqualityComparer.Default.GetHashCode(TargetMethod)
                    : 0);
            }
        }

        public static bool operator ==(InvocationGenerationTarget? left, InvocationGenerationTarget? right) =>
            Equals(left, right);

        public static bool operator !=(InvocationGenerationTarget? left, InvocationGenerationTarget? right) =>
            !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        return node switch
        {
            MethodDeclarationSyntax method =>
                method.ParameterList.Parameters.Any(x => x.AttributeLists.Count > 0) ||
                (method.TypeParameterList?.Parameters.Any(x => x.AttributeLists.Count > 0) ?? false),
            InvocationExpressionSyntax => true,
            _ => false
        };
    }

    public static bool IsTargetMethod(IMethodSymbol method)
    {
        return VariableFuncArgs.IsTargetMethod(method) || TransitiveFill.IsTargetMethod(method);
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        switch (context.Node)
        {
            case MethodDeclarationSyntax method:
                var methodSymbol = context.SemanticModel.GetDeclaredSymbol(method);

                if (methodSymbol is null)
                    return null;

                return IsTargetMethod(methodSymbol)
                    ? new MethodDeclarationGenerationTarget(new MethodTarget(method, methodSymbol),
                        context.SemanticModel)
                    : null;

            case InvocationExpressionSyntax invocation:
                var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation.Expression);

                if (symbolInfo.Symbol is IMethodSymbol target && IsTargetMethod(target))
                {
                    return new InvocationGenerationTarget(invocation, target, context.SemanticModel);
                }

                var candidate =
                    symbolInfo.CandidateSymbols.FirstOrDefault(x =>
                        x is IMethodSymbol target && IsTargetMethod(target));

                if (candidate is IMethodSymbol candidateTarget)
                    return new InvocationGenerationTarget(invocation, candidateTarget, context.SemanticModel);

                if (symbolInfo.Symbol is null && symbolInfo.CandidateSymbols.Length == 0)
                    return new InvocationGenerationTarget(invocation, null, context.SemanticModel);

                return null;
        }

        return null;
    }

    public static ITypeSymbol? GetInvocationTarget(InvocationExpressionSyntax invocation, SemanticModel semantic)
    {
        switch (invocation.Expression)
        {
            case MemberAccessExpressionSyntax memberAccessExpressionSyntax:
                return semantic.GetTypeInfo(memberAccessExpressionSyntax.Expression).Type;
            default:
                return semantic.GetTypeInfo(invocation.Expression).Type;
        }
    }

    private static bool IsSpeculativeInvocationTarget(MethodTarget method, InvocationExpressionSyntax invocation,
        SemanticModel semantic)
    {
        var target = GetInvocationTarget(invocation, semantic);

        if (target is null)
            return false;

        if (!method.CanBeCalledFor(target, semantic))
            return false;

        return invocation.Expression.ToString().EndsWith(method.MethodSymbol.Name);
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
    {
        var processed =
            new Dictionary<string, (TypeDeclarationSyntax Syntax, HashSet<string> Methods, string Usings, string
                Namespace)>();

        var nonNullTargets = targets
            .Where(x => x is not null)
            .Cast<GenerationTarget>()
            .ToArray();

        if (nonNullTargets.Length == 0)
        {
            Hanz.Logger.Log("No non-null targets found.");
            return;
        }

        var methods = nonNullTargets
            .OfType<MethodDeclarationGenerationTarget>()
            .ToArray();

        if (methods.Length == 0)
        {
            Hanz.Logger.Log("No methods found.");
            return;
        }

        foreach (var target in targets.OfType<InvocationGenerationTarget>())
        {
            var targetMethod =
                target.TargetMethod is not null
                    ? methods.FirstOrDefault(x =>
                        x.Method.MethodSymbol.Equals(target.TargetMethod, SymbolEqualityComparer.Default) ||
                        (
                            target.TargetMethod.ReducedFrom is not null &&
                            x.Method.MethodSymbol.Equals(target.TargetMethod.ReducedFrom,
                                SymbolEqualityComparer.Default)
                        )
                    )
                    : methods.FirstOrDefault(x =>
                        IsSpeculativeInvocationTarget(x.Method, target.InvocationExpression, target.SemanticModel)
                    );

            if (targetMethod?.Method.MethodSyntax.Parent is not TypeDeclarationSyntax methodContainingTypeSyntax)
            {
                // Hanz.Logger.Log(
                //     $"Failed to resolve method declaration for {target.InvocationExpression.Expression}\n" +
                //     $"Methods:\n- {string.Join("\n - ", methods.Select(x => x.Method.MethodSymbol.ToDisplayString()))}\n" +
                //     $"Target method?: {target.TargetMethod?.ToDisplayString() ?? "null"}\n" +
                //     $"Target reduced from?: {target.TargetMethod?.ReducedFrom?.ToDisplayString() ?? "null"}");
                continue;
            }

            //Hanz.Logger.Log($"Successfully processing target {targetMethod.Method.MethodSymbol.ToDisplayString()}");

            var containingTypeName = targetMethod.Method.MethodSymbol.ContainingType.ToDisplayString();
            var methodName = targetMethod.Method.MethodSymbol.ToDisplayString();

            if (!processed.TryGetValue(containingTypeName, out var type))
                type = (
                    SyntaxFactory.TypeDeclaration(
                        methodContainingTypeSyntax.Kind(),
                        [],
                        methodContainingTypeSyntax.Modifiers,
                        methodContainingTypeSyntax.Keyword,
                        methodContainingTypeSyntax.Identifier,
                        methodContainingTypeSyntax.TypeParameterList,
                        null,
                        methodContainingTypeSyntax.ConstraintClauses,
                        SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                        [],
                        SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                        default
                    ),
                    new(),
                    targetMethod.Method.MethodSyntax.GetFormattedUsingDirectives(),
                    targetMethod.Method.MethodSymbol.ContainingNamespace.ToString()
                );

            var newFunctionSyntax = targetMethod.Method.MethodSyntax;
            var typeSyntax = type.Syntax;

            if (VariableFuncArgs.IsTargetMethod(targetMethod.Method.MethodSymbol))
            {
                VariableFuncArgs.Apply(
                    ref newFunctionSyntax,
                    target.InvocationExpression,
                    targetMethod.Method
                );
            }

            if (TransitiveFill.IsTargetMethod(targetMethod.Method.MethodSymbol))
            {
                TransitiveFill.Apply(
                    ref newFunctionSyntax,
                    target.InvocationExpression,
                    targetMethod.Method,
                    target.SemanticModel
                );
            }

            if (newFunctionSyntax.IsEquivalentTo(targetMethod.Method.MethodSyntax) &&
                typeSyntax.IsEquivalentTo(type.Syntax))
            {
                Hanz.Logger.Warn(
                    $"No structural changes found for {targetMethod.Method.MethodSymbol.ToDisplayString()}");
                continue;
            }

            if(!type.Methods.Add(newFunctionSyntax.ToString()))
                continue;

            processed[containingTypeName] = (
                typeSyntax.AddMembers(newFunctionSyntax),
                type.Methods,
                type.Usings,
                type.Namespace
            );
        }

        foreach (var toGenerate in processed)
        {
            context.AddSource(
                $"GenerativeFunctions/{toGenerate.Key}",
                $$"""
                  {{toGenerate.Value.Usings}}

                  namespace {{toGenerate.Value.Namespace}};

                  {{toGenerate.Value.Syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}
