using Discord.Rest;

namespace Discord;

public interface IIntegrationActor :
    IGuildRelationship,
    IDeletable<ulong, IIntegrationActor>,
    IActor<ulong, IIntegration>
{
    static IApiRoute IDeletable<ulong, IIntegrationActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteGuildIntegration(path.Require<IGuild>(), id);
}
