namespace Discord;

public interface ILoadableThreadableGuildChannelActor<TGuildChannel> :
    IThreadableGuildChannelActor<TGuildChannel>,
    ILoadableEntity<ulong, TGuildChannel>
    where TGuildChannel : class, IGuildChannel<TGuildChannel>;

public interface IThreadableGuildChannelActor<out TGuildChannel> :
    IGuildChannelActor<TGuildChannel>
    where TGuildChannel : IGuildChannel<TGuildChannel>
{
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PublicArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> PrivateArchivedThreads { get; }
    IPagedLoadableRootActor<ILoadableThreadActor<IThreadChannel>, ulong, IThreadChannel> JoinedPrivateArchivedThreads { get; }
}
