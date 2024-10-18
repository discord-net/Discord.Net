using Discord.Net.Hanz.Tasks.Actors.Links.V4.SourceTypes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using LinkTarget = Discord.Net.Hanz.Tasks.Actors.V3.LinkActorTargets.GenerationTarget;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes.Types;

public class DefinedNode(LinkTarget target, LinkSchematics.Entry entry) : LinkTypeNode(target, entry)
{
    private protected override void Visit(NodeContext context, Logger logger)
    {
        Properties.Clear();
        Properties.Add(new Property("Ids", $"IReadOnlyCollection<{Target.Id}>"));
        
        base.Visit(context, logger);
        
        RedefinesLinkMembers = Ancestors.Count > 0;
    }

    protected override void AddMembers(List<string> members, NodeContext context, Logger logger)
    {
        if (!IsTemplate || !RedefinesLinkMembers) return;
        
        members.AddRange([
            $"new IReadOnlyCollection<{Target.Id}> Ids {{ get; }}",
            $"IReadOnlyCollection<{Target.Id}> {FormattedLinkType}.Defined.Ids => Ids;",
        ]);

        if (ParentLinks.Any())
        {
            members.AddRange([
                $"IReadOnlyCollection<{Target.Id}> {Target.Actor}.{LinksV4.FormatTypeName(Entry.Symbol)}.Ids => Ids;"
            ]);
        }

        foreach (var ancestor in Ancestors)
        {
            var overrideType =
                $"{(
                    ancestor.Ancestors.Count > 0
                        ? $"{ancestor.Target.Actor}{FormatRelativeTypePath()}"
                        : ancestor.FormattedLinkType
                )}.Defined";


            members.AddRange([
                $"IReadOnlyCollection<{ancestor.Target.Id}> {overrideType}.Ids => Ids;"
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
        var overrideType = RedefinesLinkMembers ? FormatAsTypePath() : $"{FormattedCoreLinkType}.Defined";
        members.AddRange([
            $"IReadOnlyCollection<{Target.Id}> {FormattedLinkType}.Defined.Ids => Ids;",
            $"IReadOnlyCollection<{Target.Id}> {overrideType}.Ids => Ids;"
        ]);
    }
}