namespace Discord;

public interface ILoadableThreadableGuildChannelActor<TGuildChannel> :
    IThreadableGuildChannelActor,
    ILoadableGuildChannelActor<TGuildChannel>
    where TGuildChannel : class, IGuildChannel;

public interface IThreadableGuildChannelActor :
    IGuildChannelActor
{
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PublicArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PrivateArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> JoinedPrivateArchivedThreads { get; }
}
