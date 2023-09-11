using Discord.Gateway.Cache;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway
{
    public sealed class GatewayRole : GatewayCacheableEntity<ulong, IRoleModel>, IRole
    {
        public GuildCacheable Guild { get; }

        public Color Color
            => new((uint)_source.Color);

        public bool IsHoisted
            => _source.IsHoisted;

        public bool IsManaged
            => _source.IsManaged;

        public bool IsMentionable
            => _source.IsMentionable;

        public string Name
            => _source.Name;

        public string? Icon
            => _source.Icon;

        public Emoji? Emoji
            => _source.UnicodeEmoji is null
                ? null
                : Emoji.TryParse(_source.UnicodeEmoji, out var emoji)
                    ? emoji
                    : new Emoji(_source.UnicodeEmoji);

        public GuildPermissions Permissions { get; }

        public int Position
            => _source.Position;

        public RoleTags Tags
            => _tags;

        public RoleFlags Flags
            => _source.Flags;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        public string Mention
            => MentionUtils.MentionRole(Id);

        protected override IRoleModel Model
            => _source;

        private IRoleModel _source;
        private GuildPermissions _permissions;
        private RoleTags _tags;
        private int _tagsVersion;

        internal GatewayRole(DiscordGatewayClient discord, ulong id, ulong guildId, IRoleModel model)
            : base(discord, id)
        {
            Guild = new(guildId, discord, discord.State.Guilds.ProvideSpecific(guildId));
            Update(model);
        }

        [MemberNotNull(nameof(_source), nameof(_tags))]
        internal override void Update(IRoleModel model)
        {
            _source = model;

            if(_permissions.RawValue != model.Permissions)
            {
                _permissions = new GuildPermissions(model.Permissions);
            }

            var tagsVersion = HashCode.Combine(
                model.BotId, model.IntegrationId, model.IsPremiumSubscriberRole,
                model.SubscriptionListingId, model.AvailableForPurchase,
                model.IsGuildConnection
            );

            if (_tags is null || _tagsVersion != tagsVersion)
            {
                _tags = new RoleTags(
                    model.BotId,
                    model.IntegrationId, model.IsPremiumSubscriberRole
                );
                _tagsVersion = tagsVersion;
            }
        }


        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        public int CompareTo(IRole? other) => throw new NotImplementedException();
        public Task DeleteAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public string GetIconUrl() => throw new NotImplementedException();
        public Task ModifyAsync(Action<ModifyRoleProperties> func, RequestOptions? options = null) => throw new NotImplementedException();

        IEntitySource<IGuild, ulong> IRole.Guild => Guild;
    }
}

