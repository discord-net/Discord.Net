using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Enumerable : ILinkTypeProcessor
{
    public ConstructorRequirements AddImplementation(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        var fetchableOfManyInterface = target.LinkTarget.GetCoreEntity()
            .AllInterfaces
            .FirstOrDefault(x => x.Name == "IFetchableOfMany");

        var canUseFetchable =
            fetchableOfManyInterface?
                .TypeArguments
                .ElementAtOrDefault(1)?
                .Equals(target.LinkTarget.Model, SymbolEqualityComparer.Default)
            ?? false;

        var overrideTarget = target.EntityAssignableAncestors.Count > 0
            ? $"{target.LinkTarget.GetCoreActor()}{LinksV3.FormatPath(path.Add(type))}"
            : $"{target.FormattedCoreLinkType}.Enumerable";

        var apiProviderType = $"ApiModelProviderDelegate<IEnumerable<{target.LinkTarget.Model}>>";

        var extraArgs = GetExtraParameters(target);
        var formattedExtraArgs = FormatExtraParameters(extraArgs);
        var formattedInvocationParameters = extraArgs.Count == 0
            ? string.Empty
            : $", {string.Join(", ", extraArgs.Select(x => $"{x.Name}: {x.Name}"))}";

        var fetchModels = canUseFetchable
            ? $"await {target.LinkTarget.GetCoreEntity()}.FetchManyRoute(_path{formattedInvocationParameters}).AsRequiredProvider()(Client, options, token)"
            : "await _apiProvider(Client, options, token);";

        members.AddRange([
            canUseFetchable 
                ? "private readonly IPathable _path;"
                : $"private readonly {apiProviderType} _apiProvider;",
            $$"""
              public async ITask<IReadOnlyCollection<{{target.LinkTarget.Entity}}>> AllAsync({{formattedExtraArgs}}RequestOptions? options = null, CancellationToken token = default)
              {
                  var models = {{fetchModels}}
                  
                  if (models is null) return [];
                  
                  return models.Select(CreateEntity).ToList().AsReadOnly();
              }
              """,
            $"ITask<IReadOnlyCollection<{target.LinkTarget.GetCoreEntity()}>> {overrideTarget}.AllAsync(RequestOptions? options, CancellationToken token = default) => AllAsync(options, token);"
        ]);

//         members.Add(
//             canUseFetchable
//                 ? $$"""
//                     public Enumerable(
//                         Discord{{target.LinkTarget.Assembly}}Client client,
//                         {{target.FormattedActorProvider}} provider,
//                         IPathable path)
//                     { 
//                         Client = client;
//                         Provider = provider;
//                         
//                         _path = path;
//                     }    
//                     """
//                 : $$"""
//                     public Enumerable(
//                         Discord{{target.LinkTarget.Assembly}}Client client,
//                         {{target.FormattedActorProvider}} provider,
//                         {{apiProviderType}} apiProvider)
//                     {
//                         Client = client;
//                         Provider = provider;
//                         _apiProvider = apiProvider;
//                     }    
//                     """
//         );

        return canUseFetchable
            ? new ConstructorRequirements().Require("_path", "IPathable")
            : new ConstructorRequirements().Require("_apiProvider", apiProviderType);
    }

    public void AddOverrideMembers(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        var extraParameters = GetExtraParameters(target);

        if (target.EntityAssignableAncestors.Count == 0 && extraParameters.Count == 0) return;

        members.AddRange([
            $"new ITask<IReadOnlyCollection<{target.LinkTarget.Entity}>> AllAsync({FormatExtraParameters(extraParameters)}RequestOptions? options = null, CancellationToken token = default);",
            $"ITask<IReadOnlyCollection<{target.LinkTarget.Entity}>> {target.FormattedLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options: options, token: token);",
        ]);

        if (path.Count > 0)
        {
            var formattedInvocationParameters = extraParameters.Count == 0
                ? string.Empty
                : $"{string.Join(", ", extraParameters.Select(x => $"{x.Name}: {x.Name}"))}, ";

            members.AddRange([
                $"ITask<IReadOnlyCollection<{target.LinkTarget.Entity}>> {target.LinkTarget.Actor}.{LinksV3.FormatTypeName(type.Symbol)}.AllAsync({FormatExtraParameters(extraParameters, defaults: false)}RequestOptions? options, CancellationToken token) => AllAsync({formattedInvocationParameters}options: options, token: token);"
            ]);
        }

        foreach (var ancestor in target.EntityAssignableAncestors)
        {
            var overrideType = ancestor.EntityAssignableAncestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedLinkType}.Enumerable";

            var ancestorExtraParameters = GetExtraParameters(ancestor);

            var matching = ancestorExtraParameters
                .Where(x => extraParameters.Any(y =>
                    y.Name == x.Name &&
                    y.Type.Equals(x.Type, SymbolEqualityComparer.Default))
                )
                .ToList();

            var formattedAncestorInvocationParameters = matching.Count == 0
                ? string.Empty
                : $"{string.Join(", ", matching.Select(x => $"{x.Name}: {x.Name}"))}, ";

            members.AddRange([
                $"ITask<IReadOnlyCollection<{ancestor.LinkTarget.Entity}>> {overrideType}.AllAsync({FormatExtraParameters(ancestorExtraParameters, defaults: false)}RequestOptions? options, CancellationToken token) => AllAsync({formattedAncestorInvocationParameters}options: options, token: token);"
            ]);
        }
    }

    private static List<IParameterSymbol> GetExtraParameters(LinksV3.Target target)
    {
        if (target.LinkTarget.Assembly is not LinkActorTargets.AssemblyTarget.Core)
        {
            var fetchableOfManyMethod = target.LinkTarget.GetCoreEntity()
                .GetMembers("FetchManyRoute")
                .OfType<IMethodSymbol>()
                .FirstOrDefault();

            if (fetchableOfManyMethod is null || fetchableOfManyMethod.Parameters.Length == 1) return [];

            return fetchableOfManyMethod
                .Parameters
                .Skip(1)
                .Where(x => x.HasExplicitDefaultValue)
                .ToList();
        }

        var fetchableOfManyAttribute = target.LinkTarget.GetCoreEntity()
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "FetchableOfManyAttribute");

        if (fetchableOfManyAttribute is null) return [];

        if (EntityTraits.GetNameOfArgument(fetchableOfManyAttribute) is not MemberAccessExpressionSyntax
            routeMemberAccess)
            return [];

        var route = EntityTraits.GetRouteSymbol(
            routeMemberAccess,
            target.LinkTarget.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
        );

        return route is IMethodSymbol method && ParseExtraArgs(method, target) is { } extra
            ? extra
            : [];

        static List<IParameterSymbol> ParseExtraArgs(IMethodSymbol symbol, LinksV3.Target target)
        {
            var args = new List<IParameterSymbol>();

            foreach (var parameter in symbol.Parameters)
            {
                var heuristic = parameter.GetAttributes()
                    .FirstOrDefault(x => x.AttributeClass?.Name == "IdHeuristicAttribute");

                if (heuristic is not null)
                {
                    continue;
                }

                if (parameter.Name is "id") continue;

                if (!parameter.HasExplicitDefaultValue) continue;

                args.Add(parameter);
            }

            return args;
        }
    }

    private static string FormatExtraParameters(List<IParameterSymbol> parameters, bool defaults = true)
    {
        if (parameters.Count == 0) return string.Empty;

        return $"{string.Join(", ", parameters
            .Select(x => $"{x.Type} {x.Name}{(defaults ? $" = {SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)}" : string.Empty)}")
        )}, ";
    }

    public string? CreateProvider(LinksV3.Target target, Logger logger)
    {
        return null;
    }
}