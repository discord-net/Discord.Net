using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Indexable : ILinkTypeProcessor
{
    public void AddOverrideMembers(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path)
    {
        members.AddRange([
            $"new {target.LinkTarget.Actor} this[{target.LinkTarget.Id} id] => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);",
            $"new {target.LinkTarget.Actor} Specifically({target.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
        ]);

        foreach (var ancestor in target.Ancestors)
        {
            var overrideType = ancestor.Ancestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedCoreLinkType}.Indexable";

            members.AddRange([
                $"{ancestor.LinkTarget.Actor} {overrideType}.this[{ancestor.LinkTarget.Id} id] => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);",
                $"{ancestor.LinkTarget.Actor} {overrideType}.Specifically({ancestor.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
            ]);
        }
    }
}