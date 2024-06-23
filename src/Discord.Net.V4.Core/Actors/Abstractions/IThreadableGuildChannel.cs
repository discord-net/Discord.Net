namespace Discord;

public interface ILoadableThreadableGuildChannelActor :
    IThreadableGuildChannelActor,
    ILoadableGuildChannelActor;

public interface IThreadableGuildChannelActor :
    IGuildChannelActor
{
    IPagedActor<ulong, IThreadChannel> PublicArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel> PrivateArchivedThreads { get; }
    IPagedActor<ulong, IThreadChannel> JoinedPrivateArchivedThreads { get; }
}
