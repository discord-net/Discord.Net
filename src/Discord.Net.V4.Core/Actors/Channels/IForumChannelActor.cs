using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface IForumChannelActor<out TForumChannel> :
    IThreadableGuildChannelActor<TForumChannel>,
    IModifiable<ulong, IForumChannelActor<TForumChannel>, ModifyForumChannelProperties, ModifyGuildChannelParams>
    where TForumChannel : IForumChannel<TForumChannel>
{
    static ApiBodyRoute<ModifyGuildChannelParams> IModifiable<ulong, IForumChannelActor<TForumChannel>, ModifyForumChannelProperties, ModifyGuildChannelParams>
        .ModifyRoute(IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
