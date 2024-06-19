using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGroupChannelActor<TGroupChannel> :
    IGroupChannelActor,
    ILoadableChannelActor
    where TGroupChannel : class, IGroupChannel;

public interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>,
    IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>
{
    static ApiBodyRoute<ModifyGroupDmParams> IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>.ModifyRoute(IPathable path, ulong id, ModifyGroupDmParams args)
        => Routes.ModifyChannel(id, args);
}
