using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class EnumerableNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    public List<IParameterSymbol> ExtraParameters { get; } = [];

    public string EnumerableProviderDelegateType
        =>
            $"Func<{(ExtraParameters.Count > 0 ? $"{string.Join(", ", ExtraParameters.Select(x => x.Type))}, " : string.Empty)}{Target.Actor}.Enumerable, RequestOptions?, CancellationToken, ITask<IReadOnlyCollection<{Target.Entity}>>>";

    private protected override void Visit(NodeContext context, Logger logger)
    {
        Properties.Clear();
        ExtraParameters.Clear();

        ExtraParameters.AddRange(GetExtraParameters(Target));
        RedefinesLinkMembers = GetEntityAssignableAncestors(context).Length > 0 || ExtraParameters.Count > 0;

        Properties.Add(new("EnumerableProvider", EnumerableProviderDelegateType));

        base.Visit(context, logger);
    }

    public override string Build(NodeContext context, Logger logger)
    {
        AddDefaultProvider();
        return base.Build(context, logger);
    }

    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        if (!RedefinesLinkMembers) return;

        var ancestors = GetEntityAssignableAncestors(context);

        members.AddRange([
            $"new ITask<IReadOnlyCollection<{Target.Entity}>> AllAsync({FormatExtraParameters(ExtraParameters)}RequestOptions? options = null, CancellationToken token = default);",
            $"ITask<IReadOnlyCollection<{Target.Entity}>> {FormattedLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options: options, token: token);",
        ]);

        if (ParentLinks.Any())
        {
            var formattedInvocationParameters = ExtraParameters.Count == 0
                ? string.Empty
                : $"{string.Join(", ", ExtraParameters.Select(x => $"{x.Name}: {x.Name}"))}, ";

            members.AddRange([
                $"ITask<IReadOnlyCollection<{Target.Entity}>> {Target.Actor}.{LinksV3.FormatTypeName(Entry.Symbol)}.AllAsync({FormatExtraParameters(ExtraParameters, defaults: false)}RequestOptions? options, CancellationToken token) => AllAsync({formattedInvocationParameters}options: options, token: token);"
            ]);
        }

        foreach (var ancestor in ancestors)
        {
            var overrideType =
                $"{(
                    ancestor.GetEntityAssignableAncestors(context).Length > 0
                        ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}"
                        : ancestor.FormattedLinkType
                )}.Enumerable";

            var ancestorExtraParameters = GetExtraParameters(ancestor.Target);

            var matching = ancestorExtraParameters
                .Where(x => ExtraParameters.Any(y =>
                    y.Name == x.Name &&
                    y.Type.Equals(x.Type, SymbolEqualityComparer.Default))
                )
                .ToList();

            var formattedAncestorInvocationParameters = matching.Count == 0
                ? string.Empty
                : $"{string.Join(", ", matching.Select(x => $"{x.Name}: {x.Name}"))}, ";

            members.AddRange([
                $"ITask<IReadOnlyCollection<{ancestor.Target.Entity}>> {overrideType}.AllAsync({FormatExtraParameters(ancestorExtraParameters, defaults: false)}RequestOptions? options, CancellationToken token) => AllAsync({formattedAncestorInvocationParameters}options: options, token: token);"
            ]);
        }
    }

    protected override void CreateImplementation(
        List<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        switch (Target.Assembly)
        {
            case LinkActorTargets.AssemblyTarget.Rest:
                CreateRestImplementation(members, bases, context, logger);
                break;
        }
    }

    private void CreateRestImplementation(
        List<string> members,
        List<string> bases,
        NodeContext context,
        Logger logger)
    {
        var memberModifier = Overloads(ImplementationBase)
            ? "override "
            : Overloads(ImplementationChild)
                ? "virtual "
                : string.Empty;

        var formattedExtraParameters = FormatExtraParameters(ExtraParameters);
        var formattedExtraArgs = ExtraParameters.Count > 0
            ? $"{string.Join(", ", ExtraParameters.Select(x => x.Name))}, "
            : string.Empty;

        members.AddRange([
            $"""
             public {memberModifier}ITask<IReadOnlyCollection<{Target.Entity}>> AllAsync({formattedExtraParameters}RequestOptions? options = null, CancellationToken token = default)
                 => EnumerableProvider({formattedExtraArgs}this, options, token);
             """,
            $"""
             ITask<IReadOnlyCollection<{Target.Entity}>> {FormattedLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token)
                 => AllAsync(options: options, token: token);
             """
        ]);

        if (RedefinesLinkMembers)
        {
            members.Add(
                $"""
                 ITask<IReadOnlyCollection<{Target.Entity}>> {FormatAsTypePath()}.AllAsync({formattedExtraArgs}RequestOptions? options, CancellationToken token)
                     => AllAsync({formattedExtraArgs}options: options, token: token);
                 """
            );
        }
        else
        {
            members.AddRange([
                $"""
                 ITask<IReadOnlyCollection<{Target.Entity}>> {FormattedLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token)
                     => AllAsync(options: options, token: token);
                 """,
                $"""
                 ITask<IReadOnlyCollection<{Target.GetCoreEntity()}>> {FormattedCoreLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token)
                     => AllAsync(options: options, token: token);
                 """
            ]);
        }
    }

    private void AddDefaultProvider()
    {
        if (Parent is not ActorNode || RootActorNode is null || IsCore) return;
        
        if (
            Target.GetCoreEntity()
                .Interfaces
                .FirstOrDefault(x =>
                    x.Name is "IFetchableOfMany"
                ) is not { } fetchable
        ) return;

        if (fetchable.TypeArguments[1] is not INamedTypeSymbol routeModel)
            return;

        if (
            !routeModel.Equals(Target.Model, SymbolEqualityComparer.Default) &&
            !Hierarchy.Implements(Target.Model, routeModel)
        ) return;

        var extraParameters = FormatExtraParameters(ExtraParameters, false)
            .Replace(",", $",{Environment.NewLine}")
            .WithNewlinePadding(4);

        RootActorNode.AdditionalTypes.Add(
            $$"""
              internal static async ITask<IReadOnlyCollection<{{Target.Entity}}>> DefaultEnumerableProvider(
                  {{extraParameters}}{{Target.Actor}}.Enumerable link,
                  RequestOptions? options,
                  CancellationToken token)
              {
                  var result = await link.Client.RestApiClient.ExecuteAsync(
                      {{Target.GetCoreEntity()}}.FetchManyRoute(link{{(
                          ExtraParameters.Count > 0
                            ? $", {string.Join(", ", ExtraParameters.Select(x => x.Name))}"
                            : string.Empty
                      )}}),
                      options,
                      token
                  );
                  
                  if(result is null) return [];
                  
                  return result.Select(x => link.CreateEntity(x)).ToList().AsReadOnly();
              }
              """
        );
    }

    private bool Overloads(LinkNode? other)
    {
        if (other is not EnumerableNode otherEnumerable)
            return false;

        if (otherEnumerable.ExtraParameters.Count != ExtraParameters.Count)
            return false;

        for (var i = 0; i < otherEnumerable.ExtraParameters.Count; i++)
        {
            if (!otherEnumerable.ExtraParameters[i].Type
                    .Equals(ExtraParameters[i].Type, SymbolEqualityComparer.Default))
                return false;
        }

        return true;
    }

    private static List<IParameterSymbol> GetExtraParameters(LinkTarget target)
    {
        if (target.Assembly is not LinkActorTargets.AssemblyTarget.Core)
        {
            var fetchableOfManyMethod = target.GetCoreEntity()
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

        var fetchableOfManyAttribute = target.GetCoreEntity()
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "FetchableOfManyAttribute");

        if (fetchableOfManyAttribute is null) return [];

        if (EntityTraits.GetNameOfArgument(fetchableOfManyAttribute) is not MemberAccessExpressionSyntax
            routeMemberAccess)
            return [];

        var route = EntityTraits.GetRouteSymbol(
            routeMemberAccess,
            target.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
        );

        return route is IMethodSymbol method && ParseExtraArgs(method, target) is { } extra
            ? extra
            : [];

        static List<IParameterSymbol> ParseExtraArgs(IMethodSymbol symbol, LinkTarget target)
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
}