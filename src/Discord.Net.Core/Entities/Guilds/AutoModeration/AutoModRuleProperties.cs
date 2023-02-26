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
        ///     Returns the max keyword count for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxKeywordCount = 1000;

        /// <summary>
        ///     Returns the max keyword length for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxKeywordLength = 60;

        /// <summary>
        ///     Returns the max regex pattern count for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxRegexPatternCount = 10;

        /// <summary>
        ///     Returns the max regex pattern length for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxRegexPatternLength = 260;

        /// <summary>
        ///     Returns the max allowlist keyword count for a <see cref="AutoModTriggerType.Keyword"/> AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxAllowListCountKeyword = 100;

        /// <summary>
        ///     Returns the max allowlist keyword count for a <see cref="AutoModTriggerType.KeywordPreset"/> AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxAllowListCountKeywordPreset = 1000;

        /// <summary>
        ///     Returns the max allowlist keyword length for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxAllowListEntryLength = 60;

        /// <summary>
        ///     Returns the max mention limit for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxMentionLimit = 50;

        /// <summary>
        ///     Returns the max exempt role count for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxExemptRoles = 20;

        /// <summary>
        ///     Returns the max exempt channel count for an AutoMod rule allowed by Discord.
        /// </summary>
        public const int MaxExemptChannels = 50;

        /// <summary>
        ///     Returns the max timeout duration in seconds for an auto moderation rule action.
        /// </summary>
        public const int MaxTimeoutSeconds = 2419200;

        /// <summary>
        ///     Returns the max custom message length AutoMod rule action allowed by Discord.
        /// </summary>
        public const int MaxCustomBlockMessageLength = 50;


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

        /// <summary>
        ///     Gets or sets the custom message that will be shown to members whenever their message is blocked.
        /// </summary>
        public Optional<string> CustomMessage { get; set; }
    }

}
