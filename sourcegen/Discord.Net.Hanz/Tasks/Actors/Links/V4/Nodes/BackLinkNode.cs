using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;

public class BackLinkNode(LinkTarget target) :
    LinkNode(target),
    ITypeImplementerNode
{
    public bool IsTemplate
        => Parent is ActorNode;

    public bool IsClass => !IsCore && IsTemplate;

    public bool HasImplementation { get; private set; }
    public bool RedefinesSource { get; private set; }

    public string ImplementationClassName
        => $"__{Target.Assembly}{
            string.Join(
                string.Empty,
                Parents
                    .Select(x =>
                        x switch {
                            LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
                            LinkExtensionNode extension => extension.GetTypeName(),
                            LinkHierarchyNode => "Hierarchy",
                            _ => null
                        }
                    )
                    .OfType<string>()
                    .Reverse()
            )
        }BackLink";

    public List<(string Type, string Name, string? Default)> RequiredMembers { get; }
        = [("TSource", "Source", null)];

    public List<(string Type, string Name, string? Default)> ConstructorMembers { get; } = [];

    public List<string> SpecialMembers { get; } = [];
    public List<string> SpecialInitializations { get; } = [];

    public List<BackLinkNode> InheritedBackLinks { get; } = [];

    private protected override void Visit(NodeContext context, Logger logger)
    {
        InheritedBackLinks.Clear();
        ConstructorMembers.Clear();
        SpecialMembers.Clear();
        SpecialInitializations.Clear();

        HasImplementation = Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
        RedefinesSource = HasImplementation || GetEntityAssignableAncestors(context).Length > 0;

        foreach (var relative in ContainingNodes.Select(x => x.Children.OfType<BackLinkNode>().FirstOrDefault()))
        {
            if (relative is null || relative == this) continue;

            logger.Log($"{FormatTypePath()} BackLink += {relative.FormatTypePath()}");

            InheritedBackLinks.Add(relative);
            RedefinesSource |= relative.RedefinesSource;
        }

        ConstructorMembers.AddRange(RequiredMembers);

        if (!IsCore)
        {
            switch (Parent)
            {
                case LinkHierarchyNode linkHierarchy:
                    SpecialMembers.AddRange(
                        linkHierarchy.HierarchyNodes
                            .SelectMany(IEnumerable<string> (x) =>
                            {
                                var type =
                                    $"{x.Target.Actor}{linkHierarchy.FormatRelativeTypePath()}.BackLink<TSource>";
                                var name = LinksV4.GetFriendlyName(x.Target.Actor);

                                return
                                [
                                    $"internal override {type} {name} {{ get; }}",
                                    $"{type} {FormatAsTypePath()}.{name} => {name}"
                                ];
                            })
                    );

                    SpecialInitializations.AddRange(
                        linkHierarchy.HierarchyNodes.Select(x =>
                            $"{LinksV4.GetFriendlyName(x.Target.Actor)} = {ToParameterName(LinksV4.GetFriendlyName(x.Target.Actor))};"
                        )
                    );

                    ConstructorMembers.AddRange(
                        [
                            ..linkHierarchy
                                .ConstructorMembers
                                .Where(x => linkHierarchy
                                    .HierarchyNodes
                                    .All(y => LinksV4
                                            .GetFriendlyName(y.Target.Actor) != x.Name
                                    )
                                ),
                            ..linkHierarchy.HierarchyNodes.Select(x =>
                                (
                                    $"{x.Target.Actor}{linkHierarchy.FormatRelativeTypePath()}.BackLink<TSource>",
                                    LinksV4.GetFriendlyName(x.Target.Actor),
                                    (string?) null
                                )
                            )
                        ]
                    );

                    break;
                case ITypeImplementerNode parentImplementer:
                    ConstructorMembers.AddRange(parentImplementer.ConstructorMembers);
                    break;
            }
        }
    }

    private void CreateBackLinkInterface(
        HashSet<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        var ancestors = GetEntityAssignableAncestors(context);

        if (!IsCore)
        {
            bases.AddRange([
                FormattedBackLinkType,
                $"{Target.GetCoreActor()}{FormatRelativeTypePath()}.BackLink<TSource>"
            ]);
        }

        if (RedefinesSource)
        {
            members.UnionWith([
                "new TSource Source { get; }",
                $"TSource {FormattedCoreBackLinkType}.Source => Source;"
            ]);

            if (!IsCore)
                members.Add($"TSource {FormattedBackLinkType}.Source => Source;");
        }

        if (ancestors.Length > 0)
        {
            foreach (var ancestor in ancestors)
            {
                if (GetNodeWithEquivalentPathing(ancestor) is not BackLinkNode ancestorBackLink) continue;

                var ancestorPath = ancestorBackLink.FormatTypePath();

                bases.Add($"{ancestorPath}.BackLink<TSource>");

                var overrideType = ancestorBackLink.RedefinesSource
                    ? $"{ancestorPath}.BackLink<TSource>"
                    : ancestor.FormattedBackLinkType;

                members.Add($"TSource {overrideType}.Source => Source;");
            }
        }

        switch (Parent)
        {
            case LinkHierarchyNode hierarchy:
                members.UnionWith(hierarchy.GetFormattedProperties(true));

                if (!hierarchy.IsTemplate)
                {
                    var hierarchyBackLink = $"{Target.Actor}.Hierarchy.BackLink<TSource>";

                    if (hierarchy.Children.OfType<BackLinkNode>().FirstOrDefault() is {RedefinesSource: true})
                        members.Add($"TSource {hierarchyBackLink}.Source => Source;");
                }

                break;
            case LinkExtensionNode extension:
                members.UnionWith(
                    extension.Properties
                        .SelectMany(x => extension.FormatProperty(x, true))
                );

                if (!extension.IsTemplate)
                {
                    var templateExtensionBackLink = $"{Target.Actor}.{extension.GetTypeName()}.BackLink<TSource>";

                    if (extension.Children.OfType<BackLinkNode>().FirstOrDefault() is {RedefinesSource: true})
                        members.Add($"TSource {templateExtensionBackLink}.Source => Source;");
                }

                break;
        }

        foreach (var inheritedBackLink in InheritedBackLinks)
        {
            bases.Add($"{inheritedBackLink.FormatTypePath()}.BackLink<TSource>");

            if (!inheritedBackLink.RedefinesSource) continue;

            members.Add($"TSource {inheritedBackLink.FormatTypePath()}.BackLink<TSource>.Source => Source;");
        }
    }

    public override string Build(NodeContext context, Logger logger)
    {
        var members = new HashSet<string>();

        var bases = new List<string>()
        {
            FormatTypePath(),
            FormattedBackLinkType
        };

        if (IsClass)
            CreateImplementation(members, bases, context, logger);
        else
        {
            CreateBackLinkInterface(members, bases, context, logger);

            if (!IsCore)
                CreateImplementation(members, bases, context, logger);
        }


        var kind = IsClass ? "class" : "interface";
        var name = IsClass ? GetTypeName() : "BackLink<out TSource>";

        return
            $$"""
              public {{kind}} {{name}} :
                  {{string.Join($",{Environment.NewLine}", bases.Distinct()).WithNewlinePadding(4)}}
                  where TSource : class, IPathable
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private void CreateImplementation(
        HashSet<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        if (IsClass)
        {
            CreateRootImplementation(members, bases, context, logger);
            return;
        }

        if (RootActorNode is null) return;

        // var ancestors = GetAncestors(context);
        var backLinkMembers = new List<string>()
        {
            "internal TSource Source { get; }",
            $"TSource {FormatAsTypePath()}.Source => Source;"
        };

        var backlinkBases = new List<string>()
        {
            FormatAsTypePath()
        };

        List<(string Type, string Name, string? Default)> baseCtorParameters = [];

        if (Parent is ITypeImplementerNode implementer)
        {
            baseCtorParameters = implementer.ConstructorMembers;

            Parent.GetPathGenerics(out var parentGenerics, out _);
            backlinkBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
                parentGenerics.Count > 0
                    ? $"<{string.Join(", ", parentGenerics)}>"
                    : string.Empty
            )}");
        }

        GetPathGenerics(out var generics, out var constraints);

        var typeName = $"__{Target.Assembly}{
            string.Join(
                string.Empty,
                Parents
                    .Select(x =>
                        x switch {
                            LinkTypeNode linkType => $"{linkType.Entry.Symbol.Name}{(linkType.Entry.Symbol.TypeParameters.Length > 0 ? linkType.Entry.Symbol.TypeParameters.Length : string.Empty)}",
                            LinkExtensionNode extension => extension.GetTypeName(),
                            LinkHierarchyNode => "Hierarchy",
                            _ => null
                        }
                    )
                    .OfType<string>()
                    .Reverse()
            )
        }BackLink";

        var ctorParams = string.Join(
            $",{Environment.NewLine}",
            ConstructorMembers
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .Select(x =>
                    $"{x.Type} {ToParameterName(x.Name)}{(x.Default is not null ? $" = {x.Default}" : string.Empty)}"
                )
        );

        var baseArgs = baseCtorParameters
            .GroupBy(x => x.Name)
            .Select(x => x.First())
            .Select(x => ToParameterName(x.Name))
            .ToArray();

        backLinkMembers.AddRange(
            SpecialMembers
        );

        backLinkMembers.Add(
            $$"""
              public {{typeName}}(
                  {{ctorParams.WithNewlinePadding(4)}}
              ){{(
                  baseCtorParameters.Count > 0
                      ? $" : base({string.Join(", ", baseArgs)})"
                      : string.Empty
              )}}
              {
                  Source = source;{{(
                      SpecialInitializations.Count > 0
                          ? $"{Environment.NewLine}{
                              string.Join(
                                  Environment.NewLine,
                                  SpecialInitializations
                              )}".WithNewlinePadding(4)
                          : string.Empty
                  )}}
              }
              """
        );

        members.Add(
            $$"""
              internal static {{FormatAsTypePath()}} Create
              (
                  {{ctorParams.WithNewlinePadding(4)}}
              ) => new {{Target.Actor}}.{{typeName}}<{{string.Join(", ", generics)}}>(source{{(
                  baseCtorParameters.Count > 0
                      ? $", {string.Join(", ", baseArgs)}"
                      : string.Empty
              )}});
              """
        );

        RootActorNode.AdditionalTypes.Add(
            $$"""
              private sealed class {{typeName}}<{{string.Join(", ", generics)}}> : 
                  {{string.Join($",{Environment.NewLine}", backlinkBases.Distinct()).WithNewlinePadding(4)}}
                  {{string.Join(Environment.NewLine, constraints).WithNewlinePadding(4)}}
              {
                  {{string.Join(Environment.NewLine, backLinkMembers.Distinct()).WithNewlinePadding(4)}}
              }   
              """
        );
    }

    private void CreateRootImplementation(
        HashSet<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        var coreBackLinkTemplate = Target.GetCoreActor()
            .GetTypeMembers()
            .FirstOrDefault(x => x.Name == "BackLink");

        if (coreBackLinkTemplate is null)
        {
            logger.Warn("Could not find core BackLink template");
            return;
        }

        // create the root back link type
        bases.AddRange([
            FormattedBackLinkType,
            $"{Target.GetCoreActor()}.BackLink<TSource>"
        ]);

        // only need to define once, since we'll
        members.UnionWith([
            "internal TSource Source { get; }",
            $"TSource {FormattedBackLinkType}.Source => Source;"
        ]);

        var overrideType = coreBackLinkTemplate
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Any(x =>
                x.Name == "Source" &&
                x.ExplicitInterfaceImplementations.Length == 0
            )
            ? $"{Target.GetCoreActor()}.BackLink<TSource>"
            : FormattedCoreBackLinkType;

        members.Add($"TSource {overrideType}.Source => Source;");

        BuildConstructorsFromTargetActor(members);

        members.UnionWith([
            $"{Target.Actor} {FormattedActorProvider}.GetActor({Target.Id} id) => this;",
            $"{Target.GetCoreActor()} {FormattedCoreActorProvider}.GetActor({Target.Id} id) => this;",
        ]);
    }

    private void BuildConstructorsFromTargetActor(HashSet<string> members, string? typeName = null)
    {
        typeName ??= "BackLink";

        foreach (var constructor in Target.Actor.InstanceConstructors)
        {
            var parameters = constructor
                .Parameters
                .Select(x =>
                    $"{x.Type} {x.Name}{(x.HasExplicitDefaultValue ? $" = {SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)}" : string.Empty)}"
                )
                .Prepend("TSource source");

            var args = constructor
                .Parameters
                .Select(x => x.Name);

            members.Add(
                $$"""
                  internal {{typeName}}(
                      {{string.Join($",{Environment.NewLine}", parameters).WithNewlinePadding(4)}}
                  ) : base(
                      {{string.Join($",{Environment.NewLine}", args).WithNewlinePadding(4)}}
                  )
                  {
                      Source = source;
                  }
                  """
            );
        }
    }

    public string GetTypeName()
        => "BackLink<TSource>";

    public override string ToString()
    {
        return
            $"""
             {base.ToString()}
             Is Template?: {IsTemplate}
             Has Implementation?: {HasImplementation}
             Redefines Source?: {RedefinesSource}
             Inherited BackLinks: {InheritedBackLinks.Count}
             """;

//        {(
//                  InheritedBackLinks.Count > 0
//                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, InheritedBackLinks.Select(x => $"- {x}".Replace(Environment.NewLine, $"{Environment.NewLine}  > ")))}"
//                      : string.Empty
//              )}
    }
}