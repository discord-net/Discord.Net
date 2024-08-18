using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Traits;

public sealed class GenericTraits : IGenerationCombineTask<GenericTraits.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel model,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        HashSet<INamedTypeSymbol> traits
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel Model { get; } = model;
        public TypeDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public HashSet<INamedTypeSymbol> Traits { get; } = traits;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Symbol.Equals(other.Symbol, SymbolEqualityComparer.Default) &&
                   Traits.SequenceEqual(other.Traits, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (SymbolEqualityComparer.Default.GetHashCode(Symbol) * 397) ^ Traits.GetHashCode();
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not TypeDeclarationSyntax syntax) return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax) is not INamedTypeSymbol symbol)
            return null;

        //if (IsTrait(symbol)) return null;

        var traits = GetAllTraits(symbol);

        if (traits.Count == 0) return null;

        return new GenerationTarget(
            context.SemanticModel,
            syntax,
            symbol,
            traits
        );
    }

    private static HashSet<INamedTypeSymbol> GetAllTraits(INamedTypeSymbol symbol)
    {
        return new HashSet<INamedTypeSymbol>(
            symbol.AllInterfaces
                .Where(IsTrait)
                .Where(x => x
                    .ContainingAssembly
                    .Equals(
                        symbol.ContainingAssembly,
                        SymbolEqualityComparer.Default
                    )
                ),
            SymbolEqualityComparer.Default
        );
    }

    private static bool IsTrait(ITypeSymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.TraitAttribute");
    }

    private static bool IsActor(ITypeSymbol symbol)
        => symbol.ToDisplayString().StartsWith("Discord.IActor");

    public void Execute(
        SourceProductionContext context,
        ImmutableArray<GenerationTarget?> targets,
        Logger logger)
    {
        if (targets.Length == 0) return;

        var generationTargets = targets
            .OfType<GenerationTarget>()
            .ToArray();

        foreach
        (
            var trait in targets
                .OfType<GenerationTarget>()
                .SelectMany(x => x.Traits.Select(y => (Trait: y, Target: x)))
                .GroupBy(
                    x => x.Trait,
                    x => x.Target,
                    (IEqualityComparer<INamedTypeSymbol>)SymbolEqualityComparer.Default
                )
        )
        {
            var implementers = trait.ToArray();
            if (implementers.Length == 0) continue;

            var targetLogger = logger.WithSemanticContext(implementers[0].Model);

            if (
                trait.Key.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                is not TypeDeclarationSyntax traitSyntax
            ) continue;

            if (!traitSyntax.Modifiers.Any(x => x.Kind() is SyntaxKind.PartialKeyword))
            {
                targetLogger.Warn($"{trait.Key} is not partial, skipping");
                continue;
            }

            var commonInterfaces = implementers
                .Select(x => Hierarchy.GetHierarchy(x.Symbol, false).Select(x => x.Type))
                .Aggregate((a, b) =>
                    a.Intersect<INamedTypeSymbol>(b, SymbolEqualityComparer.Default)
                )
                .Where(x =>
                    !IsTrait(x) &&
                    x.AllInterfaces.Any(IsActor) &&
                    x.Name is not "IActorTrait"
                )
                .Select(x =>
                    (
                        Common: x,
                        Distance: implementers
                            .Select(y => Hierarchy
                                .GetHierarchy(y.Symbol, false)
                                .Where(z => z
                                    .Type
                                    .Equals(x, SymbolEqualityComparer.Default)
                                )
                                .Average(x => x.Distance)
                            )
                            .Average()
                    )
                )
                .OrderBy(x => x.Distance)
                .ToArray();

            targetLogger.Log($"{trait.Key} has {implementers.Length} implementers:");

            foreach (var implementer in implementers)
            {
                targetLogger.Log($" - implementer {implementer.Symbol}");
            }

            targetLogger.Log($"{trait.Key} has {commonInterfaces.Length} common interfaces:");

            foreach (var commonInterface in commonInterfaces)
            {
                targetLogger.Log($" - common {commonInterface.Common}: {Math.Round(commonInterface.Distance, 2)}");
            }

            if (commonInterfaces.Length == 0) continue;

            SyntaxToken[] extraModifiers = trait.Key
                .AllInterfaces
                .Any(x => generationTargets
                    .Any(y => y
                        .Traits
                        .Contains(x, SymbolEqualityComparer.Default)
                    )
                )
                ? [SyntaxFactory.Token(SyntaxKind.NewKeyword)]
                : [];
            
            var clone = SyntaxUtils.CreateSourceGenClone(traitSyntax)
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"internal static Type TraitRoot => typeof({commonInterfaces[0].Common.ToDisplayString()});"
                    )!.AddModifiers(extraModifiers),
                    SyntaxFactory.ParseMemberDeclaration(
                        $"internal static Type TraitRootModel => typeof({GetModel(commonInterfaces[0].Common)!.ToDisplayString()});"
                    )!.AddModifiers(extraModifiers),
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          private static readonly HashSet<Type> _implementers = new()
                          {
                              {{
                                  string.Join(
                                      ", ",
                                      implementers.Select(x =>
                                          $"typeof({x.Symbol.ToDisplayString()})"
                                      )
                                  )
                              }}
                          };
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          private static readonly HashSet<Type> _implementerModels = new()
                          {
                            {{
                                string.Join(
                                    ", ",
                                    implementers
                                        .Select(x => GetModel(x.Symbol))
                                        .Where(x => x is not null)
                                        .Distinct(SymbolEqualityComparer.Default)
                                        .Select(x =>
                                            $"typeof({x!.ToDisplayString()})"
                                        )
                                )
                            }}
                          };
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          internal static bool ImplementsTrait(Type type)
                              => _implementers.Contains(type) || _implementers.Any(x => x.IsAssignableFrom(type));
                          """
                    )!.AddModifiers(extraModifiers),
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          internal static bool ImplementsTraitByModel(Type type)
                              => _implementerModels.Contains(type) || _implementerModels.Any(x => x.IsAssignableFrom(type));
                          """
                    )!.AddModifiers(extraModifiers)
                );

            context.AddSource(
                $"GeneratedTraits/{trait.Key.ToFullMetadataName()}",
                $$"""
                  {{traitSyntax.GetFormattedUsingDirectives()}}

                  namespace {{trait.Key.ContainingNamespace}};

                  {{clone.NormalizeWhitespace()}}
                  """
            );
        }
    }

    internal static ITypeSymbol? GetModel(ITypeSymbol symbol)
    {
        var hierarchy = Hierarchy.GetHierarchy(symbol);

        var entityOfTYpe = hierarchy.FirstOrDefault(x =>
            x.Type is {Name: "IEntityOf", IsGenericType: true}
        ).Type;

        if (entityOfTYpe is not null)
            return entityOfTYpe.TypeArguments[0];

        var actor = hierarchy.FirstOrDefault(x =>
            x.Type is {Name: "IActor", TypeArguments.Length: 2}
        ).Type;

        if (actor is not null)
        {
            return Hierarchy.GetHierarchy(actor.TypeArguments[1]).FirstOrDefault(x =>
                x.Type is {Name: "IEntityOf", IsGenericType: true}
            ).Type?.TypeArguments[0];
        }

        return null;
    }
}
