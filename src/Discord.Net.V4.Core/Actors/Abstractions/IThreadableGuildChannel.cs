namespace Discord;

public interface ILoadableThreadableGuildChannelActor :
    IThreadableGuildChannelActor,
    ILoadableGuildChannelActor;

public interface IThreadableGuildChannelActor :
    IGuildChannelActor
{
    IPagedLoadableRootActor<ILoadableThreadActor, ulong, IThreadChannel> PublicArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor, ulong, IThreadChannel> PrivateArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor, ulong, IThreadChannel> JoinedPrivateArchivedThreads { get; }
}
