using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class IndexableNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        var ancestors = GetEntityAssignableAncestors(context);
        
        if (ancestors.Length == 0) return;
        
        members.AddRange([
            $"new {Target.Actor} this[{Target.Id} id] => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);",
            $"new {Target.Actor} Specifically({Target.Id} id) => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);"
        ]);

        if (Parent is LinkTypeNode)
        {
            members.AddRange([
                $"{Target.Actor} {Target.Actor}.{LinksV4.FormatTypeName(Entry.Symbol)}.this[{Target.Id} id] => this[id];",
                $"{Target.Actor} {Target.Actor}.{LinksV4.FormatTypeName(Entry.Symbol)}.Specifically({Target.Id} id) => Specifically(id);"
            ]);
        }

        foreach (var ancestor in ancestors)
        {
            var overrideType =
                $"{(ancestor.GetEntityAssignableAncestors(context).Length > 0 ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}" : ancestor.FormattedLinkType)}.Indexable";

            members.AddRange([
                $"{ancestor.Target.Actor} {overrideType}.this[{ancestor.Target.Id} id] => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);",
                $"{ancestor.Target.Actor} {overrideType}.Specifically({ancestor.Target.Id} id) => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);"
            ]);
        }
    }

    protected override string CreateImplementation(NodeContext context, Logger logger)
    {
        return string.Empty;
    }
}