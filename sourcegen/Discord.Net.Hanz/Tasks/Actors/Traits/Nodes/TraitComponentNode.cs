using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Common;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.TraitsV2.Nodes;

using TraitLinkExtension = (string Name, string Type, string? Getter);
using TraitProperty = (string Type, string Name);

public sealed class TraitComponentNode : TraitNode
{
    private record State(
        ITraitInfo Trait,
        ImmutableEquatableArray<ComponentState> Components,
        ImmutableEquatableArray<TraitProperty> Properties
    )
    {
    }

    private record ComponentState(
        TypeRef Component,
        string? Parent,
        ImmutableEquatableArray<TraitLinkExtension> Extensions
    )
    {
        public string TypeName => Component.Name.Replace("Component", string.Empty);

        public bool TryGetExtensionForProperty(string name, out TraitLinkExtension extension)
            => (extension = Extensions.FirstOrDefault(x => x.Name == name)) != default;
    }

    private record Context(
        string Trait,
        TypeRef Component,
        string? Parent,
        ImmutableEquatableArray<TraitProperty> Properties
    );

    private record LinkExtensionContext(
        string Component,
        ImmutableEquatableArray<TraitLinkExtension> Extensions
    );

    private readonly IncrementalKeyValueProvider<ITraitInfo, State> _componentsProvider;

    public TraitComponentNode(IncrementalGeneratorInitializationContext context, ILogger logger) : base(context, logger)
    {
        var componentsProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Discord.TraitComponentAttribute",
                (node, _) => node is InterfaceDeclarationSyntax,
                MapTraitComponent
            )
            .WhereNotNull();

        var linkExtensionsProvider = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Discord.TraitLinkExtendsAttribute",
                (node, _) => node is InterfaceDeclarationSyntax,
                MapLinkExtensionContext
            )
            .WhereNotNull();

        _componentsProvider = componentsProvider
            .KeyedBy(x => x.Component.DisplayString)
            .JoinByKey(
                linkExtensionsProvider.GroupBy(x => x.Component),
                (_, context, extensions) => (
                    State: new ComponentState(
                        context.Component,
                        context.Parent,
                        extensions.SelectMany(x => x.Extensions).ToImmutableEquatableArray()
                    ),
                    context.Properties,
                    context.Trait
                ),
                includeEmpty: true
            )
            .ValuesProvider
            .GroupBy(x => x.Trait)
            .TransformKeysVia(GetTask<TraitsTask>().TraitInfos)
            .ToKeyed((trait, components) =>
                new State(
                    trait,
                    components.Select(x => x.State).ToImmutableEquatableArray(),
                    components.SelectMany(x => x.Properties).Distinct().ToImmutableEquatableArray()
                )
            );

        context.RegisterSourceOutput(
            _componentsProvider.Select(CreateSourceSpec)
        );
    }

    private SourceSpec CreateSourceSpec(ITraitInfo traitInfo, State state)
        => new(
            $"TraitComponents/{traitInfo.Trait.MetadataName}",
            traitInfo.Trait.Namespace!,
            new(["Discord", "Discord.Models"]),
            new([
                TypeSpec
                    .From(traitInfo.Trait)
                    .AddModifiers("partial")
                    .AddNestedTypes(
                        state.Components.Select(x =>
                            CreateComponentSpec(
                                traitInfo,
                                state,
                                x,
                                state.Components,
                                ImmutableArray<ComponentState>.Empty
                            )
                        )
                    )
            ])
        );

    private TypeSpec CreateComponentSpec(
        ITraitInfo traitInfo,
        State state,
        ComponentState componentState,
        ImmutableEquatableArray<ComponentState> children,
        ImmutableArray<ComponentState> path
    )
    {
        var baseType = string.Join(".", [
            traitInfo.Trait.ReferenceName,
            ..path.Select(x => x.TypeName)
        ]);

        var spec = new TypeSpec(
            componentState.TypeName,
            TypeKind.Interface,
            Bases: new([
                baseType
            ])
        );

        foreach (var traitLinkExtension in componentState.Extensions)
        {
            ApplyTraitExtension(ref spec, traitInfo, state, componentState, traitLinkExtension, path, baseType);
        }

        children = children.Remove(componentState);
        path = path.Add(componentState);

        return spec.AddNestedTypes(
            children.Select(x => CreateComponentSpec(traitInfo, state, x, children, path))
        );
    }

    private void ApplyTraitExtension(
        ref TypeSpec spec,
        ITraitInfo traitInfo,
        State state,
        ComponentState component,
        TraitLinkExtension extension,
        ImmutableArray<ComponentState> path,
        string baseType
    )
    {
        var targetProperty = state
            .Properties
            .FirstOrDefault(x => x.Name == extension.Name);

        if (targetProperty == default)
            return;

        var parts = new List<string>() {targetProperty.Type};

        foreach (var parentComponent in path)
        {
            if (!parentComponent.TryGetExtensionForProperty(extension.Name, out var parentExtension))
                continue;

            parts.Add(string.Join(".", parentExtension.Type.Split('.').Last()));
        }

        var basePropertyType = string.Join(".", parts);
        var propertyType =
            $"{basePropertyType}.{string.Join(".", extension.Type.Split('.').Last())}";

        if (path.Length > 0)
            spec = spec.AddBases(
                path
                    .Select((_, i) =>
                        string.Join(
                            ".",
                            path
                                .Take(i)
                                .Select(x => x.TypeName)
                                .Prepend(traitInfo.Trait.ReferenceName)
                                .Append(component.TypeName)
                        )
                    )
            );


        spec = spec
            .AddProperties([
                new PropertySpec(
                    propertyType,
                    targetProperty.Name,
                    Modifiers: new(["new"])
                ),
                new PropertySpec(
                    basePropertyType,
                    targetProperty.Name,
                    ExplicitInterfaceImplementation: baseType,
                    Expression: targetProperty.Name
                ),
                ..path
                    .Select(PropertySpec[] (_, i) =>
                        [
                            new PropertySpec(
                                string.Join(".", parts.Take(i + 1).Append(extension.Type.Split('.').Last())),
                                targetProperty.Name,
                                ExplicitInterfaceImplementation: string.Join(
                                    ".",
                                    path
                                        .Take(i)
                                        .Select(x => x.TypeName)
                                        .Prepend(traitInfo.Trait.ReferenceName)
                                        .Append(component.TypeName)
                                ),
                                Expression: targetProperty.Name
                            ),
                            new PropertySpec(
                                string.Join(".", parts.Take(i + 1)),
                                targetProperty.Name,
                                ExplicitInterfaceImplementation: string.Join(
                                    ".",
                                    path
                                        .Take(i)
                                        .Select(x => x.TypeName)
                                        .Prepend(traitInfo.Trait.ReferenceName)
                                ),
                                Expression: targetProperty.Name
                            )
                        ]
                    )
                    .SelectMany(x => x)
            ]);
    }

    private LinkExtensionContext? MapLinkExtensionContext(
        GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        if (context.TargetNode is not InterfaceDeclarationSyntax syntax)
            return null;

        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;

        if (syntax.Parent is not TypeDeclarationSyntax traitSyntax)
            return null;

        if (traitSyntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
            return null;

        var extensions = context
            .Attributes
            .Select(x =>
                (
                    Name: x.ConstructorArguments[0].Value as string,
                    Type: x.ConstructorArguments[1].Value?.ToString(),
                    Getter: x.ConstructorArguments.ElementAtOrDefault(2).Value as string
                )
            )
            .Where(x => x.Name is not null && x.Type is not null)
            .ToImmutableEquatableArray();

        if (extensions.Count == 0)
            return null;

        return new(symbol.ToDisplayString(), extensions!);
    }

    private Context? MapTraitComponent(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetNode is not InterfaceDeclarationSyntax syntax)
            return null;

        if (context.TargetSymbol is not INamedTypeSymbol symbol)
            return null;

        if (!symbol.Name.EndsWith("Component"))
            return null;

        if (!symbol.Interfaces.Contains(symbol.ContainingType))
            return null;

        if (syntax.Parent is not TypeDeclarationSyntax traitSyntax)
            return null;

        if (traitSyntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
            return null;

        if (context.Attributes.Length != 1)
            return null;

        return new Context(
            context.TargetSymbol.ContainingType.ToDisplayString(),
            new(symbol),
            (
                context.Attributes[0]
                        .NamedArguments
                        .FirstOrDefault(x => x.Key == "Parent")
                        .Value
                        .Value
                    as ITypeSymbol
            )?.ToDisplayString(),
            symbol.ContainingType
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Select(x => (x.Type.ToDisplayString(), x.Name))
                .ToImmutableEquatableArray()
        );
    }
}