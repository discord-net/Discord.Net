using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides properties used to create a <see cref="IAutoModRule"/>.
    /// </summary>
    public class AutoModRule
    {
        /// <summary>
        ///     Gets the name for the rule.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Gets the event type for the rule.
        /// </summary>
        public AutoModEventType EventType { get; }

        /// <summary>
        ///     Gets the trigger type for the rule.
        /// </summary>
        public AutoModTriggerType TriggerType { get; }

        /// <summary>
        ///     Gets the keyword filter for the rule.
        /// </summary>
        public string[] KeywordFilter { get; }

        /// <summary>
        ///     Gets regex patterns for the rule. Empty if the rule has no regexes.
        /// </summary>
        public string[] RegexPatterns { get; }

        /// <summary>
        ///     Gets the allow list for the rule. Empty if the rule has no allowed terms.
        /// </summary>
        public string[] AllowList { get; }

        /// <summary>
        ///     Gets total mention limit for the rule. <see langword="null"/> if not provided.
        /// </summary>
        public int? MentionLimit { get; }

        /// <summary>
        ///     Gets the presets for the rule. Empty if the rule has no presets.
        /// </summary>
        public KeywordPresetTypes[] Presets { get; }

        /// <summary>
        ///     Gets the actions for the rule.
        /// </summary>
        public AutoModRuleAction[] Actions { get; }

        /// <summary>
        ///     Gets whether or not the rule is enabled.
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        ///     Gets the exempt roles for the rule. Empty if the rule has no exempt roles.
        /// </summary>
        public ulong[] ExemptRoles { get; }

        /// <summary>
        ///     Gets the exempt channels for the rule. Empty if the rule has no exempt channels.
        /// </summary>
        public ulong[] ExemptChannels { get; }
        
        internal AutoModRule(string name,
            AutoModEventType eventType,
            AutoModTriggerType triggerType,
            string[] keywordFilter,
            string[] regexPatterns,
            string[] allowList,
            int? mentionLimit,
            KeywordPresetTypes[] presets,
            AutoModRuleAction[] actions,
            bool enabled,
            ulong[] exemptRoles,
            ulong[] exemptChannels)
        {
            Name = name;
            EventType = eventType;
            TriggerType = triggerType;
            KeywordFilter = keywordFilter;
            RegexPatterns = regexPatterns;
            AllowList = allowList;
            MentionLimit = mentionLimit;
            Presets = presets;
            Actions = actions;
            Enabled = enabled;
            ExemptRoles = exemptRoles;
            ExemptChannels = exemptChannels;
        }
    }

}
