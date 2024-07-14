using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableThreadableChannelActor :
    IThreadableChannelActor,
    ILoadableEntity<ulong, IThreadableChannel>;

[Modifiable<ModifyThreadableChannelProperties>(nameof(Routes.ModifyChannel))]
public partial interface IThreadableChannelActor :
    IGuildChannelActor,
    IActor<ulong, IThreadableChannel>
{
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
}
