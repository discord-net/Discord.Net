using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Traits;

public sealed class GenericTraits : ISyntaxGenerationCombineTask<GenericTraits.GenerationTarget>
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

    private static bool IsOverrideTrait(INamedTypeSymbol trait)
    {
        var attributeProp = trait
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == "Discord.TraitAttribute")
            ?.NamedArguments
            .FirstOrDefault(x => x.Key == "Overrides")
            .Value;

        if (attributeProp?.Value is null) return false;
        
        return (bool)attributeProp.Value.Value!;
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

        foreach (var target in generationTargets)
        {
            var targetLogger = logger.WithSemanticContext(target.Model);

            var overridableTraits = target
                .Traits
                .Where(x => target.Symbol.Interfaces.Contains(x))
                .Where(IsOverrideTrait)
                .SelectMany(IEnumerable<INamedTypeSymbol> (x) => 
                    [x, ..x.AllInterfaces.Where(IsOverrideTrait)]
                )
                .ToArray();

            if(overridableTraits.Length == 0) continue;
            
            targetLogger.Log($"{target.Symbol}: {overridableTraits.Length} overridable traits");
            
            var overrides = new List<string>();

            foreach (var trait in overridableTraits)
            {
                var traitOverrides = new List<string>();
                var overloadedMembers = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
                var overloadedBaseTraits = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
                
                targetLogger.Log($" - processing {trait}...");
                
                foreach (var baseTarget in generationTargets.Where(x => Hierarchy.Implements(target.Symbol, x.Symbol)))
                {
                    var traitComparer = trait.IsGenericType
                        ? trait.ConstructUnboundGenericType()
                        : trait;

                    foreach (var overrideTarget in baseTarget.Traits.Where(x =>
                                 (x.IsGenericType ? x.ConstructUnboundGenericType() : x).Equals(traitComparer,
                                     SymbolEqualityComparer.Default)))
                    {
                        if(!overloadedBaseTraits.Add(overrideTarget))
                            continue;

                        targetLogger.Log($"    - {overrideTarget} is base");
                        
                        foreach (var member in trait.GetMembers())
                        {
                            if (!member.IsVirtual && !member.IsAbstract)
                            {
                                targetLogger.Log($" - Skipping {member}: not implementable");
                                continue;
                            }

                            var overrideMember = overrideTarget.GetMembers().FirstOrDefault(x => x.Name == member.Name);
                           
                            targetLogger.Log($" - {member} -> {overrideMember}");

                            if (overrideMember is null )
                            {
                                targetLogger.Log($" - Skipping {member}: not overridable");
                                continue;
                            }

                            switch (overrideMember)
                            {
                                case IPropertySymbol property:
                                    traitOverrides.Add(
                                        $"{property.Type} {property.ContainingType}.{property.Name} => {property.Name};"
                                    );
                                    overloadedMembers.Add(member);
                                    break;
                                case IMethodSymbol method when method.MethodKind is MethodKind.Ordinary:
                                    var args = method.Parameters
                                        .ToDictionary(x => x.Name, x => x.Type);
                                    traitOverrides.Add(
                                        $"{method.ReturnType} {method.ContainingType}.{method.Name}({
                                            string.Join(", ", args.Select(x => $"{x.Value} {x.Key}"))
                                        }) => {method.Name}({string.Join(", ", args.Keys)});"
                                    );
                                    overloadedMembers.Add(member);
                                    break;
                            }
                        }
                    }
                }

                if (traitOverrides.Count == 0)
                {
                    targetLogger.Log($" - {trait}: no overloads");
                    continue;
                }
                
                overrides.AddRange(traitOverrides);

                foreach (var member in overloadedMembers)
                {
                    switch (member)
                    {
                        case IPropertySymbol property:
                            overrides.AddRange([
                                $"new {property.Type} {property.Name} {{ get; }}",
                                $"{property.Type} {property.ContainingType}.{property.Name} => {property.Name};"
                            ]);
                            break;
                        case IMethodSymbol method:
                            var args = method.Parameters
                                .ToDictionary(x => x.Name, x => x.Type);
                            overrides.AddRange([
                                $"new {method.ReturnType} {method.Name}({
                                    string.Join(", ", args.Select(x => $"{x.Value} {x.Key}"))
                                });",
                                $"{method.ReturnType} {method.ContainingType}.{method.Name}({
                                    string.Join(", ", args.Select(x => $"{x.Value} {x.Key}"))
                                }) => {method.Name}({string.Join(", ", args.Keys)});"
                            ]);
                            break;
                    }
                }
            }

            if (overrides.Count == 0) continue;
            
            context.AddSource(
                $"TraitOverrides/{target.Symbol.ToFullMetadataName()}",
                $$"""
                {{target.Syntax.GetFormattedUsingDirectives()}}
                
                namespace {{target.Symbol.ContainingNamespace}};
                
                public partial {{target.Symbol.TypeKind.ToString().ToLower()}} {{target.Symbol.Name}}{{(
                    target.Symbol.TypeParameters.Length > 0
                        ? $"<{string.Join(", ", target.Symbol.TypeParameters)}>"
                        : string.Empty
                )}}
                {
                    {{string.Join(Environment.NewLine, overrides).WithNewlinePadding(4)}}
                }
                """
            );
        }

        var grouping = targets
            .OfType<GenerationTarget>()
            .SelectMany(x => x.Traits.Select(y => (Trait: y, Target: x)))
            .GroupBy(
                x => x.Trait,
                x => x.Target,
                (IEqualityComparer<INamedTypeSymbol>) SymbolEqualityComparer.Default
            )
            .Select(grouping =>
                (
                    grouping.Key,
                    Implementers: grouping.ToArray(),
                    CommonInterfaces: grouping
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
                                Distance: grouping
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
                        .ToArray()
                )
            )
            .Where(x => x.Implementers.Length > 0 && x.CommonInterfaces.Length > 0)
            .ToArray();

        var generatedTraits = new HashSet<string>();

        foreach
        (
            var trait in grouping
        )
        {
            var targetLogger = logger.WithSemanticContext(trait.Implementers[0].Model);

            if (
                trait.Key.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                is not TypeDeclarationSyntax traitSyntax
            ) continue;

            if (!traitSyntax.Modifiers.Any(x => x.Kind() is SyntaxKind.PartialKeyword))
            {
                targetLogger.Warn($"{trait.Key} is not partial, skipping");
                continue;
            }

            targetLogger.Log($"{trait.Key} has {trait.Implementers.Length} implementers:");

            foreach (var implementer in trait.Implementers)
            {
                targetLogger.Log($" - implementer {implementer.Symbol}");
            }

            targetLogger.Log($"{trait.Key} has {trait.CommonInterfaces.Length} common interfaces:");

            foreach (var commonInterface in trait.CommonInterfaces)
            {
                targetLogger.Log($" - common {commonInterface.Common}: {Math.Round(commonInterface.Distance, 2)}");
            }

            SyntaxToken[] extraModifiers = trait.Key
                .AllInterfaces
                .Any(x => grouping
                    .Any(y =>
                        y.Key.Equals(x, SymbolEqualityComparer.Default)
                    )
                )
                ? [SyntaxFactory.Token(SyntaxKind.NewKeyword)]
                : [];

            var clone = SyntaxUtils.CreateSourceGenClone(traitSyntax)
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"internal static Type TraitRoot => typeof({trait.CommonInterfaces[0].Common.ToDisplayString()});"
                    )!.AddModifiers(extraModifiers),
                    SyntaxFactory.ParseMemberDeclaration(
                        $"internal static Type TraitRootModel => typeof({GetModel(trait.CommonInterfaces[0].Common)!.ToDisplayString()});"
                    )!.AddModifiers(extraModifiers),
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          private static readonly HashSet<Type> _implementers = new()
                          {
                              {{
                                  string.Join(
                                      ", ",
                                      trait.Implementers.Select(x =>
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
                                    trait.Implementers
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

            if (!generatedTraits.Add(trait.Key.ToFullMetadataName()))
            {
                logger.Warn($"Failed to generate {trait.Key}, already added to sources");
                continue;
            }

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