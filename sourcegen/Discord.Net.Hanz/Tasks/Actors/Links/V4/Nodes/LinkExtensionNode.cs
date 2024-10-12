using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class LinkExtensionNode :
    LinkModifierNode,
    ITypeImplementerNode
{
    public class ExtensionProperty(
        LinkExtensionNode node,
        IPropertySymbol symbol,
        bool isLinkMirror,
        bool isBackLinkMirror,
        ActorNode? propertyTarget,
        LinkExtensionNode? overloadedBase)
    {
        public bool IsOverload => Symbol.ExplicitInterfaceImplementations.Length > 0;
        public string Name => MemberUtils.GetMemberName(Symbol);
        public LinkExtensionNode Node { get; } = node;
        public IPropertySymbol Symbol { get; } = symbol;
        public LinkExtensionNode? OverloadedBase { get; } = overloadedBase;
        public bool IsLinkMirror { get; } = isLinkMirror;
        public bool IsBackLinkMirror { get; } = isBackLinkMirror;
        public ActorNode? PropertyTarget { get; } = propertyTarget;
        public bool IsValid { get; private set; }
        public string Type { get; private set; } = string.Empty;

        public bool IsDefinedOnPath => IsValid && (Node.IsTemplate || !IsBackLinkMirror && IsLinkMirror);

        public ExtensionProperty GetClosestDefinedVariant()
        {
            if (
                IsDefinedOnPath ||
                !Node.CartesianExtensionProperties.TryGetValue(Name, out var cartesian)
            ) return this;

            return cartesian.FirstOrDefault(x => x.IsDefinedOnPath) ?? this;
        }

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

            Type = GetPropertyType();
        }

        public string GetPropertyType(bool backlink = false, bool useCoreTypes = false)
        {
            if (!IsValid) return string.Empty;

            var actorType = useCoreTypes
                ? PropertyTarget?.Target.GetCoreActor()
                : PropertyTarget?.Target.Actor;

            if (IsBackLinkMirror)
            {
                if (backlink)
                {
                    return $"{actorType}.BackLink<TSource>";
                }

                return $"{actorType}";
            }

            if (IsLinkMirror)
            {
                return Node.IsTemplate
                    ? (useCoreTypes ? PropertyTarget!.FormattedCoreLink : PropertyTarget!.FormattedLink)
                    : $"{actorType}{Node.FormatRelativeTypePath()}";
            }

            return Symbol.Type.ToDisplayString();
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
                 > Symbol: {Symbol}
                 """;
        }
    }

    public INamedTypeSymbol ExtensionSymbol { get; }

    public bool IsTemplate => Parent is ActorNode;

    public bool WillGenerateImplementation => !IsCore;

    public string ImplementationClassName => $"__{Target.Assembly}LinkExtension{
        string.Join(
            string.Empty,
            Parents
                .Select(x =>
                    x switch {
                        LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
                        LinkHierarchyNode => "Hierarchy",
                        _ => null
                    }
                )
                .OfType<string>()
                .Reverse()
        )
    }{GetTypeName()}";

    public Constructor? Constructor { get; private set; }

    public List<Property> Properties { get; } = [];

    public List<ExtensionProperty> ExtensionProperties { get; } = [];

    public Dictionary<string, ExtensionProperty[]> CartesianExtensionProperties { get; private set; } = [];

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
        base.Visit(context, logger);

        ExtensionProperties.Clear();

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
                propertySymbol.Type is INamedTypeSymbol named
                    ? IsCore
                        ? context.Graph.Nodes.TryGetValue(named, out var targetNode)
                            ? targetNode
                            : null
                        : context.Graph.Nodes.FirstOrDefault(x => x
                            .Value
                            .Target
                            .GetCoreActor()
                            .Equals(named, SymbolEqualityComparer.Default)
                        ).Value
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

            ExtensionProperties.Add(new ExtensionProperty(
                this,
                propertySymbol,
                attribute is not null,
                attribute?.NamedArguments
                    .FirstOrDefault(x => x.Key == "OnlyBackLinks")
                    .Value.Value as bool? == true,
                propertyTarget,
                overloadedBase
            ));
        }

        ExtensionProperties.ForEach(x => x.Visit(this, logger));

        Properties.AddRange(
            ExtensionProperties
                .Where(x => x.IsValid && x.Type != string.Empty)
                .Select(x =>
                    new Property(
                        x.Name,
                        x.Type,
                        isVirtual: Children.Any(x => x is BackLinkNode)
                    )
                )
        );

        Constructor = new(
            ImplementationClassName,
            Properties
                .Select(x =>
                    new ConstructorParamter(
                        ToParameterName(x.Name),
                        x.Type,
                        initializes: x
                    )
                )
                .ToList(),
            Parents.OfType<ITypeImplementerNode>().FirstOrDefault(x => x.WillGenerateImplementation)?.Constructor
        );

        CartesianExtensionProperties = CartesianLinkTypeNodes.OfType<LinkExtensionNode>()
            .SelectMany(x => x.ExtensionProperties)
            .Where(x => x.IsDefinedOnPath)
            .GroupBy(x => x.Name)
            .ToDictionary(x => x.Key, x => x.ToArray());
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var bases = new List<string>();
        var members = new List<string>();

        members.AddRange(
            ExtensionProperties
                .Where(x => x.IsDefinedOnPath)
                .SelectMany(x =>
                {
                    ExtensionProperty[]? existingMembers = null;

                    var isNew = !IsCore || CartesianExtensionProperties.TryGetValue(x.Name, out existingMembers);

                    var results = new List<string>()
                    {
                        $"{(isNew ? "new " : string.Empty)}{x.Type} {x.Name} {{ get; }}"
                    };

                    var corePath = $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.{GetTypeName()}";

                    if (!IsCore)
                    {
                        results.Add(
                            $"{x.GetPropertyType(useCoreTypes: true)} {corePath}.{x.Name} => {x.Name};"
                        );
                    }

                    if (existingMembers?.Length > 0)
                    {
                        results.AddRange(
                            existingMembers.SelectMany(x =>
                            {
                                var result = new List<string>()
                                {
                                    $"{x.Type} {x.Node.FormatAsTypePath()}.{x.Name} => {x.Name};"
                                };

                                if (!IsCore)
                                    result.Add(
                                        $"{x.GetPropertyType(useCoreTypes: true)} {corePath}.{x.Name} => {x.Name}");

                                return result;
                            })
                        );
                    }

                    return results;
                })
        );

        if (!IsCore)
        {
            bases.Add($"{Target.GetCoreActor()}{FormatRelativeTypePath()}.{GetTypeName()}");
        }

        if (!IsTemplate)
        {
            bases.AddRange([
                FormatTypePath(),
            ]);

            logger.Log($"{FormatAsTypePath()}: {CartesianLinkTypeNodes.Count} cartesian products");

            foreach (var relative in CartesianLinkTypeNodes.OfType<LinkExtensionNode>())
            {
                bases.Add($"{relative.FormatAsTypePath()}");

                var coreOverrideTarget = $"{Target.GetCoreActor()}{relative.FormatRelativeTypePath()}.{GetTypeName()}";

                if (!IsCore)
                    bases.Add(coreOverrideTarget);
            }
        }

        // TODO: base exts
        // foreach (var baseExtension in ExtensionProperties.Select(x => x.OverloadedBase).OfType<LinkExtensionNode>())
        // {
        //     bases.Add($"{baseExtension.FormatTypePath()}.{baseExtension.GetTypeName()}");
        // }

        if (WillGenerateImplementation)
            CreateImplementation(members, bases, context, logger);

        members.Add(BuildChildren(context, logger));

        return
            $$"""
              public interface {{GetTypeName()}}{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}{string.Join($",{Environment.NewLine}", bases.OrderBy(x => x))}".WithNewlinePadding(4)
                      : string.Empty
              )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private void CreateImplementation(
        List<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        if (RootActorNode is null) return;

        var extensionMembers = new List<string>();
        var extensionBases = new List<string>()
        {
            FormatAsTypePath()
        };

        if (
            Parents
                .OfType<ITypeImplementerNode>()
                .FirstOrDefault(x => x.WillGenerateImplementation)
            is LinkNode baseLinkType and ITypeImplementerNode baseImplementer)
        {
            baseLinkType.GetPathGenerics(out var parentGenerics, out _);

            extensionBases.Insert(0, $"{Target.Actor}.{baseImplementer.ImplementationClassName}{(
                parentGenerics.Count > 0
                    ? $"<{string.Join(", ", parentGenerics)}>"
                    : string.Empty
            )}");
        }

        extensionMembers.AddRange(Properties.Select(x => x.Format()));

        extensionMembers.AddRange(
            ExtensionProperties
                .Where(x => Properties.Any(y => y.Name == x.Name))
                .Select(x => x.GetClosestDefinedVariant())
                .Where(x => x.IsValid)
                .Select(x => $"{x.Type} {x.Node.FormatAsTypePath()}.{x.Name} => {x.Name};")
        );

        if (Constructor is not null)
            extensionMembers.Add(Constructor.Format());

        GetPathGenerics(out var generics, out var constraints);

        RootActorNode.AdditionalTypes.Add(
            $$"""
              private protected class {{ImplementationClassName}}{{(generics.Count > 0 ? $"<{string.Join(", ", generics)}>" : string.Empty)}} : 
                  {{string.Join($",{Environment.NewLine}", extensionBases.Distinct()).WithNewlinePadding(4)}}
                  {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, extensionMembers.Distinct()).WithNewlinePadding(4)}}
              }   
              """
        );

        var ctorParams = Constructor?.GetActualParameters() ?? [];

        members.Add(
            $$"""
              internal static {{FormatAsTypePath()}} Create({{(
                  ctorParams.Count > 0
                      ? $"{Environment.NewLine}{string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x =>
                              x.Format()
                          )
                      )}".WithNewlinePadding(4) + Environment.NewLine
                      : string.Empty
              )}}) => new {{Target.Actor}}.{{ImplementationClassName}}<{{string.Join(", ", generics)}}>({{(
              ctorParams.Count > 0
                  ? string.Join(", ", ctorParams.Select(x => x.Name))
                  : string.Empty
          )}});
              """
        );
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
             Properties: {ExtensionProperties.Count}{(
                 ExtensionProperties.Count > 0
                     ? $"{Environment.NewLine}{string.Join(Environment.NewLine, ExtensionProperties.Select(x => $"- {x}"))}"
                     : string.Empty
             )}
             Is Template: {IsTemplate}
             Symbol: {ExtensionSymbol}
             """;
    }
}