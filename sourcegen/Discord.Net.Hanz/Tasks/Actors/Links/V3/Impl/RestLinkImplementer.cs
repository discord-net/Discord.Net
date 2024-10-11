using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Actors.V3.Types;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        //if(path.Count == 0)

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
              internal static {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}.BackLink<TSource> Create(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x => $"{x.Value} {x.Key}")
                      ).WithNewlinePadding(4)
                  }}
              ) => new {{ToRestImplementationName(type, path)}}.{{ToRestImplementationName(type, path)}}BackLink<TSource>(
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

    public string CreateRootBackLink(LinksV3.Target target, Logger logger)
    {
        var members = new List<string>();

        var ctors = target.LinkTarget.Actor
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => x.MethodKind is MethodKind.Constructor);


        foreach (var methodSymbol in ctors)
        {
            var access = methodSymbol.DeclaredAccessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                _ => "internal"
            };

            var parameters = methodSymbol.Parameters.Length > 0
                ? $",{Environment.NewLine}{
                    string.Join(
                        $",{Environment.NewLine}",
                        methodSymbol
                            .Parameters
                            .Select(x =>
                                $"{x.Type} {x.Name}{(
                                    x.HasExplicitDefaultValue
                                        ? $" = {SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)}"
                                        : string.Empty
                                )}"
                            )
                    )
                }".WithNewlinePadding(4)
                : string.Empty;

            var baseArgs = methodSymbol.Parameters.Length > 0
                ? string.Join(
                    $", ",
                    methodSymbol.Parameters.Select(x => x.Name)
                )
                : string.Empty;

            members.Add(
                $$"""
                  {{access}} BackLink(
                      TSource source{{parameters}}
                  ) : base({{baseArgs}})
                  {
                      Source = source;
                  }
                  """
            );
        }

        members.AddRange([
            $"TSource {target.FormattedCoreBackLinkType}.Source => Source;",
            $"TSource {target.FormattedBackLinkType}.Source => Source;",
            $"{target.LinkTarget.GetCoreActor()} {target.FormattedCoreActorProvider}.GetActor({target.LinkTarget.Id} id) => this;",
            $"{target.LinkTarget.Actor} {target.FormattedActorProvider}.GetActor({target.LinkTarget.Id} id) => this;",
        ]);

        return
            $$"""
              internal sealed class BackLink<TSource> : 
                  {{target.LinkTarget.Actor}},
                  {{target.LinkTarget.GetCoreActor()}}.BackLink<TSource>,
                  {{target.FormattedBackLinkType}}
                  where TSource : class, IPathable
              {
                  internal TSource Source { get; }
              
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
              }
              """;
    }

    public ConstructorRequirements? ImplementHierarchy()
    {
        throw new NotImplementedException();
    }

    public string ImplementExtension(
        LinksV3.Target target,
        LinkExtensions.Extension extension,
        ImmutableList<LinkExtensions.Extension> children,
        ImmutableList<LinkExtensions.Extension> parents,
        ImmutableList<string> path,
        ImmutableList<string> pathWithoutExtensions,
        Logger logger)
    {
        var ident = $"__RestExt{string.Join(string.Empty, path)}{extension.Name}";

        var bases = new List<string>()
        {
            $"{target.LinkTarget.Actor}{LinksV3.FormatPath(path)}.{extension.Name}"
        };

        if (parents.Count == 0)
        {
            if (pathWithoutExtensions.Count > 0)
            {
                bases.Insert(0, $"{target.LinkTarget.Actor}{LinksV3.FormatPath(pathWithoutExtensions)}.__RestLink{string.Join(string.Empty, pathWithoutExtensions)}");
            }
        }
        else
        {
            bases.Insert(0, $"{target.LinkTarget.Actor}{LinksV3.FormatPath(path)}.__RestExt{string.Join(string.Empty, path)}");
        }
        
        return
            $$"""
            internal class {{ident}} : 
                {{string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}}
            {
                
            }
            """;
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
              internal static {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}} Create(
                  {{
                      string.Join(
                          $",{Environment.NewLine}",
                          ctorParams.Select(x => $"{x.Value} {x.Key}")
                      ).WithNewlinePadding(4)
                  }}
              ) => new {{ToRestImplementationName(type, path)}}(
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

        var hasExtraParameters = constructorRequirements?.MembersWhoNeedInitialization.Count > 0;

        var additionalConstructorArguments = hasExtraParameters
            ? $",{Environment.NewLine}{
                string.Join(
                    $",{Environment.NewLine}",
                    constructorRequirements!.MembersWhoNeedInitialization
                        .Select(x => $"{x.Value} {ToParameterLower(x.Key)}"))
            }".WithNewlinePadding(4)
            : string.Empty;

        var additionalInitializers = hasExtraParameters
            ? $"{Environment.NewLine}{
                string.Join(
                    Environment.NewLine,
                    constructorRequirements!.MembersWhoNeedInitialization.Select(x =>
                        $"{x.Key} = {ToParameterLower(x.Key)};"
                    )
                )
            }".WithNewlinePadding(4)
            : string.Empty;

        var typeName = ToRestImplementationName(type, path);

        members.AddRange([
            $"public Discord{target.LinkTarget.Assembly}Client Client {{ get; }}",
            $"internal {target.FormattedActorProvider} Provider {{ get; }}",
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
        ]);

        members.AddRange([
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
              private class {{typeName}} : 
                  {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}
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
        var additionalArgs = constructorRequirements?.MembersWhoNeedInitialization?.Count > 0
            ? $",{Environment.NewLine}{string.Join(
                $",{Environment.NewLine}",
                constructorRequirements.MembersWhoNeedInitialization
                    .Select(x => $"{x.Value} {x.Key}")
            )}".WithNewlinePadding(8)
            : string.Empty;

        var additionalBaseArgs = constructorRequirements?.MembersWhoNeedInitialization?.Count > 0
            ? $", {string.Join(
                ", ",
                constructorRequirements.MembersWhoNeedInitialization
                    .Keys
            )}".WithNewlinePadding(8)
            : string.Empty;

        var backlinkBaseInterface =
            $"{target.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}.BackLink<TSource>";

        var members = new List<string>();

        members.Add(
            $"TSource {backlinkBaseInterface}.Source => Source;"
        );

        var typeName = $"{ToRestImplementationName(type, path)}BackLink";

        return
            $$"""
              internal sealed class {{typeName}}<TSource> : 
                  {{target.LinkTarget.Actor}}{{LinksV3.FormatPath(path.Add(type))}}.{{ToRestImplementationName(type, path)}},
                  {{backlinkBaseInterface}}
                  where TSource : class, IPathable
              {
                  internal TSource Source { get; }
              
                  public {{typeName}}(
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

    private static string ToRestImplementationName(
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        var sb = new StringBuilder();

        foreach (var entry in path.Add(type))
        {
            sb.Append(entry.Symbol.Name);
        }

        return $"__RestLink{sb}";
    }
}