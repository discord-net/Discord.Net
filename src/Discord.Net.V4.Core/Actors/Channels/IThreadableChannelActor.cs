namespace Discord;

public interface ILoadableThreadableChannelActor :
    IThreadableChannelActor,
    ILoadableEntity<ulong, IThreadableChannel>;

public interface IThreadableChannelActor :
    IGuildChannelActor,
    IActor<ulong, IThreadableChannel>
{
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
}
