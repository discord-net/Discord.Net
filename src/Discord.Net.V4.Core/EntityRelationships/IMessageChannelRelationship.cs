namespace Discord;

public interface IMessageChannelRelationship : IMessageChannelRelationship<IMessageChannel>;

public interface IMessageChannelRelationship<TMessageChannel> :
    IChannelRelationship<TMessageChannel, ILoadableMessageChannelActor<TMessageChannel>>
    where TMessageChannel : class, IMessageChannel;
