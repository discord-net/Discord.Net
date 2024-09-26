using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public class Enumerable : ILinkTypeProcessor
{
    public void AddOverrideMembers(List<string> members, LinksV3.Target target, LinkSchematics.Entry type, ImmutableList<LinkSchematics.Entry> path)
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
}