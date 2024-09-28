using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Defined : ILinkTypeProcessor
{
    public void AddOverrideMembers(List<string> members, LinksV3.Target target, LinkSchematics.Entry type, ImmutableList<LinkSchematics.Entry> path)
    {
        members.AddRange([
            $"new IReadOnlyCollection<{target.LinkTarget.Id}> Ids {{ get; }}",
            $"IReadOnlyCollection<{target.LinkTarget.Id}> {target.FormattedCoreLinkType}.Defined.Ids => Ids;",
        ]);
        
        if (path.Count > 0)
        {
            members.AddRange([
                $"IReadOnlyCollection<{target.LinkTarget.Id}> {target.LinkTarget.Actor}.{LinksV3.FormatTypeName(type.Symbol)}.Ids => Ids;"
            ]);
        }
        
        foreach (var ancestor in target.EntityAssignableAncestors)
        {
            var overrideType = ancestor.EntityAssignableAncestors.Count > 0
                ? $"{ancestor.LinkTarget.Actor}{LinksV3.FormatPath(path.Add(type))}"
                : $"{ancestor.FormattedCoreLinkType}.Defined";

            members.AddRange([
                $"IReadOnlyCollection<{ancestor.LinkTarget.Id}> {overrideType}.Ids => Ids;"
            ]);
        }
    }

    public string? CreateProvider(LinksV3.Target target, Logger logger) => null;
}