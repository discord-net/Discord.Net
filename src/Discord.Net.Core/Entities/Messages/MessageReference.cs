using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     Contains the IDs sent from a crossposted message.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class MessageReference
    {
        /// <summary>
        ///     Gets the Message ID of the original message.
        /// </summary>
        public ulong? MessageId { get; internal set; }

        /// <summary>
        ///     Gets the Channel ID of the original message.
        /// </summary>
        public ulong ChannelId { get; internal set; }

        /// <summary>
        ///     Gets the Guild ID of the original message.
        /// </summary>
        public ulong? GuildId { get; internal set; }

        private string DebuggerDisplay
            => $"Channel ID: ({ChannelId}){(GuildId.HasValue ? $", Guild ID: ({GuildId.Value})" : "")}" +
            $"{(MessageId.HasValue ? $", Message ID: ({MessageId.Value})" : "")}";

        public override string ToString()
            => DebuggerDisplay;
    }
}
