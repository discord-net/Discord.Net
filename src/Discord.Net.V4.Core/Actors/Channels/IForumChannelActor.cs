using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableForumChannelActor<TForumChannel> :
    IForumChannelActor,
    ILoadableEntity<ulong, TForumChannel>
    where TForumChannel : class, IForumChannel;

public interface IForumChannelActor :
    IThreadableGuildChannelActor,
    IActor<ulong, IForumChannel>,
    IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams>
{
    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
