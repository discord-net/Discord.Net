using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableMediaChannelActor :
    IMediaChannelActor,
    ILoadableEntity<ulong, IMediaChannel>;

public interface IMediaChannelActor :
    IThreadableGuildChannelActor,
    IActor<ulong, IForumChannel>,
    IModifiable<ulong, IMediaChannelActor, ModifyMediaChannelProperties, ModifyGuildChannelParams>
{
    static IApiInRoute<ModifyGuildChannelParams>
        IModifiable<ulong, IMediaChannelActor, ModifyMediaChannelProperties, ModifyGuildChannelParams>.ModifyRoute(
            IPathable path, ulong id, ModifyGuildChannelParams args)
        => Routes.ModifyChannel(id, args);
}
