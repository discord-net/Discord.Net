using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Model = Discord.API.AutoModerationRule;

namespace Discord.Rest;

public class RestAutoModRule : RestEntity<ulong>, IAutoModRule
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public ulong GuildId { get; private set; }      

    /// <inheritdoc />
    public string Name { get; private set; }    

    /// <inheritdoc />
    public ulong CreatorId { get; private set; }

    /// <inheritdoc />
    public AutoModEventType EventType { get; private set; } 

    /// <inheritdoc />
    public AutoModTriggerType TriggerType { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> KeywordFilter { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> RegexPatterns { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<string> AllowList { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<KeywordPresetTypes> Presets { get; private set; }

    /// <inheritdoc />
    public int? MentionTotalLimit { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<AutoModRuleAction> Actions { get; private set; }

    /// <inheritdoc />
    public bool Enabled { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> ExemptRoles { get; private set; }

    /// <inheritdoc />
    public IReadOnlyCollection<ulong> ExemptChannels { get; private set; }

    internal RestAutoModRule(BaseDiscordClient discord, ulong id) : base(discord, id)
    {

    }

    internal static RestAutoModRule Create(BaseDiscordClient discord, Model model)
    {
        var entity = new RestAutoModRule(discord, model.Id);
        entity.Update(model);
        return entity;
    }

    internal void Update(Model model)
    {
        Name = model.Name;
        CreatorId = model.CreatorId;
        GuildId = model.GuildId;

        EventType = model.EventType;
        TriggerType = model.TriggerType;
        KeywordFilter = model.TriggerMetadata.KeywordFilter.GetValueOrDefault(Array.Empty<string>()).ToImmutableArray();
        Presets = model.TriggerMetadata.Presets.GetValueOrDefault(Array.Empty<KeywordPresetTypes>()).ToImmutableArray();
        RegexPatterns = model.TriggerMetadata.RegexPatterns.GetValueOrDefault(Array.Empty<string>()).ToImmutableArray();
        AllowList = model.TriggerMetadata.AllowList.GetValueOrDefault(Array.Empty<string>()).ToImmutableArray();
        MentionTotalLimit = model.TriggerMetadata.MentionLimit.IsSpecified
            ? model.TriggerMetadata.MentionLimit.Value
            : null;
        Actions = model.Actions.Select(x => new AutoModRuleAction(
            x.Type,
            x.Metadata.GetValueOrDefault()?.ChannelId.ToNullable(),
            x.Metadata.GetValueOrDefault()?.DurationSeconds.ToNullable(),
            x.Metadata.IsSpecified
                ? x.Metadata.Value.CustomMessage.IsSpecified
                    ? x.Metadata.Value.CustomMessage.Value
                    : null
                : null
        )).ToImmutableArray();
        Enabled = model.Enabled;
        ExemptRoles = model.ExemptRoles.ToImmutableArray();
        ExemptChannels = model.ExemptChannels.ToImmutableArray();
    }

    /// <inheritdoc />
    public async Task ModifyAsync(Action<AutoModRuleProperties> func, RequestOptions options = null)
    {
        var model = await GuildHelper.ModifyRuleAsync(Discord, this, func, options);
        Update(model);
    }

    /// <inheritdoc />
    public Task DeleteAsync(RequestOptions options = null)
        => GuildHelper.DeleteRuleAsync(Discord, this, options);
}
