using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

public class LinkHierarchy
{
    public static string BuildHierarchy(
        LinksV3.Target target,
        ImmutableList<string> path)
    {
        if (target.Hierarchy is null || target.Hierarchy.Length == 0) return string.Empty;

        var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core ? "interface" : "class";

        var bases = new List<string>();

        if (path.Count > 0)
            bases.AddRange([
                $"{target.LinkTarget.Actor}{LinksV3.FormatPath(path)}",
                $"{target.LinkTarget.Actor}.Hierarchy"
            ]);

        return
            $$"""
              public {{kind}} Hierarchy{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}    {string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}"
                      : string.Empty
              )}}
              {
                  {{
                      FormatMembers().WithNewlinePadding(4)
                  }}
                  {{
                      LinksV3
                          .BuildBackLink(target, path.Add("Hierarchy"), path, extraMembers: FormatMembers(true))
                          .WithNewlinePadding(4)
                  }}
              }
              """;

        string FormatMembers(bool backlink = false)
        {
            if (backlink && path.Count == 0) return string.Empty;
            
            return string
                .Join(
                    Environment.NewLine,
                    target.Hierarchy!
                        .Select(x =>
                            (
                                Type: path.Count > 0
                                    ? $"{x.LinkTarget.Actor}{LinksV3.FormatPath(path)}"
                                    : x.FormattedCoreLink,
                                Target: x
                            )
                        )
                        .Select(x =>
                        {
                            var type = backlink && path.Count > 0 ? $"{x.Type}.BackLink<TSource>" : x.Type;
                            var name = LinksV3.GetFriendlyName(x.Target.LinkTarget.Actor);
                            if (path.Count == 0)
                                return $"{type} {name} {{ get; }}";

                            var overrideType = backlink && path.Count > 0 ? x.Type : x.Target.FormattedCoreLink;
                            return
                                $$"""
                                  new {{type}} {{name}} { get; }
                                  {{overrideType}} {{target.LinkTarget.Actor}}{{(backlink ? LinksV3.FormatPath(path) : string.Empty)}}.Hierarchy.{{name}} => {{name}};
                                  """;
                        })
                )
                .WithNewlinePadding(4);
        }
    }

    public static LinkActorTargets.GenerationTarget[]? GetHierarchy(
        LinksV3.Target target,
        ImmutableArray<LinkActorTargets.GenerationTarget> targets,
        Logger logger)
    {
        var hierarchyAttribute =
            target.LinkTarget.GetCoreActor()
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");

        if (hierarchyAttribute is null) return null;

        var types = hierarchyAttribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Types")
            .Value;

        if (types.Kind is TypedConstantKind.Array)
        {
            foreach (var t in types.Values)
            {
                logger.Log($"- {t.Kind} : {t.Value?.GetType()}");
            }
        }

        var children = types.Kind is not TypedConstantKind.Error
            ? (
                types.Kind switch
                {
                    TypedConstantKind.Array => types.Values.Select(x => (INamedTypeSymbol) x.Value!),
                    _ => (INamedTypeSymbol[]) types.Value!
                }
            )
            .Select(x => targets
                .FirstOrDefault(y =>
                    y is not null
                    &&
                    y.GetCoreActor().Equals(x, SymbolEqualityComparer.Default)
                )
            )
            .OfType<LinkActorTargets.GenerationTarget>()
            .ToArray()
            : targets
                .Where(x =>
                    x is not null
                    &&
                    Hierarchy.Implements(x.GetCoreActor(), target.LinkTarget.GetCoreActor()))
                .ToArray();

        logger.Log($"{target.LinkTarget.Actor}: {children.Length} hierarchical link targets");

        return children;
    }
}