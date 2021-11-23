using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.ThreadMember;
using System.Collections.Immutable;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a thread user received over the gateway.
    /// </summary>
    public class SocketThreadUser : SocketUser, IGuildUser
    {
        /// <summary>
        ///     Gets the <see cref="SocketThreadChannel"/> this user is in.
        /// </summary>
        public SocketThreadChannel Thread { get; private set; }

        /// <summary>
        ///     Gets the timestamp for when this user joined this thread.
        /// </summary>
        public DateTimeOffset ThreadJoinedAt { get; private set; }

        /// <summary>
        ///     Gets the guild this user is in.
        /// </summary>
        public SocketGuild Guild { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset? JoinedAt
            => GuildUser.JoinedAt;

        /// <inheritdoc/>
        public string Nickname
            => GuildUser.Nickname;

        /// <inheritdoc/>
        public DateTimeOffset? PremiumSince
            => GuildUser.PremiumSince;

        /// <inheritdoc/>
        public bool? IsPending
            => GuildUser.IsPending;
        /// <inheritdoc />
        public int Hierarchy
            => GuildUser.Hierarchy;

        /// <inheritdoc/>
        public override string AvatarId
        {
            get => GuildUser.AvatarId;
            internal set => GuildUser.AvatarId = value;
        }
        /// <inheritdoc/>
        public string GuildAvatarId
            => GuildUser.GuildAvatarId;

        /// <inheritdoc/>
        public override ushort DiscriminatorValue
        {
            get => GuildUser.DiscriminatorValue;
            internal set => GuildUser.DiscriminatorValue = value;
        }

        /// <inheritdoc/>
        public override bool IsBot
        {
            get => GuildUser.IsBot;
            internal set => GuildUser.IsBot = value;
        }

        /// <inheritdoc/>
        public override bool IsWebhook
            => GuildUser.IsWebhook;

        /// <inheritdoc/>
        public override string Username
        {
            get => GuildUser.Username;
            internal set => GuildUser.Username = value;
        }

        /// <inheritdoc/>
        public bool IsDeafened
            => GuildUser.IsDeafened;

        /// <inheritdoc/>
        public bool IsMuted
            => GuildUser.IsMuted;

        /// <inheritdoc/>
        public bool IsSelfDeafened
            => GuildUser.IsSelfDeafened;

        /// <inheritdoc/>
        public bool IsSelfMuted
            => GuildUser.IsSelfMuted;

        /// <inheritdoc/>
        public bool IsSuppressed
            => GuildUser.IsSuppressed;

        /// <inheritdoc/>
        public IVoiceChannel VoiceChannel
            => GuildUser.VoiceChannel;

        /// <inheritdoc/>
        public string VoiceSessionId
            => GuildUser.VoiceSessionId;

        /// <inheritdoc/>
        public bool IsStreaming
            => GuildUser.IsStreaming;

        /// <inheritdoc/>
        public DateTimeOffset? RequestToSpeakTimestamp
            => GuildUser.RequestToSpeakTimestamp;

        private SocketGuildUser GuildUser { get; set; }

        internal SocketThreadUser(SocketGuild guild, SocketThreadChannel thread, SocketGuildUser member)
            : base(guild.Discord, member.Id)
        {
            Thread = thread;
            Guild = guild;
            GuildUser = member;
        }

        internal static SocketThreadUser Create(SocketGuild guild, SocketThreadChannel thread, Model model, SocketGuildUser member)
        {
            var entity = new SocketThreadUser(guild, thread, member);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            ThreadJoinedAt = model.JoinTimestamp;

            if (model.Presence.IsSpecified)
            {
                GuildUser.Update(Discord.State, model.Presence.Value, true);
            }

            if (model.Member.IsSpecified)
            {
                GuildUser.Update(Discord.State, model.Member.Value);
            }
        }

        /// <inheritdoc/>
        public ChannelPermissions GetPermissions(IGuildChannel channel) => GuildUser.GetPermissions(channel);

        /// <inheritdoc/>
        public Task KickAsync(string reason = null, RequestOptions options = null) => GuildUser.KickAsync(reason, options);

        /// <inheritdoc/>
        public Task ModifyAsync(Action<GuildUserProperties> func, RequestOptions options = null) => GuildUser.ModifyAsync(func, options);

        /// <inheritdoc/>
        public Task AddRoleAsync(ulong roleId, RequestOptions options = null) => GuildUser.AddRoleAsync(roleId, options);

        /// <inheritdoc/>
        public Task AddRoleAsync(IRole role, RequestOptions options = null) => GuildUser.AddRoleAsync(role, options);

        /// <inheritdoc/>
        public Task AddRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null) => GuildUser.AddRolesAsync(roleIds, options);

        /// <inheritdoc/>
        public Task AddRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null) => GuildUser.AddRolesAsync(roles, options);

        /// <inheritdoc/>
        public Task RemoveRoleAsync(ulong roleId, RequestOptions options = null) => GuildUser.RemoveRoleAsync(roleId, options);

        /// <inheritdoc/>
        public Task RemoveRoleAsync(IRole role, RequestOptions options = null) => GuildUser.RemoveRoleAsync(role, options);

        /// <inheritdoc/>
        public Task RemoveRolesAsync(IEnumerable<ulong> roleIds, RequestOptions options = null) => GuildUser.RemoveRolesAsync(roleIds, options);

        /// <inheritdoc/>
        public Task RemoveRolesAsync(IEnumerable<IRole> roles, RequestOptions options = null) => GuildUser.RemoveRolesAsync(roles, options);

        /// <inheritdoc/>
        GuildPermissions IGuildUser.GuildPermissions => GuildUser.GuildPermissions;

        /// <inheritdoc/>
        IGuild IGuildUser.Guild => Guild;

        /// <inheritdoc/>
        ulong IGuildUser.GuildId => Guild.Id;

        /// <inheritdoc/>
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => GuildUser.Roles.Select(x => x.Id).ToImmutableArray();

        string IGuildUser.GetGuildAvatarUrl(ImageFormat format, ushort size) => GuildUser.GetGuildAvatarUrl(format, size);

        internal override SocketGlobalUser GlobalUser => GuildUser.GlobalUser;

        internal override SocketPresence Presence { get => GuildUser.Presence; set => GuildUser.Presence = value; }

        /// <summary>
        ///     Gets the guild user of this thread user.
        /// </summary>
        /// <param name="user"></param>
        public static explicit operator SocketGuildUser(SocketThreadUser user) => user.GuildUser;
    }
}
