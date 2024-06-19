namespace Discord;

public interface IMessageChannelRelationship : IMessageChannelRelationship<IMessageChannel>;

public interface IMessageChannelRelationship<TMessageChannel> :
    IChannelRelationship<TMessageChannel, ILoadableMessageChannelActor>
    where TMessageChannel : class, IMessageChannel;
