using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IForumChannelActor, ModifyForumChannelProperties, ModifyGuildChannelParams, IForumChannel,
    IGuildForumChannelModel>;

public interface ILoadableForumChannelActor :
    IForumChannelActor,
    ILoadableEntity<ulong, IForumChannel>;

public interface IForumChannelActor :
    IThreadableGuildChannelActor,
    IActor<ulong, IForumChannel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildChannelParams args
    ) => Routes.ModifyChannel(id, args);
}
