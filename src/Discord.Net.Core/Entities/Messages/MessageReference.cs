using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     Contains the IDs sent from a crossposted message or inline reply.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class MessageReference
    {
        /// <summary>
        ///     Gets the Message ID of the original message.
        /// </summary>
        public Optional<ulong> MessageId { get; internal set; }

        /// <summary>
        ///     Gets the Channel ID of the original message.
        /// </summary>
        /// <remarks>
        ///     It only will be the default value (zero) if it was instantiated with a <see langword="null"/> in the constructor.
        /// </remarks>
        public ulong ChannelId { get => InternalChannelId.GetValueOrDefault(); }
        internal Optional<ulong> InternalChannelId;

        /// <summary>
        ///     Gets the Guild ID of the original message.
        /// </summary>
        public Optional<ulong> GuildId { get; internal set; }

        /// <summary>
        ///     Gets whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message
        ///     Defaults to true.
        /// </summary>
        public Optional<bool> FailIfNotExists { get; internal set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageReference"/> class.
        /// </summary>
        /// <param name="messageId">
        ///     The ID of the message that will be referenced. Used to reply to specific messages and the only parameter required for it.
        /// </param>
        /// <param name="channelId">
        ///     The ID of the channel that will be referenced. It will be validated if sent.
        /// </param>
        /// <param name="guildId">
        ///     The ID of the guild that will be referenced. It will be validated if sent.
        /// </param>
        /// <param name="failIfNotExists">
        ///     Whether to error if the referenced message doesn't exist instead of sending as a normal (non-reply) message. Defaults to true.
        /// </param>
        public MessageReference(ulong? messageId = null, ulong? channelId = null, ulong? guildId = null, bool? failIfNotExists = null)
        {
            MessageId = messageId ?? Optional.Create<ulong>();
            InternalChannelId = channelId ?? Optional.Create<ulong>();
            GuildId = guildId ?? Optional.Create<ulong>();
            FailIfNotExists = failIfNotExists ?? Optional.Create<bool>();
        }

        private string DebuggerDisplay
            => $"Channel ID: ({ChannelId}){(GuildId.IsSpecified ? $", Guild ID: ({GuildId.Value})" : "")}" +
            $"{(MessageId.IsSpecified ? $", Message ID: ({MessageId.Value})" : "")}" +
            $"{(FailIfNotExists.IsSpecified ? $", FailIfNotExists: ({FailIfNotExists.Value})" : "")}";

        public override string ToString()
            => DebuggerDisplay;
    }
}
