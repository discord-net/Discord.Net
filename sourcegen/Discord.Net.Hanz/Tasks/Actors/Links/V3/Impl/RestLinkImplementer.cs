using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.V3.Types;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Impl;

public class RestLinkImplementer : ILinkImplementer
{
    public void ImplementBackLink(
        List<string> members,
        LinksV3.Target target, 
        ImmutableList<string> path, 
        ImmutableList<string>? ancestorPath = null,
        string? extraMembers = null)
    {
//         var typeName = $"__Rest{LinksV3.GetFriendlyName(target.LinkTarget.Actor)}BackLink{path.Count}";
//         
//         members.Add(
//             $$"""
//               public static void 
//               """
//         );

    }
    
    public void Implement(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        members.AddRange([
            CreateClassImplementation(target, type, path, out var constructorRequirements),
            GenerateCreateMethod(target, type, path, constructorRequirements)
        ]);
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
              public {{typeName}}(
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

        return
            $$"""
              private sealed class __RestInternal{{LinksV3.FormatTypeName(type.Symbol)}} : 
                  {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    private static string CreateBackLink()
    {
        
    }
    
    private static string ToParameterLower(string name)
    {
        if (name.StartsWith("_"))
            name = name.Substring(1);

        return $"{char.ToLower(name[0])}{name.Substring(1)}";
    }
}