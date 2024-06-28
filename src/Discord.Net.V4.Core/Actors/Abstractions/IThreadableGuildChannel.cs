namespace Discord;

public interface ILoadableThreadableGuildChannelActor :
    IThreadableGuildChannelActor,
    ILoadableGuildChannelActor;

public interface IThreadableGuildChannelActor :
    IGuildChannelActor
{
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel, PageThreadChannelsParams> JoinedPrivateArchivedThreads { get; }
}
