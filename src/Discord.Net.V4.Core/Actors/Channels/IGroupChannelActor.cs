using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface IGroupChannelActor<out TGroupChannel> :
    IMessageChannelActor<TGroupChannel>,
    IModifiable<ulong, IGroupChannelActor<TGroupChannel>, ModifyGroupDMProperties, ModifyGroupDmParams>
    where TGroupChannel : IGroupChannel<TGroupChannel>
{
    static ApiBodyRoute<ModifyGroupDmParams> IModifiable<ulong, IGroupChannelActor<TGroupChannel>, ModifyGroupDMProperties, ModifyGroupDmParams>.ModifyRoute(IPathable path, ulong id, ModifyGroupDmParams args)
        => Routes.ModifyChannel(id, args);
}
