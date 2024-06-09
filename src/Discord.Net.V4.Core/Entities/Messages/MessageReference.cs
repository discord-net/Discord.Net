using System.Diagnostics;

namespace Discord;

/// <summary>
///     Contains the IDs sent from a crossposted message or inline reply.
/// </summary>
[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct MessageReference
{
    /// <summary>
    ///     The Message ID of the original message.
    /// </summary>
    public readonly ulong? MessageId;

    /// <summary>
    ///     The Channel ID of the original message.
    /// </summary>
    public readonly ulong? ChannelId;

    /// <summary>
    ///     The Guild ID of the original message.
    /// </summary>
    public readonly ulong? GuildId;

    /// <summary>
    ///     Whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message
    ///     Defaults to true.
    /// </summary>
    public readonly bool FailIfNotExists;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MessageReference" /> class.
    /// </summary>
    /// <param name="messageId">
    ///     The ID of the message that will be referenced. Used to reply to specific messages and the only parameter required
    ///     for it.
    /// </param>
    /// <param name="channelId">
    ///     The ID of the channel that will be referenced. It will be validated if sent.
    /// </param>
    /// <param name="guildId">
    ///     The ID of the guild that will be referenced. It will be validated if sent.
    /// </param>
    /// <param name="failIfNotExists">
    ///     Whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message.
    ///     Defaults to true.
    /// </param>
    public MessageReference(ulong? messageId = null, ulong? channelId = null, ulong? guildId = null,
        bool failIfNotExists = true)
    {
        MessageId = messageId;
        ChannelId = channelId;
        GuildId = guildId;
        FailIfNotExists = failIfNotExists;
    }

    private string DebuggerDisplay
        => $"Channel ID: ({ChannelId}){(GuildId.HasValue ? $", Guild ID: ({GuildId.Value})" : "")}" +
           $"{(MessageId.HasValue ? $", Message ID: ({MessageId.Value})" : "")}" +
           $", FailIfNotExists: ({FailIfNotExists})";

    /// <inheritdoc />
    public override string ToString()
        => DebuggerDisplay;
}
