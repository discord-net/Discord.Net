using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Types;

public interface ILinkTypeProcessor
{
    void AddOverrideMembers(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path
    );

    string? CreateProvider(LinksV3.Target target, Logger logger);
}