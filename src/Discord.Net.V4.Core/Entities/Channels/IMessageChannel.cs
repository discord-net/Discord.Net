namespace Discord;

public interface IMessageChannel : IMessageChannel<IMessageChannel>;

/// <summary>
///     Represents a generic channel that can send and receive messages.
/// </summary>
public interface IMessageChannel<out TChannel> :
    IChannel<TChannel>
    where TChannel : IMessageChannel<TChannel>;
