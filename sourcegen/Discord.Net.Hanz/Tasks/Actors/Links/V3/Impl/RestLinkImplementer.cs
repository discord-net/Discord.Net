using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.V3.Types;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Impl;

public class RestLinkImplementer : ILinkImplementer
{
    public ConstructorRequirements? ImplementBackLink(
        ConstructorRequirements? requirements,
        List<string> members,
        LinksV3.Target target, 
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path, Logger logger)
    {
        var ctorParams = new Dictionary<string, string>()
        {
            {"source", "TSource"},
            {"client", $"Discord{target.LinkTarget.Assembly}Client"},
            {"provider", target.FormattedActorProvider}
        };

        if (requirements?.MembersWhoNeedInitialization.Count > 0)
        {
            foreach (var entry in requirements.MembersWhoNeedInitialization)
            {
                ctorParams.Add(entry.Key, entry.Value);
            }
        }

        members.Add(
            $$"""
              internal static {{LinksV3.FormatTypeName(type.Symbol)}}.BackLink<TSource> Create(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x => $"{x.Value} {x.Key}")
                      ).WithNewlinePadding(4)
                  }}
              ) => new __RestInternal{{LinksV3.FormatTypeName(type.Symbol)}}.__RestInternalBackLink{{type.Symbol.Name}}<TSource>(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Keys
                      ).WithNewlinePadding(4)
                  }}
              );
              """
            );

        return requirements;
    }

    public ConstructorRequirements? ImplementHierarchy()
    {
        throw new NotImplementedException();
    }

    public ConstructorRequirements? ImplementExtensions()
    {
        throw new NotImplementedException();
    }
    
    public ConstructorRequirements? ImplementLink(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        Logger logger)
    {
        members.AddRange([
            CreateClassImplementation(target, type, path, out var constructorRequirements),
            GenerateCreateMethod(target, type, path, constructorRequirements)
        ]);

        return constructorRequirements;
    }

    private string GenerateCreateMethod(
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        ConstructorRequirements? constructorRequirements)
    {
        var ctorParams = new Dictionary<string, string>()
        {
            {"client", $"Discord{target.LinkTarget.Assembly}Client"},
            {"provider", target.FormattedActorProvider}
        };

        if (constructorRequirements?.MembersWhoNeedInitialization.Count > 0)
        {
            foreach (var entry in constructorRequirements.MembersWhoNeedInitialization)
            {
                ctorParams.Add(entry.Key, entry.Value);
            }
        }

        return
            $$"""
              internal static {{LinksV3.FormatTypeName(type.Symbol)}} Create(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x => $"{x.Value} {x.Key}")
                      ).WithNewlinePadding(4)
                  }}
              ) => new __RestInternal{{LinksV3.FormatTypeName(type.Symbol)}}(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Keys
                      ).WithNewlinePadding(4)
                  }}
              );
              """;
    }

    private string CreateClassImplementation(
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        out ConstructorRequirements? constructorRequirements)
    {
        var members = new List<string>();

        constructorRequirements = null;

        foreach (var entry in path.Add(type))
        {
            if (LinksV3.Processors.TryGetValue(entry.Symbol.Name, out var processor))
                constructorRequirements += processor.AddImplementation(members, target, type, path);
        }

        var additionalConstructorArguments = constructorRequirements?.MembersWhoNeedInitialization.Count > 0
            ? $",{Environment.NewLine}{
                string.Join(
                    $",{Environment.NewLine}",
                    constructorRequirements.MembersWhoNeedInitialization
                        .Select(x => $"{x.Value} {ToParameterLower(x.Key)}"))
            }".WithNewlinePadding(4)
            : string.Empty;

        var additionalInitializers = constructorRequirements?.MembersWhoNeedInitialization.Count > 0
            ? $"{Environment.NewLine}{
                string.Join(
                    Environment.NewLine,
                    constructorRequirements.MembersWhoNeedInitialization.Select(x =>
                        $"{x.Key} = {ToParameterLower(x.Key)};"
                    )
                )
            }".WithNewlinePadding(4)
            : string.Empty;

        var typeName = $"__RestInternal{LinksV3.FormatTypeName(type.Symbol)}";
        
        members.Add(
            $$"""
              public __RestInternal{{type.Symbol.Name}}(
                  Discord{{target.LinkTarget.Assembly}}Client client,
                  {{target.FormattedActorProvider}} provider{{additionalConstructorArguments}}
              )
              {
                  Client = client;
                  Provider = provider;{{additionalInitializers}}
              }
              """
        );

        if (target.EntityAssignableAncestors.Count == 0)
            members.Add($"public Discord{target.LinkTarget.Assembly}Client Client {{ get; }}");

        members.AddRange([
            $"internal {target.FormattedActorProvider} Provider {{ get; }}",
            $"{target.FormattedActorProvider} {target.FormattedRestLinkType}.Provider => Provider;",
            $"{target.LinkTarget.GetCoreActor()} {target.FormattedCoreActorProvider}.GetActor({target.LinkTarget.Id} id) => Provider.GetActor(id);"
        ]);

        if (target.LinkTarget.Model.AllInterfaces.Any(v => v.Name == "IEntityModel"))
        {
            members.AddRange([
                $"internal {(target.Ancestors.Count > 0 ? "new " : string.Empty)}{target.LinkTarget.Entity} CreateEntity({target.LinkTarget.Model} model) => Provider.GetActor(model.Id).CreateEntity(model);",
                $"{target.LinkTarget.Entity} {target.FormattedEntityProvider}.CreateEntity({target.LinkTarget.Model} model) => CreateEntity(model);",
                $"{target.LinkTarget.GetCoreEntity()} {target.FormattedCoreEntityProvider}.CreateEntity({target.LinkTarget.Model} model) => CreateEntity(model);"
            ]);
        }

        members.Add(CreateBackLink(target, type, path, constructorRequirements));
        
        return
            $$"""
              private class __RestInternal{{LinksV3.FormatTypeName(type.Symbol)}} : 
                  {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}{{(
                      type.Syntax.ConstraintClauses.Count > 0
                          ? $"{Environment.NewLine}    {string.Join(Environment.NewLine, type.Syntax.ConstraintClauses).WithNewlinePadding(4)}"
                          : string.Empty
                  )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private static string CreateBackLink(
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        ConstructorRequirements? constructorRequirements)
    {
        var ident = LinksV3.FormatTypeName(type.Symbol);

        var additionalArgs = constructorRequirements?.MembersWhoNeedInitialization?.Count > 0
            ? $",{Environment.NewLine}{string.Join(
                $",{Environment.NewLine}",
                constructorRequirements.MembersWhoNeedInitialization
                    .Select(x => $"{x.Value} {x.Key}")
            )}".WithNewlinePadding(4)
            : string.Empty;
        
        var additionalBaseArgs = constructorRequirements?.MembersWhoNeedInitialization?.Count > 0
            ? $",{string.Join(
                ", ",
                constructorRequirements.MembersWhoNeedInitialization
                    .Keys
            )}".WithNewlinePadding(4)
            : string.Empty;

        var backlinkBaseInterface =
            $"{target.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}.BackLink<TSource>";
                    
        var members = new List<string>();

        members.Add(
            $"TSource {backlinkBaseInterface}.Source => Source;"
        );
        
        return 
            $$"""
            internal sealed class __RestInternalBackLink{{type.Symbol.Name}}<TSource> : 
                {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}.__RestInternal{{ident}},
                {{backlinkBaseInterface}}
                where TSource : class, IPathable
            {
                internal TSource Source { get; }
            
                public __RestInternalBackLink{{type.Symbol.Name}}(
                    TSource source, 
                    DiscordRestClient client,
                    {{target.FormattedActorProvider}} provider{{additionalArgs}}
                ) : base(client, provider{{additionalBaseArgs}})
                {
                    Source = source;
                }
                
                {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
            }
            """;
    }
    
    private static string ToParameterLower(string name)
    {
        if (name.StartsWith("_"))
            name = name.Substring(1);

        return $"{char.ToLower(name[0])}{name.Substring(1)}";
    }
}