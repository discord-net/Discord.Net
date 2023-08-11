using Discord.WebSocket.Cache;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.WebSocket
{
    public sealed class SocketGuildEmote : SocketCacheableEntity<ulong, IGuildEmoteModel>, IEmote
    {
        public GuildRolesCacheable Roles { get; }
        public GuildMemberCacheable Author { get; }

        public IReadOnlyCollection<ulong> RoleIds
            => _source.Roles.ToImmutableArray();

        public string Name
            => _source.Name;

        public bool IsAnimated
            => _source.IsAnimated;

        public bool RequiresColons
            => _source.RequireColons;

        public bool IsManaged
            => _source.IsManaged;

        public bool IsAvailable
            => _source.IsAvailable;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        protected override IGuildEmoteModel Model
            => _source;

        private IGuildEmoteModel _source;

        public SocketGuildEmote(DiscordSocketClient discord, ulong guildId, IGuildEmoteModel model)
            : base(discord, model.Id)
        {
            Update(model);
            Roles = new(guildId, () => _source.Roles, id => new GuildRoleCacheable(id, discord, discord.State.GuildRoles.SourceSpecific(id)));
            Author = new(guildId, model.CreatorId, discord, discord.State.Members.SourceSpecific(model.CreatorId, guildId));
        }

        [MemberNotNull(nameof(_source))]
        internal override void Update(IGuildEmoteModel model)
        {
            _source = model;
        }

        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();
    }
}

