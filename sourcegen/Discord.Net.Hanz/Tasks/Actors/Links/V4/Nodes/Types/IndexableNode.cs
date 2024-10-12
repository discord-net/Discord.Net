using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class IndexableNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    private protected override void Visit(NodeContext context, Logger logger)
    {
        RedefinesLinkMembers = GetEntityAssignableAncestors(context).Length > 0;
        
        base.Visit(context, logger);
    }

    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        if(!RedefinesLinkMembers) return;

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

        foreach (var ancestor in GetEntityAssignableAncestors(context))
        {
            var overrideType =
                $"{(ancestor.GetEntityAssignableAncestors(context).Length > 0 ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}" : ancestor.FormattedLinkType)}.Indexable";

            members.AddRange([
                $"{ancestor.Target.Actor} {overrideType}.this[{ancestor.Target.Id} id] => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);",
                $"{ancestor.Target.Actor} {overrideType}.Specifically({ancestor.Target.Id} id) => (this as IActorProvider<{Target.Actor}, {Target.Id}>).GetActor(id);"
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
        var memberModifier = ImplementationBase is not null
            ? "override "
            : ImplementationChild is not null
                ? "virtual "
                : string.Empty;

        members.AddRange([
            $"public {memberModifier}{Target.Actor} this[{Target.Id} id] => GetActor(id);",
            $"public {memberModifier}{Target.Actor} Specifically({Target.Id} id) => GetActor(id);"
        ]);
    }
}