using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGroupChannelActor :
    IGroupChannelActor,
    ILoadableEntity<ulong, IGroupChannel>;

public interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>,
    IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>
{
    static IApiInRoute<ModifyGroupDmParams>
        IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams>.ModifyRoute(IPathable path,
            ulong id, ModifyGroupDmParams args)
        => Routes.ModifyChannel(id, args);
}
