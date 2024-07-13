using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

using Modifiable =
    IModifiable<ulong, IThreadableChannelActor, ModifyThreadableChannelProperties, ModifyGuildChannelParams,
        IThreadableChannel, IThreadableChannelModel>;

public interface ILoadableThreadableChannelActor :
    IThreadableChannelActor,
    ILoadableEntity<ulong, IThreadableChannel>;

public interface IThreadableChannelActor :
    IGuildChannelActor,
    Modifiable,
    IActor<ulong, IThreadableChannel>
{
    static IApiInOutRoute<ModifyGuildChannelParams, IEntityModel> Modifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildChannelParams args
    ) => Routes.ModifyChannel(id, args);

    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
}
