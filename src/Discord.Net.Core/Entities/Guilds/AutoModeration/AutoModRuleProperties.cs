using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides properties used to modify a <see cref="IAutoModRule"/>.
    /// </summary>
    public class AutoModRuleProperties
    {
        /// <summary>
        ///     Gets or sets the name for the rule.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets the event type for the rule.
        /// </summary>
        public Optional<AutoModEventType> EventType { get; set; }

        /// <summary>
        ///     Gets or sets the trigger type for the rule.
        /// </summary>
        public Optional<AutoModTriggerType> TriggerType { get; set; }

        /// <summary>
        ///     Gets or sets the keyword filter for the rule.
        /// </summary>
        public Optional<string[]> KeywordFilter { get; set; }

        /// <summary>
        ///     Gets or sets regex patterns for the rule.
        /// </summary>
        public Optional<string[]> RegexPatterns { get; set; }

        /// <summary>
        ///     Gets or sets the allow list for the rule.
        /// </summary>
        public Optional<string[]> AllowList { get; set; }

        /// <summary>
        ///     Gets or sets total mention limit for the rule.
        /// </summary>
        public Optional<int> MentionLimit { get; set; }

        /// <summary>
        ///     Gets or sets the presets for the rule. Empty if the rule has no presets.
        /// </summary>
        public Optional<KeywordPresetTypes[]> Presets { get; set; }

        /// <summary>
        ///     Gets or sets the actions for the rule.
        /// </summary>
        public Optional<AutoModRuleActionProperties[]> Actions { get; set; }

        /// <summary>
        ///     Gets or sets whether or not the rule is enabled.
        /// </summary>
        public Optional<bool> Enabled { get; set; }

        /// <summary>
        ///     Gets or sets the exempt roles for the rule. Empty if the rule has no exempt roles.
        /// </summary>
        public Optional<ulong[]> ExemptRoles { get; set; }

        /// <summary>
        ///     Gets or sets the exempt channels for the rule. Empty if the rule has no exempt channels.
        /// </summary>
        public Optional<ulong[]> ExemptChannels { get; set; }
    }

    /// <summary>
    ///     Provides properties used to modify a <see cref="AutoModRuleAction"/>.
    /// </summary>
    public class AutoModRuleActionProperties
    {
        /// <summary>
        ///     Gets or sets the type for this action.
        /// </summary>
        public AutoModActionType Type { get; set; }

        /// <summary>
        ///     Get or sets the channel id on which to post alerts.
        /// </summary>
        public ulong? ChannelId { get; set; }

        /// <summary>
        ///     Gets or sets the duration of which a user will be timed out for breaking this rule.
        /// </summary>
        public TimeSpan? TimeoutDuration { get; set; }
    }

}
