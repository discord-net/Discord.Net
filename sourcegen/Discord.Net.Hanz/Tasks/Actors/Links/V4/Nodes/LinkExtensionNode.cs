using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class LinkExtensionNode :
    LinkNode,
    ITypeProducerNode
{
    public class ExtensionProperty(
        IPropertySymbol symbol,
        bool isLinkMirror,
        bool isBackLinkMirror,
        ActorNode? propertyTarget,
        LinkExtensionNode? overloadedBase)
    {
        public bool IsOverload => Symbol.ExplicitInterfaceImplementations.Length > 0;
        public string Name => MemberUtils.GetMemberName(Symbol);
        public IPropertySymbol Symbol { get; } = symbol;
        public LinkExtensionNode? OverloadedBase { get; } = overloadedBase;
        public bool IsLinkMirror { get; } = isLinkMirror;
        public bool IsBackLinkMirror { get; } = isBackLinkMirror;
        public ActorNode? PropertyTarget { get; } = propertyTarget;

        public bool IsValid { get; private set; }

        public void Visit(LinkExtensionNode node, Logger logger)
        {
            if (IsOverload && OverloadedBase is null)
            {
                logger.Log($"{node}: {Name} has no overload base");
                IsValid = false;
                return;
            }

            if (IsBackLinkMirror && PropertyTarget is null)
            {
                logger.Log($"{node}: {Name} has no property target for backlink mirror");
                IsValid = false;
                return;
            }

            if (IsLinkMirror && PropertyTarget is null)
            {
                logger.Log($"{node}: {Name} has no property target for link mirror");
                IsValid = false;
                return;
            }

            IsValid = true;
        }

        public override string ToString()
        {
            return
                $"""
                 {Name}:
                 > Valid?: {IsValid}
                 > Is Overload?: {IsOverload}
                 > Is BackLink Mirror?: {IsBackLinkMirror}
                 > Is Link Mirror?: {IsLinkMirror}
                 > Symbol: {symbol}
                 """;
        }
    }

    public INamedTypeSymbol ExtensionSymbol { get; }

    public bool IsTemplate => Parent is ActorNode;

    public List<ExtensionProperty> Properties { get; } = [];

    public LinkExtensionNode(
        LinkTarget target,
        INamedTypeSymbol extensionSymbol
    ) : base(target)
    {
        ExtensionSymbol = extensionSymbol;
        AddChild(new BackLinkNode(target));
    }

    private protected override void Visit(NodeContext context, Logger logger)
    {
        Properties.Clear();

        foreach (var propertySymbol in ExtensionSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            LinkExtensionNode? overloadedBase = null;

            if (propertySymbol.ExplicitInterfaceImplementations.Length > 0)
            {
                var overloadedProp = propertySymbol.ExplicitInterfaceImplementations[0];

                var baseExtension =
                    propertySymbol.ContainingType.ContainingType is not null &&
                    context.Graph.Nodes.TryGetValue(propertySymbol.ContainingType.ContainingType, out var node)
                        ? node
                        : null;

                overloadedBase = baseExtension?.Children.OfType<LinkExtensionNode>().FirstOrDefault(
                    x => x.ExtensionSymbol.Equals(
                        overloadedProp.ContainingType,
                        SymbolEqualityComparer.Default
                    )
                );

                logger.Log(
                    $" - Overload search: {overloadedProp.ContainingType} in {overloadedProp.ContainingType.ContainingType}");
            }

            var attribute = propertySymbol.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkMirrorAttribute");

            var propertyTarget =
                propertySymbol.Type is INamedTypeSymbol named &&
                context.Graph.Nodes.TryGetValue(named, out var targetNode)
                    ? targetNode
                    : null;

            if (
                propertySymbol.Type.TypeKind is TypeKind.Unknown &&
                propertyTarget is null)
            {
                var propTypeStr = propertySymbol.Type.ToDisplayString();
                var prefix = Target.Assembly.ToString();

                var actorPart = propTypeStr
                    .Split(['.'], StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(x => x.StartsWith(prefix) && (x.EndsWith("Actor") || x.EndsWith("Trait")));

                if (actorPart is not null)
                {
                    var actorSymbol = context.Graph.Compilation
                        .GetTypeByMetadataName(
                            $"Discord{(prefix is not "Core" ? $".{prefix}" : string.Empty)}.{actorPart}"
                        );

                    if (actorSymbol is not null && context.Graph.Nodes.TryGetValue(actorSymbol, out targetNode))
                        propertyTarget = targetNode;
                }

                if (
                    propTypeStr.StartsWith("Indexable") ||
                    propTypeStr.StartsWith("Enumerable") ||
                    propTypeStr.StartsWith("Defined") ||
                    propTypeStr.StartsWith("Paged<")
                )
                {
                    propertyTarget = Parents.OfType<ActorNode>().First();
                }
            }

            Properties.Add(
                new ExtensionProperty(
                    propertySymbol,
                    attribute is not null,
                    attribute?.NamedArguments
                        .FirstOrDefault(x => x.Key == "OnlyBackLinks")
                        .Value.Value as bool? == true,
                    propertyTarget,
                    overloadedBase
                )
            );
        }

        Properties.ForEach(x => x.Visit(this, logger));
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var bases = new HashSet<string>();

        if (!IsTemplate)
        {
            bases.UnionWith([
                FormatTypePath(),
                $"{Target.Actor}.{GetTypeName()}"
            ]);

            var baseExtensionsProduct = GetProduct(
                Parents.Prepend(this).OfType<LinkExtensionNode>(), true
            ).ToArray();

            if (Parents.OfType<LinkTypeNode>().FirstOrDefault() is { } containingLinkNode)
            {
                foreach (var baseExtensions in baseExtensionsProduct)
                {
                    bases.Add(
                        $"{containingLinkNode.FormatAsTypePath()}.{string.Join(".", baseExtensions.Select(x => x.GetTypeName()))}"
                    );
                }
            }

            foreach (var product in LinkTypesProduct)
            {
                var productBase =
                    $"{Target.Actor}.{string.Join(".", product.Select(x => x.GetTypeName()))}";

                bases.Add($"{productBase}.{GetTypeName()}");

                foreach (var baseExtensions in baseExtensionsProduct)
                {
                    bases.Add(
                        $"{productBase}.{string.Join(".", baseExtensions.Select(x => x.GetTypeName()))}"
                    );
                }
            }
        }

        foreach (var baseExtension in Properties.Select(x => x.OverloadedBase).OfType<LinkExtensionNode>())
        {
            bases.Add($"{baseExtension.FormatTypePath()}.{baseExtension.GetTypeName()}");
        }

        return
            $$"""
              public interface {{GetTypeName()}}{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x))}".WithNewlinePadding(4)
                      : string.Empty
              )}}
              {
                  {{
                      string.Join(
                          Environment.NewLine,
                          Properties.SelectMany(x => FormatProperty(x))
                      ).WithNewlinePadding(4)
                  }}
                  {{BuildChildren(context, logger).WithNewlinePadding(4)}}
              }
              """;
    }

    public IEnumerable<string> FormatProperty(ExtensionProperty property, bool backlink = false)
    {
        // TODO: overloads
        if (property.IsOverload) return [];

        //var result = new List<string>();

        if (!property.IsValid)
        {
            return [$"// {property.Name} is invalid"];
        }

        if (property.IsBackLinkMirror)
        {
            if (!backlink && IsTemplate)
            {
                return
                [
                    $"{property.PropertyTarget!.Target.Actor} {property.Name} {{ get; }}"
                ];
            }

            if (backlink && IsTemplate)
            {
                return
                [
                    $"new {property.PropertyTarget!.Target.Actor}.BackLink<TSource> {property.Name} {{ get; }}",
                    $"{property.PropertyTarget!.Target.Actor} {FormatTypePath()}.{GetTypeName()}.{property.Name} => {property.Name};"
                ];
            }

            return
            [
                $"// '{property.Name}' fallthrough for backlink mirror"
            ];
        }

        if (property.IsLinkMirror)
        {
            if (IsTemplate)
            {
                return [$"{property.PropertyTarget!.FormattedLink} {property.Name} {{ get; }}"];
            }

            var path = FormatRelativeTypePath(x => x is LinkTypeNode or BackLinkNode);

            if (path == string.Empty)
                return
                [
                    $"// '{property.Name}' no path for link mirror"
                ];

            return
            [
                $"new {property.PropertyTarget!.Target.Actor}{path} {property.Name} {{ get; }}",
                $"{property.PropertyTarget!.FormattedLink} {Target.Actor}.{GetTypeName()}.{property.Name} => {property.Name};"
            ];
        }

        if (!IsTemplate || backlink)
            return
            [
                $"// '{property.Name}' skipped: raw in non-template"
            ];

        return
        [
            $"{property.Symbol.Type} {property.Name} {{ get; }}"
        ];
    }

    public string GetTypeName()
        => ExtensionSymbol.Name.Replace("Extension", string.Empty);

    public static void AddTo(LinkTarget target, LinkNode node)
    {
        var extensionTypes = target.GetCoreActor().GetTypeMembers()
            .Where(x => x
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute")
            )
            .ToImmutableList();

        if (extensionTypes.Count == 0) return;

        foreach (var extension in extensionTypes)
        {
            node.AddChild(Create(extension, extensionTypes));
        }

        return;

        LinkExtensionNode Create(INamedTypeSymbol extension, ImmutableList<INamedTypeSymbol> children)
        {
            var node = new LinkExtensionNode(target, extension);

            var nextChildren = children.Remove(extension);

            foreach (var child in nextChildren)
            {
                node.AddChild(Create(child, nextChildren));
            }

            return node;
        }
    }

    public override string ToString()
    {
        return
            $"""
             {base.ToString()}
             Properties: {Properties.Count}{(
                 Properties.Count > 0
                     ? $"{Environment.NewLine}{string.Join(Environment.NewLine, Properties.Select(x => $"- {x}"))}"
                     : string.Empty
             )}
             Is Template: {IsTemplate}
             Symbol: {ExtensionSymbol}
             """;
    }
}