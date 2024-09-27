using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Enumerable : ILinkTypeProcessor
{
    public void AddOverrideMembers(List<string> members, LinksV3.Target target, LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        members.AddRange([
            $"new ITask<IReadOnlyCollection<{target.LinkTarget.Entity}>> AllAsync(RequestOptions? options = null, CancellationToken token = default);",
            $"ITask<IReadOnlyCollection<{target.LinkTarget.Entity}>> {target.FormattedCoreLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options, token);",
        ]);

        foreach (var ancestor in target.Ancestors)
        {
            var overrideType = ancestor.Ancestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedCoreLinkType}.Enumerable";

            members.AddRange([
                $"ITask<IReadOnlyCollection<{ancestor.LinkTarget.Entity}>> {overrideType}.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options, token);"
            ]);
        }
    }

    public string? CreateProvider(LinksV3.Target target, Logger logger)
    {
        return null;
        
        var entityAttributes = target.LinkTarget.GetCoreEntity().GetAttributes();

        var fetchableOfMany = entityAttributes
            .Where(x => x.AttributeClass?.Name == "FetchableOfManyAttribute")
            .ToArray();

        if (fetchableOfMany.Length == 0) return null;

        var results = new List<string>();
        
        foreach (var attribute in fetchableOfMany)
        {
            if (EntityTraits.GetNameOfArgument(attribute) is not MemberAccessExpressionSyntax routeMemberAccess)
                continue;

            var route = EntityTraits.GetRouteSymbol(
                routeMemberAccess,
                target.LinkTarget.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
            );

            if (route is null) continue;
            
            results.Add(GenerateFetchableProvider(route, target, logger));
        }

        if (results.Count == 0) return null;

        return string.Join(Environment.NewLine, results);
    }

    private static string GenerateFetchableProvider(
        ISymbol route, 
        LinksV3.Target target,
        Logger logger)
    {
        var name = MemberUtils.GetMemberName(route);

        logger.Log($"{target.LinkTarget.Actor}: fetchable {name}");

        var extraArgs = route is IMethodSymbol method && ParseExtraArgs(method, target) is { } extra
            ? extra
            : [];

        return
            $$"""
              public class {{name}}EnumerableProvider : Discord.IEnumerableLinkProvider<{{target.LinkTarget.Entity}}>
              {
                  private readonly {{target.FormattedCoreLink}} _link;
                  private readonly IApiOutRoute<IEnumerable<{{target.LinkTarget.Model}}>> _route;
                  
                  internal {{name}}EnumerableProvider(
                      {{target.FormattedCoreLink}} link,
                      IApiOutRoute<IEnumerable<{{target.LinkTarget.Model}}>> route)
                  {
                      _link = link;
                      _route = route;
                  }
              
                  public async ITask<IReadOnlyCollection<{{target.LinkTarget.Entity}}>> AllAsync(
                      RequestOptions? options = null, 
                      CancellationToken token = default)
                  {
                      var models = await _link.Client.RestApiClient.ExecuteAsync(
                          _route,
                          options,
                          token
                      );
                      
                      if (models is null) return [];
                      
                      return models
                          .Select(model => _link.GetActor(model.Id).CreateEntity(model))
                          .ToList()
                          .AsReadOnly();
                  }
                  
                  public static {{name}}EnumerableProvider Create({{target.FormattedCoreLink}} link{{(
                      extraArgs.Count > 0
                        ? $", {string.Join(", ", extraArgs.Select(x => $"{x.Type} {x.Name} = {SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)}"))}"
                        : string.Empty
                  )}})
                  {
                      return new(link, )
                  }
              }
              """;
    }

    private static List<IParameterSymbol>? ParseExtraArgs(IMethodSymbol method, LinksV3.Target target)
    {
        var args = new List<IParameterSymbol>();

        foreach (var parameter in method.Parameters)
        {
            var heuristic = parameter.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "IdHeuristicAttribute");

            if (heuristic is not null)
            {
                continue;
            }
            
            if(parameter.Name is "id") continue;
            
            if(!parameter.HasExplicitDefaultValue) continue;
            
            args.Add(parameter);
        }
        
        return args.Count > 0 ? args : null;
    }
}