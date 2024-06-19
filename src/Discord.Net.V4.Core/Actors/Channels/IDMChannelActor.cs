using Discord.Rest;

namespace Discord;

public interface ILoadableDMChannelActor :
    IDMChannelActor,
    ILoadableEntity<ulong, IDMChannel>;

public interface IDMChannelActor :
    IMessageChannelActor,
    IDeletable<ulong, IDMChannelActor>,
    IActor<ulong, IDMChannel>
{
    static BasicApiRoute IDeletable<ulong, IDMChannelActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteChannel(id);
}
