using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.AutoModerationRule;

namespace Discord.WebSocket
{
    public class SocketAutoModRule : SocketEntity<ulong>, IAutoModRule
    {
        /// <summary>
        ///     Gets the guild that this rule is in.
        /// </summary>
        public SocketGuild Guild { get; }

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the creator of this rule.
        /// </summary>
        public SocketGuildUser Creator { get; private set; }

        /// <inheritdoc/>
        public AutoModEventType EventType { get; private set; }

        /// <inheritdoc/>
        public AutoModTriggerType TriggerType { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> KeywordFilter { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> RegexPatterns { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> AllowList { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<KeywordPresetTypes> Presets { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<AutoModRuleAction> Actions { get; private set; }

        /// <inheritdoc/>
        public int? MentionTotalLimit { get; private set; }

        /// <inheritdoc/>
        public bool Enabled { get; private set; }

        /// <summary>
        ///     Gets the roles that are exempt from this rule.
        /// </summary>
        public IReadOnlyCollection<SocketRole> ExemptRoles { get; private set; }

        /// <summary>
        ///     Gets the channels that are exempt from this rule.
        /// </summary>
        public IReadOnlyCollection<SocketGuildChannel> ExemptChannels { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        private ulong _creatorId;

        internal SocketAutoModRule(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id)
        {
            Guild = guild;
        }

        internal static SocketAutoModRule Create(DiscordSocketClient discord, SocketGuild guild, Model model)
        {
            var entity = new SocketAutoModRule(discord, model.Id, guild);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            Name = model.Name;
            _creatorId = model.CreatorId;
            Creator ??= Guild.GetUser(_creatorId);
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
            ExemptRoles = model.ExemptRoles.Select(x => Guild.GetRole(x)).ToImmutableArray();
            ExemptChannels = model.ExemptChannels.Select(x => Guild.GetChannel(x)).ToImmutableArray();
        }

        /// <inheritdoc/>
        public async Task ModifyAsync(Action<AutoModRuleProperties> func, RequestOptions options = null)
        {
            var model = await GuildHelper.ModifyRuleAsync(Discord, this, func, options);
            Guild.AddOrUpdateAutoModRule(model);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(RequestOptions options = null)
            => GuildHelper.DeleteRuleAsync(Discord, this, options);

        internal SocketAutoModRule Clone() => MemberwiseClone() as SocketAutoModRule;

        #region IAutoModRule
        IReadOnlyCollection<ulong> IAutoModRule.ExemptRoles => ExemptRoles.Select(x => x.Id).ToImmutableArray();
        IReadOnlyCollection<ulong> IAutoModRule.ExemptChannels => ExemptChannels.Select(x => x.Id).ToImmutableArray();
        ulong IAutoModRule.GuildId => Guild.Id;
        ulong IAutoModRule.CreatorId => _creatorId;
        #endregion
    }
}
