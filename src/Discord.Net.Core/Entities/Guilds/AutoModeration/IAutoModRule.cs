using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a auto mod rule within a guild.
    /// </summary>
    public interface IAutoModRule : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets the guild id on which this rule exists.
        /// </summary>
        ulong GuildId { get; }

        /// <summary>
        ///     Get the name of this rule.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the id of the user who created this use.
        /// </summary>
        ulong CreatorId { get; }

        /// <summary>
        ///     Gets the event type on which this rule is triggered.
        /// </summary>
        AutoModEventType EventType { get; }

        /// <summary>
        ///     Gets the trigger type on which this rule executes.
        /// </summary>
        AutoModTriggerType TriggerType { get; }

        /// <summary>
        ///     Gets the keyword filter for this rule.
        /// </summary>
        /// <remarks>
        ///     This collection will be empty if <see cref="TriggerType"/> is not
        ///     <see cref="AutoModTriggerType.Keyword"/>.
        /// </remarks>
        public IReadOnlyCollection<string> KeywordFilter { get; }

        /// <summary>
        ///     Gets regex patterns for this rule. Empty if the rule has no regexes.
        /// </summary>
        /// <remarks>
        ///     This collection will be empty if <see cref="TriggerType"/> is not
        ///     <see cref="AutoModTriggerType.Keyword"/>.
        /// </remarks>
        public IReadOnlyCollection<string> RegexPatterns { get; }

        /// <summary>
        ///     Gets the allow list patterns for this rule. Empty if the rule has no allowed terms.
        /// </summary>
        /// <remarks>
        ///     This collection will be empty if <see cref="TriggerType"/> is not
        ///     <see cref="AutoModTriggerType.Keyword"/>.
        /// </remarks>
        public IReadOnlyCollection<string> AllowList { get; }

        /// <summary>
        ///     Gets the preset keyword types for this rule. Empty if the rule has no presets.
        /// </summary>
        /// <remarks>
        ///     This collection will be empty if <see cref="TriggerType"/> is not
        ///     <see cref="AutoModTriggerType.KeywordPreset"/>.
        /// </remarks>
        public IReadOnlyCollection<KeywordPresetTypes> Presets { get; }

        /// <summary>
        ///     Gets the total mention limit for this rule.
        /// </summary>
        /// <remarks>
        ///     This property will be <see langword="null"/> if <see cref="TriggerType"/> is not
        ///     <see cref="AutoModTriggerType.MentionSpam"/>.
        /// </remarks>
        public int? MentionTotalLimit { get; }

        /// <summary>
        ///     Gets a collection of actions that will be preformed if a user breaks this rule.
        /// </summary>
        IReadOnlyCollection<AutoModRuleAction> Actions { get; }

        /// <summary>
        ///     Gets whether or not this rule is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        ///     Gets a collection of role ids that are exempt from this rule. Empty if the rule has no exempt roles.
        /// </summary>
        IReadOnlyCollection<ulong> ExemptRoles { get; }

        /// <summary>
        ///     Gets a collection of channel ids that are exempt from this rule. Empty if the rule has no exempt channels.
        /// </summary>
        IReadOnlyCollection<ulong> ExemptChannels { get; }

        /// <summary>
        ///     Modifies this rule.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the rule with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ModifyAsync(Action<AutoModRuleProperties> func, RequestOptions options = null);
    }
}
