using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Impl;

public interface ILinkImplementer
{
    void Implement(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path
    );

    void ImplementBackLink(
        List<string> members,
        LinksV3.Target target,
        ImmutableList<string> path,
        ImmutableList<string>? ancestorPath = null,
        string? extraMembers = null
    );
}