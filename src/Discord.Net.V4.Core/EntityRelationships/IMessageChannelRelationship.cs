namespace Discord;

public interface IMessageChannelRelationship : IMessageChannelActor<IMessageChannel>;

public interface IMessageChannelRelationship<TMessageChannel> :
    IChannelRelationship<TMessageChannel, ILoadableMessageChannelActor<TMessageChannel>>
    where TMessageChannel : class, IMessageChannel<TMessageChannel>;
