using Discord.Net.Hanz.Tasks.Actors.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.TraitsV2;

using TraitMapping = (TraitsTask.TraitSymbols Symbols, ImmutableEquatableArray<string> Ancestors);

public record TraitAncestor(
    ITraitInfo Info,
    bool? IsEntityAssignable
);

public sealed class TraitsTask : GenerationTask
{
    public IncrementalKeyValueProvider<string, TraitSymbols> Traits { get; }
    
    public IncrementalKeyValueProvider<string, ITraitInfo> TraitInfos { get; }
    public IncrementalKeyValueProvider<ITraitInfo, ImmutableEquatableArray<TraitAncestor>> TraitAncestors { get; }

    public record TraitSymbols(
        AssemblyTarget Assembly,
        INamedTypeSymbol Trait,
        ITypeSymbol Id
    )
    {
        public virtual ITraitInfo ToInfo()
            => new TraitInfo(Assembly, new(Trait), new(Id));
    }

    public sealed record ActorTraitSymbols(
        AssemblyTarget Assembly,
        INamedTypeSymbol Trait,
        ITypeSymbol Id,
        INamedTypeSymbol Entity,
        INamedTypeSymbol Model
    ) : TraitSymbols(Assembly, Trait, Id)
    {
        public override ITraitInfo ToInfo()
            => new ActorTraitInfo(Assembly, new(Trait), new(Id), new(Entity), new(Model));
    }

    public TraitsTask(
        IncrementalGeneratorInitializationContext context,
        ILogger logger
    ) : base(context, logger)
    {
        var traitsProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Discord.TraitAttribute",
                (node, _) => node is TypeDeclarationSyntax,
                Map
            )
            .WhereNotNull()
            .KeyedBy(x => x.Symbols.Trait.ToDisplayString());

        Traits = traitsProvider.MapValues((_, x) => x.Symbols);

        TraitInfos = traitsProvider
            .MapValues((_, x) => x.Symbols.ToInfo());

        TraitAncestors = traitsProvider
            .MapValues((_, mapping) =>
                mapping.Ancestors
                    .Where(traitsProvider.ContainsKey)
                    .Select(traitsProvider.GetValue)
                    .Select(x => (
                            Ancestor: x.Symbols.Trait.ToDisplayString(),
                            IsEntityAssignable:
                            mapping.Symbols is ActorTraitSymbols a &&
                            x.Symbols is ActorTraitSymbols b
                                ? a.Entity.Equals(b.Entity, SymbolEqualityComparer.Default) ||
                                  Hierarchy.Implements(a.Entity, b.Entity)
                                : (bool?) null
                        )
                    )
                    .ToImmutableEquatableArray()
            )
            .TransformKeyVia(TraitInfos)
            .MapValues((_, ancestors) => ancestors
                .Where(x => TraitInfos.ContainsKey(x.Ancestor))
                .Select(x => new TraitAncestor(TraitInfos.GetValue(x.Ancestor), x.IsEntityAssignable))
                .ToImmutableEquatableArray()
            );
    }

    private static TraitMapping? Map(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;

        if (ActorsTask.GetAssemblyTarget(context.SemanticModel.Compilation) is not { } assemblyTarget)
            return null;

        var hierarchy = Hierarchy.GetHierarchy(symbol);

        if (
            hierarchy
                .FirstOrDefault(x =>
                    x.Type is {Name: "IActorTrait", TypeArguments.Length: 2}
                ) is {Type: { } actorTrait}
        )
        {
            var entityOfInterface = Hierarchy.GetHierarchy(actorTrait.TypeArguments[1])
                .Select(x => x.Type)
                .FirstOrDefault(x => x is {Name: "IEntityOf", TypeArguments.Length: 1});

            if (entityOfInterface is null)
                return null;

            if (actorTrait.TypeArguments[1] is not INamedTypeSymbol entity)
                return null;

            if (entityOfInterface.TypeArguments[0] is not INamedTypeSymbol model)
                return null;

            return (
                new ActorTraitSymbols(
                    assemblyTarget,
                    symbol,
                    actorTrait.TypeArguments[0],
                    entity,
                    model
                ),
                symbol.AllInterfaces.Select(x => x.ToDisplayString()).ToImmutableEquatableArray()
            );
        }

        if (
            hierarchy
                .FirstOrDefault(x =>
                    x.Type is {Name: "ITrait", TypeArguments.Length: 1}
                ) is {Type: { } trait}
        )
        {
            return (
                new TraitSymbols(assemblyTarget, symbol, trait.TypeArguments[0]),
                symbol.AllInterfaces.Select(x => x.ToDisplayString()).ToImmutableEquatableArray()
            );
        }

        return null;
    }
}