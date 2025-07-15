using Discord.Net.Hanz.Tasks.Actors.Common;
using Discord.Net.Hanz.Tasks.Actors.TraitsV2.Nodes.Fetchable;
using Discord.Net.Hanz.Tasks.ApiRoutes;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.TraitsV2.Nodes.Loadable;

public sealed class LoadableTraitNode : TraitNode
{
    private record Context(
        string Target,
        TypeRef Route,
        ImmutableEquatableArray<TypeRef> RouteGenerics
    );

    private record State(
        ImmutableEquatableArray<TraitTargetAncestor> Ancestors,
        TypeRef Route,
        ImmutableEquatableArray<TypeRef> RouteGenerics,
        bool ImplementsFetchable
    );

    private readonly IncrementalKeyValueProvider<TraitImplementationTarget, State> _stateProvider;

    public LoadableTraitNode(IncrementalGeneratorInitializationContext context, ILogger logger) : base(context, logger)
    {
        _stateProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Discord.LoadableAttribute`1",
                (node, _) => node is InterfaceDeclarationSyntax,
                Map
            )
            .WhereNotNull()
            .WithLogging(Logger)
            .DependsOn(PathingInfoProvider)
            .KeyedBy(x => x.Target)
            .PairKeys(TargetsProvider)
            .JoinByKey(TargetAncestorsProvider!)
            .JoinByKey(
                GetNode<FetchableTraitNode>().FetchableProvider,
                (info, context, fetchableDetails) => (
                    Hierarchy: context.Other,
                    Context: context.Value,
                    HasSimpleFetchable: fetchableDetails.Any(x => x.Kind is FetchableTraitNode.Kind.Fetchable)
                ),
                includeEmpty: true
            )
            .MaybeMapValues((info, details) =>
            {
                if (details.Hierarchy is null) return default;

                return new State(
                    details.Hierarchy,
                    details.Context.Route,
                    details.Context.RouteGenerics,
                    details.HasSimpleFetchable
                ).Some();
            });

        context.RegisterSourceOutput(
            _stateProvider
                .MapValues(CreateSpec)
                .Select((target, spec) =>
                    new SourceSpec(
                        $"Loadable/{target.Type.MetadataName}",
                        target.Type.Namespace!,
                        new(["Discord", "Discord.Models", "Discord.Rest"]),
                        new([spec])
                    )
                )
        );
    }

    private static string GetLoadableInterface(TraitImplementationTarget target, State state)
        => $"Discord.ILoadable<{state.Route}, {target.Entity}, {target.Model}>";

    private TypeSpec CreateSpec(TraitImplementationTarget target, State state)
    {
        var loadableInterface = GetLoadableInterface(target, state);

        var spec = TypeSpec
            .From(target.Type)
            .AddModifiers("partial")
            .AddBases(loadableInterface);

        // if (!state.ImplementsFetchable)
        //     GetNode<FetchableTraitNode>().ImplementSimpleFetchable(
        //         target,
        //         new(FetchableTraitNode.Kind.Fetchable, state.Route),
        //         ref spec
        //     );

        if (!RedefinesLoadableMembers(state))
            return spec;

        // spec = spec
        //     .AddMethods(
        //         new MethodSpec(
        //             "GetOrFetchAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Modifiers: new(["new"]),
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"(this as {loadableInterface}).GetOrFetchAsync(options, token)"
        //         ),
        //         new MethodSpec(
        //             "GetAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Modifiers: new(["new"]),
        //             Parameters: new([
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"default"
        //         ),
        //         new MethodSpec(
        //             "FetchAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Modifiers: new(["new"]),
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"(this as {loadableInterface}).FetchAsync(options, token)"
        //         ),
        //         new MethodSpec(
        //             "GetOrFetchAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"GetOrFetchAsync(options, token)",
        //             ExplicitInterfaceImplementation: loadableInterface
        //         ),
        //         new MethodSpec(
        //             "GetAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Parameters: new([
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"GetAsync(token)",
        //             ExplicitInterfaceImplementation: loadableInterface
        //         ),
        //         new MethodSpec(
        //             "FetchAsync",
        //             $"ValueTask<{target.Entity}?>",
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"FetchAsync(options, token)",
        //             ExplicitInterfaceImplementation: loadableInterface
        //         )
        //     );
        //
        // foreach (var ancestor in state.Ancestors.Where(x =>
        //              x.IsEntityAssignable is not false && _stateProvider.ContainsKey(x.Target)))
        // {
        //     var ancestorState = _stateProvider.GetValue(ancestor.Target);
        //
        //     var ancestorOverloadTarget = RedefinesLoadableMembers(ancestorState)
        //         ? ancestor.Target.Type.DisplayString
        //         : GetLoadableInterface(ancestor.Target);
        //
        //     spec = spec.AddMethods(
        //         new MethodSpec(
        //             "GetOrFetchAsync",
        //             $"ValueTask<{ancestor.Target.Entity}?>",
        //             Modifiers: new(["async"]),
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"await GetOrFetchAsync(options, token)",
        //             ExplicitInterfaceImplementation: ancestorOverloadTarget
        //         ),
        //         new MethodSpec(
        //             "GetAsync",
        //             $"ValueTask<{ancestor.Target.Entity}?>",
        //             Modifiers: new(["async"]),
        //             Parameters: new([
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"await GetAsync(token)",
        //             ExplicitInterfaceImplementation: ancestorOverloadTarget
        //         ),
        //         new MethodSpec(
        //             "FetchAsync",
        //             $"ValueTask<{ancestor.Target.Entity}?>",
        //             Modifiers: new(["async"]),
        //             Parameters: new([
        //                 ("RequestOptions?", "options", "null"),
        //                 ("CancellationToken", "token", "default"),
        //             ]),
        //             Expression: $"await FetchAsync(options, token)",
        //             ExplicitInterfaceImplementation: ancestorOverloadTarget
        //         )
        //     );
        // }

        return spec;

        bool RedefinesLoadableMembers(State state)
            => state.Ancestors.Any(x => _stateProvider.ContainsKey(x.Target));
    }

    private Context? Map(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetNode is not InterfaceDeclarationSyntax syntax)
        {
            Logger.Log($"{context.TargetSymbol}: not interface");
            return null;
        }

        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            Logger.Log($"{context.TargetSymbol}: not partial");
            return null;
        }

        if (context.Attributes.Length != 1)
        {
            Logger.Log($"{context.TargetSymbol}: not 1 attribute");
            return null;
        }

        var attribute = context.Attributes[0];

        if (attribute.AttributeClass is not {TypeArguments.Length: 1})
        {
            Logger.Log($"{context.TargetSymbol}: not generic, {attribute.AttributeClass}");
            return null;
        }

        var route = new TypeRef(attribute.AttributeClass.TypeArguments[0]);

        var routeGenerics = attribute.ConstructorArguments.Length > 1
            ? attribute.ConstructorArguments[1].Kind is TypedConstantKind.Array
                ? attribute.ConstructorArguments[1].Values.Select(x => new TypeRef((x.Value as ITypeSymbol)!))
                    .ToImmutableEquatableArray()
                : ImmutableEquatableArray<TypeRef>.Empty
            : ImmutableEquatableArray<TypeRef>.Empty;

        Logger.Log($"{context.TargetSymbol}: Mapped {route}");
        
        return new(context.TargetSymbol.ToDisplayString(), route, routeGenerics);
    }
}