using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IGroupChannelActor, ModifyGroupDMProperties, ModifyGroupDmParams, IGroupChannel, IGroupDMChannelModel>;

public interface ILoadableGroupChannelActor :
    IGroupChannelActor,
    ILoadableEntity<ulong, IGroupChannel>;

public interface IGroupChannelActor :
    IMessageChannelActor,
    IActor<ulong, IGroupChannel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGroupDmParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGroupDmParams args
    ) => Routes.ModifyChannel(id, args);
}
