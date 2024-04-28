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

    /// <summary>
    ///     Gets the id of the originating message's guild
    /// </summary>
    public readonly ulong? GuildId;

    internal MessageSnapshot(IMessage message, ulong? guildId)
    {
        Message = message;
        GuildId = guildId;
    }
}
