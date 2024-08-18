using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Traits;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

public class Links : IGenerationCombineTask<Links.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol actor,
        INamedTypeSymbol entity,
        ITypeSymbol id,
        INamedTypeSymbol model,
        TypeDeclarationSyntax syntax
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel Semantic { get; } = semantic;
        public INamedTypeSymbol Actor { get; } = actor;
        public INamedTypeSymbol Entity { get; } = entity;
        public ITypeSymbol Id { get; } = id;
        public INamedTypeSymbol Model { get; } = model;
        public TypeDeclarationSyntax Syntax { get; } = syntax;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Actor.Equals(other.Actor, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            return SymbolEqualityComparer.Default.GetHashCode(Actor);
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right)
        {
            return !Equals(left, right);
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax type;
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not TypeDeclarationSyntax typeSyntax) return null;

        if (context.SemanticModel.GetDeclaredSymbol(typeSyntax, token) is not INamedTypeSymbol symbol) return null;

        if (EntityTraits.GetActorInterface(symbol) is not { } actorInterface) return null;

        // skip entities
        if (EntityTraits.GetEntityModelOfInterface(symbol) is not null) return null;
        
        if (actorInterface.TypeArguments[1] is not INamedTypeSymbol entity) return null;

        if (EntityTraits.GetEntityModelOfInterface(entity) is not { } entityOf) return null;

        if (entityOf.TypeArguments[0] is not INamedTypeSymbol model) return null;

        return new GenerationTarget(
            context.SemanticModel,
            symbol,
            entity,
            actorInterface.TypeArguments[0],
            model,
            typeSyntax
        );
    }

    private static readonly Dictionary<string, string> _lookupTable = new()
    {
        {"DefinedEnumerable", "Discord.IDefinedEnumerableLink"},
        {"DefinedIndexable", "Discord.IDefinedIndexableLink"},
        {"EnumerableIndexable", "Discord.IEnumerableIndexableLink"},
        {"Enumerable", "Discord.IEnumerableLink"},
        {"Indexable", "Discord.IIndexableLink"},
    };

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var aliases = new Dictionary<string, string>();

        foreach (var target in targets)
        {
            if (target is null) continue;

            var name = target.Actor.Name.Remove(0, 1).Replace("Actor", string.Empty);

            foreach (var entry in _lookupTable)
            {
                var key = $"{entry.Key}{name}Link";

                if (!aliases.ContainsKey(key))
                    aliases[key] =
                        $"{entry.Value}<" +
                        $"{target.Actor.ToDisplayString()}, " +
                        $"{target.Id.ToDisplayString()}, " +
                        $"{target.Entity.ToDisplayString()}, " +
                        $"{target.Model.ToDisplayString()}>";
            }

            if (FetchableTrait.IsPagedFetchableOfMany(target.Entity))
            {
                var pagedAttributes = target.Entity
                    .GetAttributes()
                    .Where(x =>
                        x.AttributeClass?
                            .ToDisplayString()
                            .StartsWith("Discord.PagedFetchableOfManyAttribute")
                        ?? false
                    )
                    .ToArray();

                foreach (var pagedAttribute in pagedAttributes)
                {
                    var pagingParams = pagedAttribute.AttributeClass!.TypeArguments[0];
                    var pagedEntity = pagedAttribute.AttributeClass.TypeArguments.Length == 2
                        ? pagedAttribute.AttributeClass.TypeArguments[1]
                        : null;

                    var pagedName = name;
                    if (pagedAttributes.Length > 1)
                    {
                        pagedName = pagedAttribute.AttributeClass.TypeArguments[0]
                            .Name
                            .Replace("Page", string.Empty)
                            .Replace("Params", string.Empty);
                    }

                    
                    var paged = $"Paged{pagedName}Link";
                    var pagedIndexableFull = $"PagedIndexable{pagedName}Link";
                    var pagedIndexablePartial = $"PartialPagedIndexable{pagedName}Link";

                    if (!aliases.ContainsKey(paged))
                        aliases[paged] = $"Discord.IPagedLink<" +
                                         $"{target.Actor.ToDisplayString()}, " +
                                         $"{target.Id.ToDisplayString()}, " +
                                         $"{target.Entity.ToDisplayString()}, " +
                                         $"{target.Model.ToDisplayString()}, " +
                                         $"{pagingParams.ToDisplayString()}" +
                                         $">";

                    if (!aliases.ContainsKey(pagedIndexableFull))
                        aliases[pagedIndexableFull] = $"Discord.IPagedIndexableLink<" +
                                                      $"{target.Actor.ToDisplayString()}, " +
                                                      $"{target.Id.ToDisplayString()}, " +
                                                      $"{target.Entity.ToDisplayString()}, " +
                                                      $"{target.Model.ToDisplayString()}, " +
                                                      $"{pagingParams.ToDisplayString()}" +
                                                      $">";

                    if (pagedEntity is not null && !aliases.ContainsKey(pagedIndexablePartial))
                        aliases[pagedIndexablePartial] = $"Discord.IPagedIndexableLink<" +
                                                         $"{target.Actor.ToDisplayString()}, " +
                                                         $"{target.Id.ToDisplayString()}, " +
                                                         $"{target.Entity.ToDisplayString()}, " +
                                                         $"{target.Model.ToDisplayString()}, " +
                                                         $"{pagedEntity.ToDisplayString()}, " +
                                                         $"{pagingParams.ToDisplayString()}" +
                                                         $">";
                }
            }
        }

        if (aliases.Count == 0) return;

        context.AddSource(
            "Links/Globals",
            string.Join(
                "\n",
                aliases.Select(x => $"global using {x.Key} = {x.Value};")
            )
        );
    }
}