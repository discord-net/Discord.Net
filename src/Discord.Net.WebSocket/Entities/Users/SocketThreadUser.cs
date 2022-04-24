using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.IThreadMemberModel;
using System.Collections.Immutable;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a thread user received over the gateway.
    /// </summary>
    public class SocketThreadUser : SocketUser, IThreadUser, IGuildUser, ICached<Model>
    {
        /// <summary>
        ///     Gets the <see cref="SocketThreadChannel"/> this user is in.
        /// </summary>
        public Lazy<SocketThreadChannel> Thread { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset ThreadJoinedAt { get; private set; }

        /// <summary>
        ///     Gets the guild this user is in.
        /// </summary>
        public Lazy<SocketGuild> Guild { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset? JoinedAt
            => GuildUser.Value.JoinedAt;

        /// <inheritdoc/>
        public string DisplayName
            => GuildUser.Value.Nickname ?? GuildUser.Value.Username;

        /// <inheritdoc/>
        public string Nickname
            => GuildUser.Value.Nickname;

        /// <inheritdoc/>
        public DateTimeOffset? PremiumSince
            => GuildUser.Value.PremiumSince;

        /// <inheritdoc/>
        public DateTimeOffset? TimedOutUntil
            => GuildUser.Value.TimedOutUntil;

        /// <inheritdoc/>
        public bool? IsPending
            => GuildUser.Value.IsPending;

        /// <inheritdoc />
        public int Hierarchy
            => GuildUser.Value.Hierarchy;

        /// <inheritdoc/>
        public override string AvatarId
        {
            get => GuildUser.Value.AvatarId;
            internal set => GuildUser.Value.AvatarId = value;
        }

        /// <inheritdoc/>
        public string DisplayAvatarId => GuildAvatarId ?? AvatarId;

        /// <inheritdoc/>
        public string GuildAvatarId
            => GuildUser.Value.GuildAvatarId;

        /// <inheritdoc/>
        public override ushort DiscriminatorValue
        {
            get => GuildUser.Value.DiscriminatorValue;
            internal set => GuildUser.Value.DiscriminatorValue = value;
        }

        /// <inheritdoc/>
        public override bool IsBot
        {
            get => GuildUser.Value.IsBot;
            internal set => GuildUser.Value.IsBot = value;
        }

        /// <inheritdoc/>
        public override bool IsWebhook
            => GuildUser.Value.IsWebhook;

        /// <inheritdoc/>
        public override string Username
        {
            get => GuildUser.Value.Username;
            internal set => GuildUser.Value.Username = value;
        }

        /// <inheritdoc/>
        public bool IsDeafened
            => GuildUser.Value.IsDeafened;

        /// <inheritdoc/>
        public bool IsMuted
            => GuildUser.Value.IsMuted;

        /// <inheritdoc/>
        public bool IsSelfDeafened
            => GuildUser.Value.IsSelfDeafened;

        /// <inheritdoc/>
        public bool IsSelfMuted
            => GuildUser.Value.IsSelfMuted;

        /// <inheritdoc/>
        public bool IsSuppressed
            => GuildUser.Value.IsSuppressed;

        /// <inheritdoc/>
        public IVoiceChannel VoiceChannel
            => GuildUser.Value.VoiceChannel;

        /// <inheritdoc/>
        public string VoiceSessionId
            => GuildUser.Value.VoiceSessionId;

        /// <inheritdoc/>
        public bool IsStreaming
            => GuildUser.Value.IsStreaming;

        /// <inheritdoc/>
        public bool IsVideoing
            => GuildUser.Value.IsVideoing;

        /// <inheritdoc/>
        public DateTimeOffset? RequestToSpeakTimestamp
            => GuildUser.Value.RequestToSpeakTimestamp;

        private Lazy<SocketGuildUser> GuildUser { get; set; }

        private ulong _threadId;
        private ulong _guildId;


        internal SocketThreadUser(DiscordSocketClient client, ulong guildId, ulong threadId, ulong userId)
            : base(client, userId)
        {
            _guildId = guildId;
            _threadId = threadId;

            GuildUser = new(() => client.StateManager.TryGetMemberStore(guildId, out var store) ? store.Get(userId) : null);
            Thread = new(() => client.GetChannel(threadId) as SocketThreadChannel);
            Guild = new(() => client.GetGuild(guildId));
        }

        internal static SocketThreadUser Create(SocketGuild guild, SocketThreadChannel thread, Model model, SocketGuildUser member)
        {
            var entity = new SocketThreadUser(guild.Discord, guild.Id, thread.Id, model.UserId.Value);
            entity.Update(model);
            return entity;
        }

        internal static SocketThreadUser Create(DiscordSocketClient client, ulong guildId, ulong threadId, Model model)
        {
            var entity = new SocketThreadUser(client, guildId, threadId, model.UserId.Value);
            entity.Update(model);
            return entity;
        }

        internal static SocketThreadUser Create(SocketGuild guild, SocketThreadChannel thread, SocketGuildUser owner)
        {
            // this is used for creating the owner of the thread.
            var entity = new SocketThreadUser(guild.Discord, guild.Id, thread.Id, owner.Id);
            entity.ThreadJoinedAt = thread.CreatedAt;
            return entity;
        }

        internal void Update(Model model)
        {
            ThreadJoinedAt = model.JoinedAt;
        }

        /// <inheritdoc/>
        public ChannelPermissions GetPermissions(IGuildChannel channel) => GuildUser.Value.GetPermissions(channel);

        /// <inheritdoc/>
        public Task KickAsync(string reason = null, RequestOptions options = null) => GuildUser.Value.KickAsync(reason, options);

        /// <inheritdoc/>
        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null) => GuildUser.Value.ModifyAsync(func, options);

        /// <inheritdoc/>
        public Task AddRoleAsync(ulong roleId, RequestOptions options = null) => GuildUser.Value.AddRoleAsync(roleId, options);

        /// <inheritdoc/>
        public Task AddRoleAsync(IRole role, RequestOptions options = null) => GuildUser.Value.AddRoleAsync(role, options);

        /// <inheritdoc/>
        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null) => GuildUser.Value.AddRolesAsync(roleIds, options);

        /// <inheritdoc/>
        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null) => GuildUser.Value.AddRolesAsync(roles, options);

        /// <inheritdoc/>
        public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null) => GuildUser.Value.RemoveRoleAsync(roleId, options);

        /// <inheritdoc/>
        public Task RemoveRoleAsync(IRole role, RequestOptions options = null) => GuildUser.Value.RemoveRoleAsync(role, options);

        /// <inheritdoc/>
        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null) => GuildUser.Value.RemoveRolesAsync(roleIds, options);

        /// <inheritdoc/>
        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null) => GuildUser.Value.RemoveRolesAsync(roles, options);
        /// <inheritdoc/>
        public Task SetTimeOutAsync(TimeSpan span, RequestOptions options = null) => GuildUser.Value.SetTimeOutAsync(span, options);

        /// <inheritdoc/>
        public Task RemoveTimeOutAsync(RequestOptions options = null) => GuildUser.Value.RemoveTimeOutAsync(options);

        /// <inheritdoc/>
        IThreadChannel IThreadUser.Thread => Thread.Value;

        /// <inheritdoc/>
        IGuild IThreadUser.Guild => Guild.Value;

        /// <inheritdoc/>
        IGuild IGuildUser.Guild => Guild.Value;

        /// <inheritdoc/>
        ulong IGuildUser.GuildId => Guild.Value.Id;

        /// <inheritdoc/>
        GuildPermissions IGuildUser.GuildPermissions => GuildUser.Value.GuildPermissions;

        /// <inheritdoc/>
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => GuildUser.Value.Roles.Select(x => x.Id).ToImmutableArray();

        /// <inheritdoc />
        string IGuildUser.GetDisplayAvatarUrl(ImageFormat format, ushort size) => GuildUser.Value.GetDisplayAvatarUrl(format, size);

        /// <inheritdoc />
        string IGuildUser.GetGuildAvatarUrl(ImageFormat format, ushort size) => GuildUser.Value.GetGuildAvatarUrl(format, size);

        internal override LazyCached<SocketPresence> Presence { get => GuildUser.Value.Presence; set => GuildUser.Value.Presence = value; }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        /// <summary>
        ///     Gets the guild user of this thread user.
        /// </summary>
        /// <param name="user"></param>
        public static explicit operator SocketGuildUser(SocketThreadUser user) => user.GuildUser.Value;

        #region Cache
        private class CacheModel : Model
        {
            public ulong? ThreadId { get; set; }
            public ulong? UserId { get; set; }
            public DateTimeOffset JoinedAt { get; set; }

            ulong IEntityModel<ulong>.Id { get => UserId.GetValueOrDefault(); set => throw new NotSupportedException(); }
        }

        internal new Model ToModel() => ToModel<CacheModel>();

        internal new TModel ToModel<TModel>() where TModel : Model, new()
        {
            return new TModel
            {
                JoinedAt = ThreadJoinedAt,
                ThreadId = _threadId,
                UserId = Id
            };
        }

        Model ICached<Model>.ToModel() => ToModel();
        TResult ICached<Model>.ToModel<TResult>() => ToModel<TResult>();
        void ICached<Model>.Update(Model model) => Update(model);
        #endregion
    }
}
