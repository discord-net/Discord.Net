using Discord.API;
using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public sealed class GatewayGuildUser : GatewayUser, ICacheableEntity<ulong, IMemberModel>, IGuildUser
    {
        public GuildCacheable Guild { get; }

        public DateTimeOffset? JoinedAt
            => _source.JoinedAt;

        public string DisplayName
            => _source.Nickname ?? Username;

        public string? Nickname
            => _source.Nickname;

        public string? DisplayAvatarId
            => _source.GuildAvatar ?? AvatarId;

        public string? GuildAvatarId
            => _source.GuildAvatar;

        public GuildUserFlags Flags
            => _source.Flags;

        public GuildPermissions GuildPermissions
            => default!; // TODO: resolve guild


        public DateTimeOffset? PremiumSince
            => _source.PremiumSince;

        public IReadOnlyCollection<ulong> RoleIds
            => _source.RoleIds.ToImmutableArray();

        public bool? IsPending
            => _source.IsPending;

        public int Hierarchy => throw new NotImplementedException(); //  TODO: resolve guild

        public DateTimeOffset? TimedOutUntil
            => _source.CommunicationsDisabledUntil;

        #region Voice state
        public bool IsDeafened => throw new NotImplementedException();

        public bool IsMuted => throw new NotImplementedException();

        public bool IsSelfDeafened => throw new NotImplementedException();

        public bool IsSelfMuted => throw new NotImplementedException();

        public bool IsSuppressed => throw new NotImplementedException();

        public IVoiceChannel VoiceChannel => throw new NotImplementedException();

        public string VoiceSessionId => throw new NotImplementedException();

        public bool IsStreaming => throw new NotImplementedException();

        public bool IsVideoing => throw new NotImplementedException();

        public DateTimeOffset? RequestToSpeakTimestamp => throw new NotImplementedException();
        #endregion

        private IMemberModel _source;

        internal GatewayGuildUser(DiscordGatewayClient client, ulong guildId, IMemberModel model, IUserModel user)
            : base(client, user)
        {
            Guild = new(guildId, client, client.State.Guilds.ProvideSpecific(guildId));
            _source = model;
        }

        public void Update(IMemberModel model)
        {
            _source = model;
        }

        public Task AddRoleAsync(ulong roleId, RequestOptions? options = null) => throw new NotImplementedException();
        public Task AddRoleAsync(IRole role, RequestOptions? options = null) => throw new NotImplementedException();
        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions? options = null) => throw new NotImplementedException();
        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null) => throw new NotImplementedException();
        public string GetDisplayAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128) => throw new NotImplementedException();
        public string GetGuildAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128) => throw new NotImplementedException();
        public ChannelPermissions GetPermissions(IGuildChannel channel) => throw new NotImplementedException();
        public Task KickAsync(string? reason = null, RequestOptions? options = null) => throw new NotImplementedException();
        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveRoleAsync(ulong roleId, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveRoleAsync(IRole role, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveTimeOutAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task SetTimeOutAsync(TimeSpan span, RequestOptions? options = null) => throw new NotImplementedException();

        IMemberModel ICacheableEntity<ulong, IMemberModel>.GetModel()
            => _source;

        IEntityModel<ulong> ICacheableEntity<ulong>.GetModel()
            => _source;

        void ICacheableEntity<ulong>.Update(IEntityModel<ulong> model)
        {
            if (model is IMemberModel member)
                Update(member);
            else if (model is IUserModel user)
                base.Update(user);
        }

        IEntitySource<IGuild, ulong> IGuildUser.Guild => Guild;

        IEntitySource<IVoiceChannel, ulong> IVoiceState.VoiceChannel => throw new NotImplementedException();
    }
}
