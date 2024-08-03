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
        IEnumerable<IMethodSymbol> candidateMethods,
        SemanticModel semanticModel
    ) : GenerationTarget(semanticModel), IEquatable<InvocationGenerationTarget>
    {
        public InvocationExpressionSyntax InvocationExpression { get; } = invocationExpression;
        public IEnumerable<IMethodSymbol> TargetMethods { get; } = candidateMethods;

        public bool Equals(InvocationGenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                InvocationExpression.IsEquivalentTo(other.InvocationExpression) &&
                TargetMethods.SequenceEqual(other.TargetMethods, SymbolEqualityComparer.Default);
        }

        public override bool Equals(GenerationTarget other) =>
            other is InvocationGenerationTarget otherThis && Equals(otherThis);

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is InvocationGenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (InvocationExpression.GetHashCode() * 397) ^ TargetMethods.GetHashCode();
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

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
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
                    return new InvocationGenerationTarget(invocation, [target], context.SemanticModel);
                }

                var candidates =
                    symbolInfo.CandidateSymbols.Where(x =>
                        x is IMethodSymbol target && IsTargetMethod(target));

                return new InvocationGenerationTarget(invocation, candidates.OfType<IMethodSymbol>(),
                    context.SemanticModel);
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

    private static bool IsSpeculativeInvocationTarget(
        MethodTarget method,
        InvocationExpressionSyntax invocation,
        SemanticModel semantic)
    {
        var target = GetInvocationTarget(invocation, semantic);

        if (target is null)
            return false;

        if (!method.CanBeCalledFor(target, semantic))
            return false;

        return invocation.Expression.ToString().EndsWith(method.MethodSymbol.Name);
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        var processed =
            new Dictionary<
                string,
                (TypeDeclarationSyntax Syntax, HashSet<string> Methods, IEnumerable<string> Usings, string Namespace)
            >();

        var nonNullTargets = targets
            .Where(x => x is not null)
            .Cast<GenerationTarget>()
            .ToArray();

        if (nonNullTargets.Length == 0)
        {
            logger.Log("No non-null targets found.");
            return;
        }

        var methods = nonNullTargets
            .OfType<MethodDeclarationGenerationTarget>()
            .ToArray();

        if (methods.Length == 0)
        {
            logger.Log("No methods found.");
            return;
        }

        foreach (var methodTarget in methods)
        {
            var methodLogger = logger.WithSemanticContext(methodTarget.SemanticModel);

            methodLogger.Log($"Candidate method: {methodTarget.Method.MethodSymbol}");
        }

        foreach (var target in targets.OfType<InvocationGenerationTarget>())
        {
            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            var targetMethods = methods
                .Where(x =>
                    target.TargetMethods.Contains(x.Method.MethodSymbol, SymbolEqualityComparer.Default))
                .ToArray();

            if (targetMethods.Length == 0)
            {
                targetMethods = methods
                    .Where(x =>
                        IsSpeculativeInvocationTarget(x.Method, target.InvocationExpression, target.SemanticModel)
                    )
                    .ToArray();
            }

            if(targetMethods.Length == 0)
                continue;

            targetLogger.Log(
                $"Found {targetMethods.Length} candidate methods for {target.InvocationExpression.NormalizeWhitespace()}"
            );

            foreach (var method in targetMethods)
            {
                targetLogger.Log($" - {method.Method.MethodSymbol}");
            }

            foreach (var targetMethod in targetMethods)
            {
                if (targetMethod?.Method.MethodSyntax.Parent is not TypeDeclarationSyntax methodContainingTypeSyntax)
                {
                    continue;
                }

                var containingTypeName = targetMethod.Method.MethodSymbol.ContainingType.ToDisplayString();

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
                        targetMethod.Method.MethodSyntax.GetUsingDirectives(),
                        targetMethod.Method.MethodSymbol.ContainingNamespace.ToString()
                    );
                else
                {
                    type.Usings = type.Usings.Concat(targetMethod.Method.MethodSyntax.GetUsingDirectives()).Distinct();
                }

                var newFunctionSyntax = targetMethod.Method.MethodSyntax;
                var typeSyntax = type.Syntax;

                if (VariableFuncArgs.IsTargetMethod(targetMethod.Method.MethodSymbol))
                {
                    VariableFuncArgs.Apply(
                        ref newFunctionSyntax,
                        target.InvocationExpression,
                        targetMethod.Method,
                        target.SemanticModel,
                        targetLogger.GetSubLogger("VariableFuncArgs")
                    );
                }

                if (TransitiveFill.IsTargetMethod(targetMethod.Method.MethodSymbol))
                {
                    TransitiveFill.Apply(
                        ref newFunctionSyntax,
                        target.InvocationExpression,
                        targetMethod.Method,
                        target.SemanticModel,
                        targetLogger.GetSubLogger("TransitiveFill")
                    );
                }

                if (newFunctionSyntax.IsEquivalentTo(targetMethod.Method.MethodSyntax) &&
                    typeSyntax.IsEquivalentTo(type.Syntax))
                {
                    targetLogger.Warn(
                        $"No structural changes found for {targetMethod.Method.MethodSymbol.ToDisplayString()}");
                    continue;
                }

                if (!type.Methods.Add(newFunctionSyntax.ToString()))
                {
                    targetLogger.Warn($"Skipping {targetMethod.Method.MethodSymbol}: Syntax equivalent exists");
                    continue;
                }

                processed[containingTypeName] = (
                    typeSyntax.AddMembers(newFunctionSyntax),
                    type.Methods,
                    type.Usings,
                    type.Namespace
                );
                break;
            }
        }

        foreach (var toGenerate in processed)
        {
            context.AddSource(
                $"GenerativeFunctions/{toGenerate.Key}",
                $$"""
                  {{string.Join("\n", toGenerate.Value.Usings)}}

                  namespace {{toGenerate.Value.Namespace}};

                  {{toGenerate.Value.Syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}
