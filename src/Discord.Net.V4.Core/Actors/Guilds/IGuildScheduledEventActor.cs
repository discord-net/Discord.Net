using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildScheduledEventActor<TScheduledEvent> :
    IGuildScheduledEventActor<TScheduledEvent>,
    ILoadableEntity<ulong, TScheduledEvent>
    where TScheduledEvent : class, IGuildScheduledEvent;

public interface IGuildScheduledEventActor<out TScheduledEvent> :
    IGuildRelationship,
    IModifiable<ulong, IGuildScheduledEventActor<TScheduledEvent>, ModifyGuildScheduledEventProperties, ModifyGuildScheduledEventParams>,
    IDeletable<ulong, IGuildScheduledEventActor<TScheduledEvent>>,
    IActor<ulong, TScheduledEvent>
    where TScheduledEvent : IGuildScheduledEvent
{
    ILoadableRootActor<ILoadableGuildScheduledEventUserActor<IGuildScheduledEventUser>, ulong, IGuildScheduledEventUser> RSVPs
    {
        get;
    }

    static BasicApiRoute IDeletable<ulong, IGuildScheduledEventActor<TScheduledEvent>>.DeleteRoute(
        IPathable path, ulong id)
        => Routes.DeleteGuildScheduledEvent(path.Require<IGuild>(), id);

    static ApiBodyRoute<ModifyGuildScheduledEventParams>
        IModifiable<ulong, IGuildScheduledEventActor<TScheduledEvent>, ModifyGuildScheduledEventProperties,
            ModifyGuildScheduledEventParams>.ModifyRoute(IPathable path, ulong id, ModifyGuildScheduledEventParams args)
        => Routes.ModifyGuildScheduledEvent(path.Require<IGuild>(), path.Require<IChannel>(), args);
}
