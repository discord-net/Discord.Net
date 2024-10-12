using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
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

    public bool WillGenerateImplementation
        => RootActorNode is not null && Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;

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

    public Constructor? Constructor { get; private set; }

    public List<Property> Properties { get; } = [];

    public List<BackLinkNode> InheritedBackLinks { get; } = [];

    private protected override void Visit(NodeContext context, Logger logger)
    {
        InheritedBackLinks.Clear();
        Properties.Clear();

        HasImplementation = Target.Assembly is not LinkActorTargets.AssemblyTarget.Core;
        RedefinesSource = HasImplementation || GetEntityAssignableAncestors(context).Length > 0;

        foreach (var relative in ContainingNodes.Select(x => x.Children.OfType<BackLinkNode>().FirstOrDefault()))
        {
            if (relative is null || relative == this) continue;

            logger.Log($"{FormatTypePath()} BackLink += {relative.FormatTypePath()}");

            InheritedBackLinks.Add(relative);
            RedefinesSource |= relative.RedefinesSource;
        }

        //ConstructorMembers.AddRange(RequiredMembers);
        Properties.Add(new("Source", "TSource"));


        if (!IsCore)
        {
            switch (Parent)
            {
                case LinkHierarchyNode hierarchy:
                    Properties.AddRange(
                        hierarchy.HierarchyNodes
                            .Select(x =>
                                new Property(
                                    LinksV4.GetFriendlyName(x.Target.Actor),
                                    hierarchy.IsTemplate
                                        ? x.FormattedBackLinkType
                                        : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>",
                                    isOverride: true

                                    // x.Name,
                                    // hierarchy.IsTemplate  ? x.$"{x.Type}.BackLink<TSource>",
                                    // x.HasSetter,
                                    // isOverride: true
                                )
                            )
                    );
                    break;
            }
        }

        Constructor = new(
            ImplementationClassName,
            Properties
                .Select(x =>
                    new ConstructorParamter(
                        ToParameterName(x.Name),
                        x.Type,
                        null,
                        x)
                )
                .ToList(),
            Parents
                .OfType<ITypeImplementerNode>()
                .FirstOrDefault(x => x.WillGenerateImplementation)
                ?.Constructor
        );
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
                members.UnionWith(hierarchy
                    .CartesianHierarchyNodes
                    .Prepend(hierarchy)
                    .SelectMany(IEnumerable<string> (node) => node
                        .HierarchyNodes
                        .SelectMany(IEnumerable<string> (x) =>
                        {
                            var type = node.IsTemplate
                                ? x.FormattedBackLinkType
                                : $"{x.Target.Actor}{node.FormatRelativeTypePath()}.BackLink<TSource>";

                            var name = LinksV4.GetFriendlyName(x.Target.Actor);

                            var overrideType = node.IsTemplate
                                ? x.FormattedLink
                                : $"{x.Target.Actor}{node.FormatRelativeTypePath()}";

                            var results = new List<string>();

                            if (node == hierarchy)
                                results.Add($"new {type} {name} {{ get;}}");
                            else
                                results.Add(
                                    $"{type} {node.FormatAsTypePath()}.BackLink<TSource>.{name} => {name};"
                                );

                            results.Add($"{overrideType} {node.FormatAsTypePath()}.{name} => {name};");

                            if (!IsCore)
                            {
                                var coreType = node.IsTemplate
                                    ? x.FormattedCoreLink
                                    : $"{x.Target.GetCoreActor()}{node.FormatRelativeTypePath()}";
                                
                                var coreOverrideType = node.IsTemplate
                                    ? x.FormattedCoreBackLinkType
                                    : $"{x.Target.GetCoreActor()}{node.FormatRelativeTypePath()}.BackLink<TSource>";

                                results.AddRange([
                                    $"{coreOverrideType} {Target.GetCoreActor()}{node.FormatRelativeTypePath()}.Hierarchy.BackLink<TSource>.{name} => {name};",
                                    $"{coreType} {Target.GetCoreActor()}{node.FormatRelativeTypePath()}.Hierarchy.{name} => {name};"
                                ]);
                            }

                            return results;
                        })
                    )
                );

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
            $"TSource {FormatAsTypePath()}.Source => Source;"
        };

        backLinkMembers.AddRange(Properties.Select(x => x.Format()));

        switch (Parent)
        {
            case LinkHierarchyNode hierarchy:
                backLinkMembers.AddRange(
                    hierarchy.HierarchyNodes.Select(x =>
                    {
                        var type = hierarchy.IsTemplate
                            ? x.FormattedBackLinkType
                            : $"{x.Target.Actor}{hierarchy.FormatRelativeTypePath()}.BackLink<TSource>";

                        var name = LinksV4.GetFriendlyName(x.Target.Actor);

                        return $"{type} {FormatAsTypePath()}.{name} => {name};";
                    })
                );
                
                // foreach (var relative in GetRelativeNodes().OfType<LinkHierarchyNode>())
                // {
                //     foreach (var hierarchyNode in hierarchy.HierarchyNodes)
                //     {
                //         var type = relative.IsTemplate 
                //             ? hierarchyNode.FormattedLink 
                //             : $"{hierarchyNode.Target.Actor}{hierarchyNode.FormatRelativeTypePath()}";
                //         var name = LinksV4.GetFriendlyName(hierarchyNode.Target.Actor);
                //
                //         members.Add($"{type} {hierarchyNode.FormatAsTypePath()}.{name} => {name}");
                //     }
                // }
                // backLinkMembers.AddRange(
                //     hierarchy.GetRelativeNodes().OfType<LinkHierarchyNode>()
                //         .SelectMany(relative => relative
                //             .HierarchyNodes
                //             .SelectMany(IEnumerable<string> (node) =>
                //             {
                //                 var type = relative.IsTemplate 
                //                     ? node.FormattedBackLinkType 
                //                     : $"{node.Target.Actor}{node.FormatRelativeTypePath()}.BackLink<TSource>";
                //                 
                //                 var coreType = relative.IsTemplate 
                //                     ? node.FormattedCoreBackLinkType 
                //                     : $"{node.Target.GetCoreActor()}{node.FormatRelativeTypePath()}.BackLink<TSource>";
                //                 
                //                 var name = LinksV4.GetFriendlyName(node.Target.Actor);
                //
                //                 return [
                //                     $"{type} {node.FormatAsTypePath()}.BackLink<TSource>.{name} => {name}",
                //                     $"{coreType} {Target.GetCoreActor()}{node.FormatRelativeTypePath()}.Hierarchy.{name} => {name}"
                //                 ];
                //             })
                //         )
                // );
                break;
        }

        var backlinkBases = new List<string>()
        {
            FormatAsTypePath()
        };

        //List<(string Type, string Name, string? Default)> baseCtorParameters = [];

        if (Parent is ITypeImplementerNode {WillGenerateImplementation: true} implementer)
        {
            //baseCtorParameters = implementer.ConstructorMembers;

            Parent.GetPathGenerics(out var parentGenerics, out _);

            backlinkBases.Insert(0, $"{Target.Actor}.{implementer.ImplementationClassName}{(
                parentGenerics.Count > 0
                    ? $"<{string.Join(", ", parentGenerics)}>"
                    : string.Empty
            )}");
        }
        else if (RootActorNode.WillGenerateImplementation)
        {
            backlinkBases.Insert(0, $"{Target.Actor}.{RootActorNode.ImplementationClassName}");
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

        if (Constructor is not null)
            backLinkMembers.Add(Constructor.Format());

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
              )}}) => new {{Target.Actor}}.{{typeName}}<{{string.Join(", ", generics)}}>({{(
              ctorParams.Count > 0
                  ? string.Join(", ", ctorParams.Select(x => x.Name))
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