using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using IModifiable = IModifiable<ulong, IMediaChannelActor, ModifyMediaChannelProperties, ModifyGuildChannelParams, IMediaChannel, IGuildMediaChannelModel>;

public interface ILoadableMediaChannelActor :
    IMediaChannelActor,
    ILoadableEntity<ulong, IMediaChannel>;

public interface IMediaChannelActor :
    IThreadableGuildChannelActor,
    IActor<ulong, IMediaChannel>,
    IModifiable
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel>
        IModifiable.ModifyRoute(
            IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
