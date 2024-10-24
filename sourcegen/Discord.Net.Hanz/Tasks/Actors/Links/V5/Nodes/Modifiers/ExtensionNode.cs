using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Common;
using Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V5.Nodes;

public class ExtensionNode :
    Node,
    INestedTypeProducerNode
{
    public readonly record struct Extension(
        string Actor,
        string Name,
        ImmutableEquatableArray<Extension.Property> Properties
    )
    {
        public readonly record struct Property(
            string Name,
            string Type,
            string? Overloads,
            Property.Kind PropertyKind,
            ActorInfo? ActorInfo = null
        )
        {
            public enum Kind
            {
                Normal,
                LinkMirror,
                BackLinkMirror
            }

            public bool IsDefinedOnPath(TypePath path)
            {
                var isRoot = path.Equals(typeof(ActorNode), typeof(ExtensionNode));

                return PropertyKind switch
                {
                    Kind.Normal => isRoot,
                    Kind.LinkMirror => path.Contains<LinkNode>() || isRoot,
                    Kind.BackLinkMirror => isRoot ||
                                           path.Equals(typeof(ActorNode), typeof(ExtensionNode), typeof(BackLinkNode)),
                    _ => false
                };
            }

            public static Property Create(IPropertySymbol symbol)
            {
                var kind = Kind.Normal;

                var linkMirrorAttribute = symbol.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "LinkMirrorAttribute");

                if (linkMirrorAttribute is not null)
                {
                    kind = linkMirrorAttribute
                        .NamedArguments
                        .FirstOrDefault(x => x.Key == "OnlyBackLinks")
                        .Value
                        .Value is true
                        ? Kind.BackLinkMirror
                        : Kind.LinkMirror;
                }


                return new Property(
                    MemberUtils.GetMemberName(symbol),
                    symbol.Type.ToDisplayString(),
                    symbol.ExplicitInterfaceImplementations.FirstOrDefault()?.ContainingType.ToDisplayString(),
                    kind
                );
            }
        }

        public Extension UpdateWithActorInfos(in Grouping<string, ActorInfo> grouping)
        {
            var properties = Properties.ToArray();

            for (var i = 0; i < properties.Length; i++)
            {
                ref var property = ref properties[i];
                if (property.PropertyKind is Property.Kind.Normal) continue;

                property = property with
                {
                    ActorInfo = grouping.GetValueOrDefault(property.Type, property.ActorInfo)
                };
            }

            return this with {Properties = properties.ToImmutableEquatableArray()};
        }

        public static IEnumerable<Extension> GetExtensions(
            LinkActorTargets.GenerationTarget target,
            CancellationToken cancellationToken)
        {
            var types = target
                .GetCoreActor()
                .GetTypeMembers()
                .Where(x => x.Name.EndsWith("Extension"))
                .Where(x => x
                    .GetAttributes()
                    .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute")
                );

            foreach (var extensionSymbol in types)
            {
                yield return new Extension(
                    target.Actor.ToDisplayString(),
                    extensionSymbol.Name.Replace("Extension", string.Empty),
                    extensionSymbol
                        .GetMembers()
                        .OfType<IPropertySymbol>()
                        .Select(Property.Create)
                        .ToImmutableEquatableArray()
                );
            }
        }
    }

    public readonly record struct BuildContext(
        ActorInfo ActorInfo,
        TypePath Path,
        ImmutableEquatableArray<Extension> Extensions
    );

    public readonly record struct ExtensionContext(
        Extension Extension,
        ActorInfo ActorInfo,
        TypePath Path
    ) : IPathedState;

    private readonly IncrementalValueProvider<Grouping<string, Extension>> _extensions;

    public ExtensionNode(
        NodeProviders providers,
        Logger logger
    ) : base(providers, logger)
    {
        _extensions = providers
            .Actors
            .SelectMany(Extension.GetExtensions)
            .Combine(
                providers.ActorInfos
            )
            .Select((tuple, _) => tuple.Left.UpdateWithActorInfos(tuple.Right))
            .GroupBy(x => x.Actor);
    }

    public IncrementalValuesProvider<Branch<TypeSpec>> Create<TSource>(
        IncrementalValuesProvider<Branch<(NestedTypeProducerContext Parameters, TSource Source)>> provider)
    {
        var extensionProvider = provider
            .Combine(
                _extensions,
                x => x.Value.Parameters.ActorInfo.Actor.DisplayString,
                (branch, extensions) => branch
                    .Mutate(
                        new BuildContext(
                            branch.Value.Parameters.ActorInfo,
                            branch.Value.Parameters.Path,
                            extensions
                        )
                    )
            )
            .Where(x => x.Extensions.Count > 0)
            .SelectMany(BuildExtensions);

        var nestedProvider = AddNestedTypes(
            extensionProvider,
            (context, token) => new NestedTypeProducerContext(context.ActorInfo, context.Path),
            GetInstance<BackLinkNode>()
        );

        return ApplyPathNesting(nestedProvider).Select((x, _) => x.Spec);
    }

    private IEnumerable<StatefulGeneration<ExtensionContext>> BuildExtensions(
        BuildContext context,
        CancellationToken token)
    {
        foreach (var extension in context.Extensions)
        foreach (var result in Build(extension, context.Extensions.Remove(extension), context.Path))
        {
            token.ThrowIfCancellationRequested();
            yield return result;
        }

        yield break;

        IEnumerable<StatefulGeneration<ExtensionContext>> Build(
            Extension extension,
            ImmutableEquatableArray<Extension> next,
            TypePath path)
        {
            var extensionPath = path.Add<ExtensionNode>(extension.Name);
            yield return BuildExtension(context.ActorInfo, extensionPath, extension);

            token.ThrowIfCancellationRequested();

            if (next.Count == 0) yield break;

            var nextExtensions = next.Remove(extension);

            foreach (var child in next)
            foreach
            (
                var result
                in Build(
                    child,
                    nextExtensions,
                    extensionPath
                )
            )
                yield return result;
        }
    }


    private TypeSpec BuildExtensionGraph(
        TypeSpec extension,
        ImmutableArray<TypeSpec> children,
        BuildContext context,
        TypePath extensionPath)
    {
        if (extensionPath.IsEmpty)
        {
            return extension.AddNestedTypes(
                children.Select(x =>
                    BuildExtensionGraph(
                        x,
                        children.Remove(x),
                        context,
                        extensionPath.Add<ExtensionNode>(x.Name)
                    )
                )
            );
        }

        var newBases = new ImmutableEquatableArray<string>([(context.Path + extensionPath).ToString()]);

        for (var i = 1; i < extensionPath.Count; i++)
        {
            newBases = newBases.Add(
                context.Path + extensionPath.Slice(0, i) + (typeof(ExtensionNode), extension.Name)
            );
        }

        return extension with {Bases = newBases};
    }

    private StatefulGeneration<ExtensionContext> BuildExtension(
        ActorInfo actorInfo,
        TypePath path,
        Extension extension)
    {
        using var logger = Logger.GetSubLogger(actorInfo.Assembly.ToString())
            .GetSubLogger(nameof(BuildExtension))
            .GetSubLogger(actorInfo.Actor.MetadataName);

        logger.Log($"Extension for {actorInfo.Actor.DisplayString}:");
        logger.Log($" - {extension}");
        logger.Log($" - {path}");

        foreach (var property in extension.Properties)
        {
            logger.Log(
                $"   - {property.Name}: {{ {property.PropertyKind}, {property.Type}, Has Actor: {property.ActorInfo.HasValue}, Is On Path: {property.IsDefinedOnPath(path)} }} ");
        }

        return new StatefulGeneration<ExtensionContext>(
            new(extension, actorInfo, path),
            new TypeSpec(
                Name: extension.Name,
                Kind: TypeKind.Interface,
                Properties: extension.Properties
                    .SelectMany(x =>
                        BuildExtensionProperty(path, x, extension)
                    )
                    .ToImmutableEquatableArray(),
                Bases: path.Equals(typeof(ActorNode), typeof(ExtensionNode))
                    ? ImmutableEquatableArray<string>.Empty
                    : new([path])
            )
        );
    }

    public static IEnumerable<PropertySpec> BuildExtensionProperty(
        TypePath path,
        Extension.Property property,
        Extension extension)
    {
        if (!property.IsDefinedOnPath(path))
            yield break;

        if (property.PropertyKind is not Extension.Property.Kind.Normal && property.ActorInfo is null)
            yield break;

        var hasNewKeyword = property.PropertyKind switch
        {
            Extension.Property.Kind.Normal => false,
            Extension.Property.Kind.LinkMirror or Extension.Property.Kind.BackLinkMirror => path.Contains<LinkNode>(),
            _ => false
        };

        var propertyType = property.PropertyKind switch
        {
            Extension.Property.Kind.Normal => property.Type,
            Extension.Property.Kind.LinkMirror =>
                path.Equals(typeof(ActorNode), typeof(ExtensionNode))
                    ? property.ActorInfo!.Value.FormattedLink
                    : $"{property.ActorInfo!.Value.Actor}.{path.OfType<LinkNode>().FormatRelative()}",
            Extension.Property.Kind.BackLinkMirror =>
                path.Last?.Type == typeof(BackLinkNode)
                    ? $"{property.ActorInfo!.Value.Actor}.BackLink<TSource>"
                    : property.ActorInfo!.Value.Actor.FullyQualifiedName,
            _ => throw new ArgumentOutOfRangeException()
        };

        var spec = new PropertySpec(
            Name: property.Name,
            Type: propertyType,
            Modifiers: hasNewKeyword
                ? new(["new"])
                : ImmutableEquatableArray<string>.Empty
        );

        yield return spec;

        switch (property.PropertyKind)
        {
            case Extension.Property.Kind.LinkMirror:
                foreach (var pathProduct in path.OfType<LinkNode>().CartesianProduct())
                {
                    yield return new PropertySpec(
                        Name: property.Name,
                        Type: $"{property.ActorInfo!.Value.Actor.DisplayString}.{pathProduct.FormatRelative()}",
                        ExplicitInterfaceImplementation: $"{extension.Actor}.{pathProduct}.{extension.Name}",
                        Expression: property.Name
                    );
                }

                break;
            case Extension.Property.Kind.BackLinkMirror when path.Last?.Type == typeof(BackLinkNode):
                yield return new PropertySpec(
                    Name: property.Name,
                    Type: property.ActorInfo!.Value.Actor.FullyQualifiedName,
                    ExplicitInterfaceImplementation: $"{extension.Actor}.{extension.Name}",
                    Expression: property.Name
                );
                break;
        }
    }
}