using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class DefinedNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        var ancestors = GetEntityAssignableAncestors(context);

        if (ancestors.Length == 0) return;

        members.AddRange([
            $"new IReadOnlyCollection<{Target.Id}> Ids {{ get; }}",
            $"IReadOnlyCollection<{Target.Id}> {FormattedLinkType}.Defined.Ids => Ids;",
        ]);

        if (ParentLinks.Any())
        {
            members.AddRange([
                $"IReadOnlyCollection<{Target.Id}> {Target.Actor}.{LinksV3.FormatTypeName(Entry.Symbol)}.Ids => Ids;"
            ]);
        }

        foreach (var ancestor in ancestors)
        {
            var overrideType =
                $"{(
                    ancestor.GetEntityAssignableAncestors(context).Length > 0
                        ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}"
                        : ancestor.FormattedLinkType
                )}.Defined";


            members.AddRange([
                $"IReadOnlyCollection<{ancestor.Target.Id}> {overrideType}.Ids => Ids;"
            ]);
        }
    }

    protected override string CreateImplementation(NodeContext context, Logger logger)
    {
        return string.Empty;
    }
}