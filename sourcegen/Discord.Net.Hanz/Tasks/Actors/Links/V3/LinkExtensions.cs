using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

public static class LinkExtensions
{
    public class Extension(string name, INamedTypeSymbol symbol, List<ExtensionProperty> properties)
    {
        public string Name { get; } = name;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public List<ExtensionProperty> Properties { get; } = properties;
    }

    public class ExtensionProperty(
        IPropertySymbol symbol,
        bool isLinkMirror,
        bool isBackLinkMirror,
        LinksV3.Target? propertyTypeTarget)
    {
        public string Name => MemberUtils.GetMemberName(Symbol);
        public IPropertySymbol Symbol { get; } = symbol;
        public LinksV3.Target? PropertyTypeTarget { get; } = propertyTypeTarget;
        public bool IsLinkMirror { get; } = isLinkMirror;
        public bool IsBackLinkMirror { get; } = isBackLinkMirror;
    }

    public static string FormatExtensions(
        ImmutableList<LinkSchematics.Entry> path,
        LinksV3.Target target,
        Logger logger)
    {
        if (target.Extensions is null || target.Extensions.Count == 0) return string.Empty;

        var extensionPath = path.Select(x => LinksV3.FormatTypeName(x.Symbol)).ToImmutableList();
        
        return string.Join(
            Environment.NewLine,
            target.Extensions
                .Select(x =>
                    BuildExtension(
                        target,
                        x,
                        target.Extensions.ToImmutableList(),
                        extensionPath,
                        extensionPath,
                        logger
                    )
                )
        );
    }

    private static string BuildExtension(
        LinksV3.Target target,
        Extension extension,
        ImmutableList<Extension> extensions,
        ImmutableList<string> path,
        ImmutableList<string> pathWithoutExtensions,
        Logger logger)
    {
        logger.Log($"{target.LinkTarget.Actor}: Building extension {extension.Name} ({string.Join(".", path)})");
        
        var nextExtensions = extensions.Remove(extension);
        var nextPath = path.Add(extension.Name);
        var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core ? "interface" : "class";

        var bases = new List<string>();

        if (path.Count > 0)
        {
            bases.AddRange([
                $"{target.LinkTarget.Actor}.{string.Join(".", path)}",
                $"{target.LinkTarget.Actor}.{extension.Name}"
            ]);
        }
        
        return
            $$"""
              public {{kind}} {{extension.Name}}{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}    {string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}"
                      : string.Empty
              )}}
              {
                  {{
                      string
                          .Join(
                              Environment.NewLine,
                              extension.Properties.Select(x =>
                                  FormatExtensionProperty(target, pathWithoutExtensions, extension, x, logger)
                              )
                          )
                          .WithNewlinePadding(4)
                  }}
                  {{
                      string
                          .Join(
                              Environment.NewLine,
                              nextExtensions
                                  .Select(x =>
                                      BuildExtension(target, x, nextExtensions, nextPath, pathWithoutExtensions, logger)
                                  )
                          )
                          .WithNewlinePadding(4)
                  }}
                  {{
                      LinksV3
                          .BuildBackLink(
                              target,
                              pathWithoutExtensions,
                              string
                                  .Join(
                                      Environment.NewLine,
                                      extension.Properties.Select(x =>
                                          FormatExtensionProperty(target, pathWithoutExtensions, extension, x, logger, true)
                                      )
                                  )
                                  .WithNewlinePadding(4)
                          )
                          .WithNewlinePadding(4)
                  }}
              }
              """;
    }

    private static string FormatExtensionProperty(
        LinksV3.Target target,
        ImmutableList<string> path,
        Extension extension,
        ExtensionProperty property,
        Logger logger,
        bool isBackLink = false)
    {
        if (property.IsBackLinkMirror)
        {
            if (!isBackLink)
            {
                return string.Empty;
            }

            if (property.PropertyTypeTarget is null)
            {
                logger.Warn($"{target.LinkTarget.Actor}: {extension.Name}.{property.Name} -> no property target on backlink mirror");
                return string.Empty;
            }

            return
                $$"""
                  {{property.PropertyTypeTarget.LinkTarget.Actor}}.BackLink<TSource> {{property.Name}} { get; }
                  """;
        }

        if (property.IsLinkMirror)
        {
            if (property.PropertyTypeTarget is null)
            {
                logger.Warn($"{target.LinkTarget.Actor}: {extension.Name}.{property.Name} -> no property target on link mirror");
                return string.Empty;
            }

            if (path.Count == 0)
            {
                return $"{property.PropertyTypeTarget.FormattedCoreLink} {property.Name} {{ get; }}";
            }
            
            return
                $$"""
                  new {{property.PropertyTypeTarget.LinkTarget.Actor}}{{LinksV3.FormatPath(path)}} {{property.Name}} { get; }
                  {{property.PropertyTypeTarget.FormattedCoreLink}} {{target.LinkTarget.Actor}}.{{extension.Name}}.{{property.Name}} => {{property.Name}};
                  """;
        }

        if (path.Count == 0)
        {
            return
                $"{property.Symbol.Type} {property.Name} {{ get; }}";
        }
        
        // TODO: in non-core, we need the implementation
        return string.Empty;
    }

    public static List<Extension>? GetExtensions(
        LinksV3.Target target,
        Func<INamedTypeSymbol, LinksV3.Target?> getTarget,
        Logger logger)
    {
        var extensionTypes = target.LinkTarget.GetCoreActor().GetTypeMembers()
            .Where(x => x
                .GetAttributes()
                .Any(x => x.AttributeClass?.Name == "LinkExtensionAttribute")
            )
            .ToArray();

        if (extensionTypes.Length == 0) return null;

        return extensionTypes
            .Select(x => ParseExtension(target, x, getTarget, logger))
            .OfType<Extension>()
            .ToList();
    }

    private static Extension? ParseExtension(
        LinksV3.Target target,
        INamedTypeSymbol symbol,
        Func<INamedTypeSymbol, LinksV3.Target?> getTarget,
        Logger logger)
    {
        var properties = new List<ExtensionProperty>();

        foreach (var propertySymbol in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            var attribute = propertySymbol.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "LinkMirrorAttribute");

            var propertyTarget = propertySymbol.Type is INamedTypeSymbol named ? getTarget(named) : null;
            
            if (
                propertySymbol.Type.TypeKind is TypeKind.Unknown &&
                propertyTarget is null)
            {
                var propTypeStr = propertySymbol.Type.ToDisplayString();
                var prefix = target.LinkTarget.Assembly.ToString();
                
                var actorPart = propTypeStr
                    .Split(['.'], StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(x => x.StartsWith(prefix) && (x.EndsWith("Actor") || x.EndsWith("Trait")));

                if (actorPart is not null)
                {
                    var actorSymbol = target.Schematic.Semantic.Compilation
                        .GetTypeByMetadataName(
                            $"Discord{(prefix is not "Core" ? $".{prefix}" : string.Empty)}.{actorPart}"
                        );

                    if (actorSymbol is not null)
                        propertyTarget = getTarget(actorSymbol);
                }

                if (
                    propTypeStr.StartsWith("Indexable") ||
                    propTypeStr.StartsWith("Enumerable") ||
                    propTypeStr.StartsWith("Defined") ||
                    propTypeStr.StartsWith("Paged<")
                )
                {
                    propertyTarget = target;
                }
            }
            
            properties.Add(new ExtensionProperty(
                propertySymbol,
                attribute is not null,
                attribute?.NamedArguments
                    .FirstOrDefault(x => x.Key == "OnlyBackLinks")
                    .Value.Value as bool? == true,
                propertyTarget
            ));
        }

        return properties.Count == 0
            ? null
            : new(
                symbol.Name.Replace("Extension", string.Empty),
                symbol,
                properties
            );
    }
}