using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class EnumerableNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        var ancestors = GetEntityAssignableAncestors(context);
        
        var extraParameters = GetExtraParameters(Target);
        
        if (ancestors.Length == 0 && extraParameters.Count == 0) return;
        
        members.AddRange([
            $"new ITask<IReadOnlyCollection<{Target.Entity}>> AllAsync({FormatExtraParameters(extraParameters)}RequestOptions? options = null, CancellationToken token = default);",
            $"ITask<IReadOnlyCollection<{Target.Entity}>> {FormattedLinkType}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options: options, token: token);",
        ]);
        
        if (ParentLinks.Any())
        {
            var formattedInvocationParameters = extraParameters.Count == 0
                ? string.Empty
                : $"{string.Join(", ", extraParameters.Select(x => $"{x.Name}: {x.Name}"))}, ";
        
            members.AddRange([
                $"ITask<IReadOnlyCollection<{Target.Entity}>> {Target.Actor}.{LinksV3.FormatTypeName(Entry.Symbol)}.AllAsync({FormatExtraParameters(extraParameters, defaults: false)}RequestOptions? options, CancellationToken token) => AllAsync({formattedInvocationParameters}options: options, token: token);"
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
                .Where(x => extraParameters.Any(y =>
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

    protected override string CreateImplementation(NodeContext context, Logger logger)
    {
        return string.Empty;
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