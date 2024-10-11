using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors.V3.Types;

namespace Discord.Net.Hanz.Tasks.Actors.V3.Impl;

public interface ILinkImplementer
{
    ConstructorRequirements? ImplementLink(
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        Logger logger
    );

    ConstructorRequirements? ImplementBackLink(
        ConstructorRequirements? requirements,
        List<string> members,
        LinksV3.Target target,
        LinkSchematics.Entry type,
        ImmutableList<LinkSchematics.Entry> path,
        Logger logger
    );

    string CreateRootBackLink(LinksV3.Target target, Logger logger);
    
    ConstructorRequirements? ImplementHierarchy();

    string ImplementExtension(
        LinksV3.Target target,
        LinkExtensions.Extension extension,
        ImmutableList<LinkExtensions.Extension> children,
        ImmutableList<LinkExtensions.Extension> parents,
        ImmutableList<string> path,
        ImmutableList<string> pathWithoutExtensions,
        Logger logger
    );
}