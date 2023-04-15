using Discord.API.AuditLogs;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket;

/// <summary>
///     Represents information for an auto moderation rule.
/// </summary>
public class SocketAutoModRuleInfo
{
    internal SocketAutoModRuleInfo(AutoModRuleInfoAuditLogModel model)
    {
        Actions = model.Actions?.Select(x => new AutoModRuleAction(
            x.Type,
            x.Metadata.GetValueOrDefault()?.ChannelId.ToNullable(),
            x.Metadata.GetValueOrDefault()?.DurationSeconds.ToNullable(),
            x.Metadata.IsSpecified
                ? x.Metadata.Value.CustomMessage.IsSpecified
                    ? x.Metadata.Value.CustomMessage.Value
                    : null
                : null
        )).ToImmutableArray();
        KeywordFilter = model.TriggerMetadata?.KeywordFilter.GetValueOrDefault(Array.Empty<string>())?.ToImmutableArray();
        Presets = model.TriggerMetadata?.Presets.GetValueOrDefault(Array.Empty<KeywordPresetTypes>())?.ToImmutableArray();
        RegexPatterns = model.TriggerMetadata?.RegexPatterns.GetValueOrDefault(Array.Empty<string>())?.ToImmutableArray();
        AllowList = model.TriggerMetadata?.AllowList.GetValueOrDefault(Array.Empty<string>())?.ToImmutableArray();
        MentionTotalLimit = model.TriggerMetadata?.MentionLimit.IsSpecified ?? false
            ? model.TriggerMetadata?.MentionLimit.Value
            : null;
        Name = model.Name;
        Enabled = model.Enabled;
        ExemptRoles = model.ExemptRoles?.ToImmutableArray();
        ExemptChannels = model.ExemptChannels?.ToImmutableArray();
        TriggerType = model.TriggerType;
        EventType = model.EventType;
    }

    /// <inheritdoc cref="IAutoModRule.Name"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public string Name { get; set; }

    /// <inheritdoc cref="IAutoModRule.EventType"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public AutoModEventType? EventType { get; set; }

    /// <inheritdoc cref="IAutoModRule.TriggerType"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public AutoModTriggerType? TriggerType { get; set; }

    /// <inheritdoc cref="IAutoModRule.Enabled"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public bool? Enabled { get; set; }

    /// <inheritdoc cref="IAutoModRule.ExemptRoles"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<ulong> ExemptRoles { get; set; }

    /// <inheritdoc cref="IAutoModRule.ExemptChannels"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<ulong> ExemptChannels { get; set; }

    /// <inheritdoc cref="IAutoModRule.KeywordFilter"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<string> KeywordFilter { get; }

    /// <inheritdoc cref="IAutoModRule.RegexPatterns"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<string> RegexPatterns { get; }

    /// <inheritdoc cref="IAutoModRule.AllowList"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<string> AllowList { get; }

    /// <inheritdoc cref="IAutoModRule.Presets"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<KeywordPresetTypes> Presets { get; }

    /// <inheritdoc cref="IAutoModRule.MentionTotalLimit"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public int? MentionTotalLimit { get; }

    /// <inheritdoc cref="IAutoModRule.Actions"/>
    /// <remarks>
    ///     <see langword="null"/> if this property is not mentioned in this entry.
    /// </remarks>
    public IReadOnlyCollection<AutoModRuleAction> Actions { get; private set; }
}
