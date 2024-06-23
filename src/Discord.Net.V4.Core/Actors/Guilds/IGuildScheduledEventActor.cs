using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildScheduledEventActor :
    IGuildScheduledEventActor,
    ILoadableEntity<ulong, IGuildScheduledEvent>;

public interface IGuildScheduledEventActor :
    IGuildRelationship,
    IModifiable<ulong, IGuildScheduledEventActor, ModifyGuildScheduledEventProperties, ModifyGuildScheduledEventParams>,
    IDeletable<ulong, IGuildScheduledEventActor>,
    IActor<ulong, IGuildScheduledEvent>
{
    IEnumerableIndexableActor<ILoadableGuildScheduledEventUserActor<IGuildScheduledEventUser>, ulong, IGuildScheduledEventUser>
        RSVPs
    {
        get;
    }

    static IApiRoute IDeletable<ulong, IGuildScheduledEventActor>.DeleteRoute(
        IPathable path, ulong id)
        => Routes.DeleteGuildScheduledEvent(path.Require<IGuild>(), id);

    static IApiInRoute<ModifyGuildScheduledEventParams>
        IModifiable<ulong, IGuildScheduledEventActor, ModifyGuildScheduledEventProperties,
            ModifyGuildScheduledEventParams>.ModifyRoute(IPathable path, ulong id, ModifyGuildScheduledEventParams args)
        => Routes.ModifyGuildScheduledEvent(path.Require<IGuild>(), path.Require<IChannel>(), args);
}
