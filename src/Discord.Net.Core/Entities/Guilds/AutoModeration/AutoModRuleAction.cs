using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents an action that will be preformed if a user breaks an <see cref="IAutoModRule"/>.
    /// </summary>
    public class AutoModRuleAction
    {
        /// <summary>
        ///     Gets the type for this action.
        /// </summary>
        public AutoModActionType Type { get; }

        /// <summary>
        ///     Get the channel id on which to post alerts. <see langword="null"/> if no channel has been provided.
        /// </summary>
        public ulong? ChannelId { get; }

        /// <summary>
        ///     Gets the custom message that will be shown to members whenever their message is blocked.
        ///     <see langword="null"/> if no message has been set.
        /// </summary>
        public Optional<string> CustomMessage { get; set; }

        /// <summary>
        ///     Gets the duration of which a user will be timed out for breaking this rule. <see langword="null"/> if no timeout duration has been provided.
        /// </summary>
        public TimeSpan? TimeoutDuration { get; }

        internal AutoModRuleAction(AutoModActionType type, ulong? channelId, int? duration, string customMessage)
        {
            Type = type;
            ChannelId = channelId;
            TimeoutDuration = duration.HasValue ? TimeSpan.FromSeconds(duration.Value) : null;
            CustomMessage = customMessage;
        }
    }
}
