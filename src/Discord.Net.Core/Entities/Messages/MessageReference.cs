using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override string ToString()
            => $"Guild: {GuildId}, Channel: {ChannelId}, Message: {MessageId}";
    }
}
