namespace Discord.Messages;

public interface ILoadableMessageEntitySource<TMessage> :
    IMessageEntitySource<TMessage>,
    ILoadableEntity<ulong, TMessage>
    where TMessage : class, IMessage;

public interface IMessageEntitySource<TMessage> : IEntitySource<ulong, TMessage>
    where TMessage : IMessage;
