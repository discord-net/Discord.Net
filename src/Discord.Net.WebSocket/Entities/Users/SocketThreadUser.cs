using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model = Discord.API.ThreadMember;
using MemberModel = Discord.API.GuildMember;
using Discord.API;
using System.Collections.Immutable;

namespace Discord.WebSocket
{
    public class SocketThreadUser : IGuildUser
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

        /// <inheritdoc/>
        public string AvatarId
            => GuildUser.AvatarId;

        /// <inheritdoc/>
        public string Discriminator
            => GuildUser.Discriminator;

        /// <inheritdoc/>
        public ushort DiscriminatorValue
            => GuildUser.DiscriminatorValue;

        /// <inheritdoc/>
        public bool IsBot
            => GuildUser.IsBot;

        /// <inheritdoc/>
        public bool IsWebhook
            => GuildUser.IsWebhook;

        /// <inheritdoc/>
        public string Username
            => GuildUser.Username;

        /// <inheritdoc/>
        public UserProperties? PublicFlags
            => GuildUser.PublicFlags;

        /// <inheritdoc/>
        public DateTimeOffset CreatedAt
            => GuildUser.CreatedAt;

        /// <inheritdoc/>
        public ulong Id
            => GuildUser.Id;

        /// <inheritdoc/>
        public string Mention
            => GuildUser.Mention;

        /// <inheritdoc/>
        public UserStatus Status
            => GuildUser.Status;

        /// <inheritdoc/>
        public IImmutableSet<ClientType> ActiveClients
            => GuildUser.ActiveClients;

        /// <inheritdoc/>
        public IImmutableList<IActivity> Activities
            => GuildUser.Activities;

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

        private SocketGuildUser GuildUser { get; set; }

        internal SocketThreadUser(SocketGuild guild, SocketThreadChannel thread, SocketGuildUser member)
        {
            this.Thread = thread;
            this.Guild = guild;
            this.GuildUser = member;
        }

        internal SocketThreadUser Create(SocketGuild guild, SocketThreadChannel thread, Model model, SocketGuildUser member)
        {
            var entity = new SocketThreadUser(guild, thread, member);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            this.ThreadJoinedAt = model.JoinTimestamp;
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
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128) => GuildUser.GetAvatarUrl(format, size);

        /// <inheritdoc/>
        public string GetDefaultAvatarUrl() => GuildUser.GetDefaultAvatarUrl();

        /// <inheritdoc/>
        public Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null) => GuildUser.CreateDMChannelAsync(options);

        /// <inheritdoc/>
        GuildPermissions IGuildUser.GuildPermissions => this.GuildUser.GuildPermissions;

        /// <inheritdoc/>
        IGuild IGuildUser.Guild => this.Guild;

        /// <inheritdoc/>
        ulong IGuildUser.GuildId => this.Guild.Id;

        /// <inheritdoc/>
        IReadOnlyCollection<ulong> IGuildUser.RoleIds => this.GuildUser.Roles.Select(x => x.Id).ToImmutableArray();

        /// <summary>
        ///     Gets the guild user of this thread user.
        /// </summary>
        /// <param name="user"></param>
        public static explicit operator SocketGuildUser(SocketThreadUser user) => user.GuildUser;
    }
}
