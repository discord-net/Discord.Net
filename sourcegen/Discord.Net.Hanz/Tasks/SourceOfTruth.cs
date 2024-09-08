using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors;
using Exception = System.Exception;

namespace Discord.Net.Hanz.Tasks;

public class SourceOfTruth : ISyntaxGenerationCombineTask<SourceOfTruth.GenerationTarget>
{
    public static string[] IgnoredTypes =
    [
        "BackLink"
    ];

    public class GenerationTarget(
        SemanticModel semantic,
        TypeDeclarationSyntax typeDeclarationSyntax,
        MemberDeclarationSyntax memberDeclarationSyntax,
        ITypeSymbol typeSymbol
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel Semantic { get; } = semantic;
        public TypeDeclarationSyntax TypeDeclarationSyntax { get; } = typeDeclarationSyntax;
        public MemberDeclarationSyntax MemberDeclarationSyntax { get; } = memberDeclarationSyntax;
        public ITypeSymbol TypeSymbol { get; } = typeSymbol;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TypeDeclarationSyntax.IsEquivalentTo(other.TypeDeclarationSyntax) &&
                   MemberDeclarationSyntax.IsEquivalentTo(other.MemberDeclarationSyntax) &&
                   TypeSymbol.Equals(other.TypeSymbol, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GenerationTarget) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = TypeDeclarationSyntax.GetHashCode();
                hashcode = (hashcode * 397) ^ (MemberDeclarationSyntax?.GetHashCode() ?? 0);
                return (hashcode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(TypeSymbol);
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        return node switch
        {
            MemberDeclarationSyntax member => member.AttributeLists.Count > 0,
            _ => false
        };
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
    {
        if (context.Node is not MemberDeclarationSyntax member) return null;
        if (member.Parent is not TypeDeclarationSyntax type) return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(member, token);
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type, token);

        if (symbol is null || !IsTarget(symbol) || typeSymbol is null)
            return null;

        return new GenerationTarget(context.SemanticModel, type, member, typeSymbol);
    }

    public static bool IsTarget(ISymbol symbol)
    {
        return symbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.SourceOfTruthAttribute");
    }

    public static ISymbol? GetSourceOfTruthMember(ITypeSymbol type, ISymbol member)
    {
        return type
            .GetMembers()
            .FirstOrDefault(x =>
                IsTarget(x) &&
                MemberUtils.GetMemberName(x) == MemberUtils.GetMemberName(member) &&
                x.Kind == member.Kind
            );
    }

    private class GenerationResult(
        string ns,
        string usingDirectives,
        TypeDeclarationSyntax syntax,
        ITypeSymbol typeSymbol)
    {
        public ITypeSymbol TypeSymbol { get; } = typeSymbol;
        public string Namespace { get; } = ns;
        public string UsingDirectives { get; } = usingDirectives;
        public TypeDeclarationSyntax Syntax { get; set; } = syntax;
    }

    private static bool ShouldIgnoreChecks(ITypeSymbol sourceType)
    {
        return
            IgnoredTypes.Contains(sourceType.Name)
            ||
            sourceType.Name.EndsWith("Link")
            ||
            sourceType.Name is "Indexable" or "Enumerable" or "Defined" or "Paged";
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        var toGenerate = new Dictionary<string, GenerationResult>();
        var addedMembers = new Dictionary<ITypeSymbol, HashSet<ISymbol>>(SymbolEqualityComparer.Default);

        foreach (var target in targets)
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.Semantic);

            var sourceOfTruthType = (target.MemberDeclarationSyntax switch
            {
                MethodDeclarationSyntax method => ModelExtensions.GetTypeInfo(target.Semantic, method.ReturnType),
                PropertyDeclarationSyntax property => ModelExtensions.GetTypeInfo(target.Semantic, property.Type),
                _ => (TypeInfo?) null
            })?.Type;

            if (sourceOfTruthType is null) continue;

            var sourceOfTruthName = target.MemberDeclarationSyntax switch
            {
                MethodDeclarationSyntax method => method.Identifier.ValueText,
                PropertyDeclarationSyntax property => property.Identifier.ValueText,
                _ => null
            };

            if (sourceOfTruthName is null) continue;

            if (target.TypeSymbol is not INamedTypeSymbol typeSymbol) continue;

            HashSet<ISymbol> validTargets = new(SymbolEqualityComparer.Default);

            var proxied = target.TypeDeclarationSyntax is ClassDeclarationSyntax cls
                ? TypeUtils.GetBaseTypesAndThis(typeSymbol)
                    .SelectMany(InterfaceProxy.GetProxiedInterfaceMembers)
                    .ToImmutableHashSet(SymbolEqualityComparer.Default)
                : ImmutableHashSet<ISymbol>.Empty;

            if (proxied.Count > 0)
            {
                targetLogger.Log($"{target.TypeSymbol}: Proxied members:");
                foreach (var item in proxied)
                {
                    targetLogger.Log($" - {item}");
                }
            }

            var interfaces = typeSymbol.AllInterfaces
                .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

            foreach (var baseSymbol in TypeUtils.GetBaseTypes(typeSymbol))
                interfaces = interfaces.Except(baseSymbol.AllInterfaces);

            foreach (var iface in interfaces)
            {
                var targetMembers = iface.GetMembers(sourceOfTruthName);

                if (targetMembers.Length > 0)
                {
                    targetLogger.Log(
                        $"{target.TypeSymbol}: {targetMembers.Length} candidate members for {iface} ({interfaces.Count} interfaces total)");
                }

                foreach (var member in targetMembers)
                {
                    if (proxied.Contains(member))
                    {
                        targetLogger.Log($"{target.TypeSymbol}: Skipping {member} (implemented by interface proxy)");
                        continue;
                    }

                    if (ShouldIgnoreChecks(sourceOfTruthType))
                    {
                        if (validTargets.Add(member))
                            targetLogger.Log($"{target.TypeSymbol}: adding target {member} ({iface})");

                        continue;
                    }

                    switch (member)
                    {
                        case IMethodSymbol method when target.MemberDeclarationSyntax is MethodDeclarationSyntax:
                            if (!target.Semantic.Compilation.HasImplicitConversion(
                                    sourceOfTruthType,
                                    method.ReturnType))
                            {
                                targetLogger.Warn(
                                    $"{target.TypeSymbol}: No conversion between {sourceOfTruthType.ToDisplayString()} -> {method.ReturnType.ToDisplayString()}");
                                break;
                            }

                            if (validTargets.Add(member))
                                targetLogger.Log($"{target.TypeSymbol}: adding target {member} ({iface})");
                            break;
                        case IPropertySymbol property when target.MemberDeclarationSyntax is PropertyDeclarationSyntax:
                            if (!target.Semantic.Compilation.HasImplicitConversion(sourceOfTruthType, property.Type))
                            {
                                targetLogger.Warn(
                                    $"{target.TypeSymbol}: No conversion between {sourceOfTruthType.ToDisplayString()} -> {property.Type.ToDisplayString()}");
                                break;
                            }

                            if (validTargets.Add(member))
                                targetLogger.Log($"{target.TypeSymbol}: adding target {member} ({iface})");
                            break;
                    }
                }
            }

            try
            {
                var fullTypeName = target.Semantic.Compilation
                    .GetSemanticModel(target.TypeDeclarationSyntax.SyntaxTree)
                    .GetDeclaredSymbol(target.TypeDeclarationSyntax)!
                    .ToFullMetadataName();

                if (!toGenerate.TryGetValue(fullTypeName, out var targetTypeDeclaration))
                    targetTypeDeclaration = new GenerationResult(
                        target.Semantic.GetDeclaredSymbol(target.TypeDeclarationSyntax)!.ContainingNamespace.ToString(),
                        string.Join("\n",
                            target.TypeDeclarationSyntax.SyntaxTree.GetRoot().ChildNodes()
                                .OfType<UsingDirectiveSyntax>()),
                        SyntaxFactory.TypeDeclaration(
                            target.TypeDeclarationSyntax.Kind(),
                            [],
                            target.TypeDeclarationSyntax.Modifiers,
                            target.TypeDeclarationSyntax.Keyword,
                            target.TypeDeclarationSyntax.Identifier,
                            target.TypeDeclarationSyntax.TypeParameterList,
                            null,
                            target.TypeDeclarationSyntax.ConstraintClauses,
                            SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                            [],
                            SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                            target.TypeDeclarationSyntax.SemicolonToken
                        ),
                        target.TypeSymbol
                    );

                foreach (var validTarget in validTargets)
                {
                    if (!addedMembers.TryGetValue(target.TypeSymbol, out var members))
                        addedMembers[target.TypeSymbol] = members = new(SymbolEqualityComparer.Default);

                    if (!members.Add(validTarget))
                    {
                        targetLogger.Log($"{target.TypeSymbol}: Skipping {validTarget} (signature already added)");
                        continue;
                    }

                    targetLogger.Log($"{target.TypeSymbol}: Adding {validTarget.ToDisplayString()}");

                    switch (validTarget)
                    {
                        case IPropertySymbol property:
                            targetTypeDeclaration.Syntax = targetTypeDeclaration.Syntax.AddMembers(
                                SyntaxFactory.PropertyDeclaration([], [],
                                        SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                                        SyntaxFactory.ExplicitInterfaceSpecifier(
                                            SyntaxFactory.IdentifierName(property.ContainingType.ToDisplayString())),
                                        SyntaxFactory.Identifier(property.Name),
                                        null,
                                        SyntaxFactory.ArrowExpressionClause(
                                            SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                            SyntaxFactory.IdentifierName(property.Name)
                                        ),
                                        null,
                                        SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                    )
                                    .WithLeadingTrivia(
                                        SyntaxFactory.Comment($"// {property.ToDisplayString()}")
                                    )
                            );
                            break;
                        case IMethodSymbol method:
                            targetTypeDeclaration.Syntax = targetTypeDeclaration.Syntax.AddMembers(
                                SyntaxFactory.MethodDeclaration(
                                        [],
                                        [],
                                        SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                                        SyntaxFactory.ExplicitInterfaceSpecifier(
                                            SyntaxFactory.IdentifierName(method.ContainingType.ToDisplayString())
                                        ),
                                        SyntaxFactory.Identifier(method.Name),
                                        method.TypeParameters.Length > 0
                                            ? SyntaxFactory.TypeParameterList(
                                                SyntaxFactory.SeparatedList(
                                                    method.TypeParameters.Select(x =>
                                                        SyntaxFactory.TypeParameter(x.ToDisplayString())
                                                    )
                                                )
                                            )
                                            : null,
                                        SyntaxFactory.ParameterList(
                                            SyntaxFactory.SeparatedList(
                                                method.Parameters.Select(x =>
                                                    SyntaxFactory.Parameter(
                                                        new SyntaxList<AttributeListSyntax>(),
                                                        new SyntaxTokenList(),
                                                        SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                                                        SyntaxFactory.Identifier(x.Name),
                                                        null
                                                    )
                                                )
                                            )
                                        ),
                                        new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                                        null,
                                        SyntaxFactory.ArrowExpressionClause(
                                            SyntaxFactory.InvocationExpression(
                                                SyntaxFactory.IdentifierName(method.Name),
                                                SyntaxFactory.ArgumentList(
                                                    SyntaxFactory.SeparatedList(
                                                        method.Parameters.Select(x =>
                                                            SyntaxFactory.Argument(
                                                                null,
                                                                SyntaxFactory.Token(SyntaxKind.None),
                                                                SyntaxFactory.IdentifierName(x.Name)
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        ),
                                        SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                    )
                                    .WithLeadingTrivia(
                                        SyntaxFactory.Comment($"// {method.ToDisplayString()}")
                                    )
                            );
                            break;
                    }
                }

                toGenerate[fullTypeName] = targetTypeDeclaration;
            }
            catch (Exception x)
            {
                targetLogger.Log(LogLevel.Error, x.ToString());
            }
        }

        foreach (var target in toGenerate)
        {
            var syntax = target.Value.Syntax;
            
            SyntaxUtils.ApplyNesting(target.Value.TypeSymbol, ref syntax);
            
            context.AddSource(
                $"SourceOfTruths/{target.Key}",
                $$"""
                  {{target.Value.UsingDirectives}}

                  namespace {{target.Value.Namespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}