namespace Discord;

/// <summary>
///     Represents a snapshot of a message.
/// </summary>
public readonly struct MessageSnapshot
{
    /// <summary>
    ///     Gets the partial message that was forwarded.
    /// </summary>
    public readonly IMessage Message;

    internal MessageSnapshot(IMessage message)
    {
        Message = message;
    }
}
