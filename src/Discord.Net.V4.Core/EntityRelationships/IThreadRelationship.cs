namespace Discord;

public interface IThreadRelationship : IThreadRelationship<IThreadChannel>;
public interface IThreadRelationship<TThread> :
    IGuildChannelRelationship<TThread, ILoadableThreadActor<TThread>>
    where TThread : class, IThreadChannel
{
    ILoadableThreadActor<TThread> Thread { get; }

    ILoadableThreadActor<TThread> IChannelRelationship<TThread, ILoadableThreadActor<TThread>>.Channel => Thread;
}
