using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based guild channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketGuildChannel : SocketChannel, IGuildChannel
    {
        private ImmutableArray<Overwrite> _overwrites;

        /// <summary>
        ///     Gets the guild associated with this channel.
        /// </summary>
        /// <returns>
        ///     A guild that this channel belongs to.
        /// </returns>
        public SocketGuild Guild { get; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }        

        /// <inheritdoc />
        public IReadOnlyCollection<Overwrite> PermissionOverwrites => _overwrites;
        /// <summary>
        ///     Gets a collection of users that are able to view the channel.
        /// </summary>
        /// <returns>
        ///     A collection of users that can access the channel (i.e. the users seen in the user list).
        /// </returns>
        public new virtual IReadOnlyCollection<SocketGuildUser> Users => ImmutableArray.Create<SocketGuildUser>();

        internal SocketGuildChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static SocketGuildChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return SocketTextChannel.Create(guild, state, model);
                case ChannelType.Voice:
                    return SocketVoiceChannel.Create(guild, state, model);
                case ChannelType.Category:
                    return SocketCategoryChannel.Create(guild, state, model);
                default:
                    // TODO: Proper implementation for channel categories
                    return new SocketGuildChannel(guild.Discord, model.Id, guild);
            }
        }
        internal override void Update(ClientState state, Model model)
        {
            Name = model.Name.Value;
            Position = model.Position.Value;
            
            var overwrites = model.PermissionOverwrites.Value;
            var newOverwrites = ImmutableArray.CreateBuilder<Overwrite>(overwrites.Length);
            for (int i = 0; i < overwrites.Length; i++)
                newOverwrites.Add(overwrites[i].ToEntity());
            _overwrites = newOverwrites.ToImmutable();
        }

        /// <inheritdoc />
        public Task ModifyAsync(Action<GuildChannelProperties> func, RequestOptions options = null)
            => ChannelHelper.ModifyAsync(this, Discord, func, options);
        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => ChannelHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the overwrite permissions of the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user that you want to get the overwrite permissions for.</param>
        /// <returns>
        ///     The overwrite permissions for the requested user; otherwise <c>null</c>.
        /// </returns>
        public OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }

        /// <summary>
        ///     Gets the overwrite permissions of the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role that you want to get the overwrite permissions for.</param>
        /// <returns>
        ///     The overwrite permissions for the requested role; otherwise <c>null</c>.
        /// </returns>
        public OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }

        /// <summary>
        ///     Adds an overwrite permission for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user you want the overwrite permission to apply to.</param>
        /// <param name="perms">The overwrite permission you want to add.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions perms, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, user, perms, options).ConfigureAwait(false);
            _overwrites = _overwrites.Add(new Overwrite(user.Id, PermissionTarget.User, new OverwritePermissions(perms.AllowValue, perms.DenyValue)));
        }

        /// <summary>
        ///     Adds an overwrite permission for the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role you want the overwrite permission to apply to.</param>
        /// <param name="perms">The overwrite permission you want to add.</param>
        /// <param name="options">The options to be used when sending the request. </param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions perms, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, role, perms, options).ConfigureAwait(false);
            _overwrites = _overwrites.Add(new Overwrite(role.Id, PermissionTarget.Role, new OverwritePermissions(perms.AllowValue, perms.DenyValue)));
        }
        /// <summary>
        ///     Removes an overwrite permission for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user you want to remove the overwrite permission from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, user, options).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }
        /// <summary>
        ///     Removes an overwrite permission for the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role you want the overwrite permissions to be removed from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, role, options).ConfigureAwait(false);

            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                {
                    _overwrites = _overwrites.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        ///     Gets the invites for this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task<IReadOnlyCollection<RestInviteMetadata>> GetInvitesAsync(RequestOptions options = null)
            => await ChannelHelper.GetInvitesAsync(this, Discord, options).ConfigureAwait(false);
        /// <summary>
        ///     Creates an invite for this channel.
        /// </summary>
        /// <param name="maxAge">The number of seconds that you want the invite to be valid for.</param>
        /// <param name="maxUses">The number of times this invite can be used before it expires.</param>
        /// <param name="isTemporary">Whether or not the invite grants temporary membership.</param>
        /// <param name="isUnique">Whether to try reuse a similar invite or not.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable <see cref="Task"/>.
        /// </returns>
        public async Task<RestInviteMetadata> CreateInviteAsync(int? maxAge = 86400, int? maxUses = null, bool isTemporary = false, bool isUnique = false, RequestOptions options = null)
            => await ChannelHelper.CreateInviteAsync(this, Discord, maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        public new virtual SocketGuildUser GetUser(ulong id) => null;

        /// <summary>
        ///     Gets the name of the channel.
        /// </summary>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, Guild)";
        internal new SocketGuildChannel Clone() => MemberwiseClone() as SocketGuildChannel;

        //SocketChannel
        internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
        internal override SocketUser GetUserInternal(ulong id) => GetUser(id);

        //IGuildChannel
        /// <inheritdoc />
        IGuild IGuildChannel.Guild => Guild;
        /// <inheritdoc />
        ulong IGuildChannel.GuildId => Guild.Id;

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => await GetInvitesAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
            => await CreateInviteAsync(maxAge, maxUses, isTemporary, isUnique, options).ConfigureAwait(false);

        /// <inheritdoc />
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IRole role)
            => GetPermissionOverwrite(role);
        /// <inheritdoc />
        OverwritePermissions? IGuildChannel.GetPermissionOverwrite(IUser user)
            => GetPermissionOverwrite(user);
        /// <inheritdoc />
        async Task IGuildChannel.AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options)
            => await AddPermissionOverwriteAsync(role, permissions, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options)
            => await AddPermissionOverwriteAsync(user, permissions, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IRole role, RequestOptions options)
            => await RemovePermissionOverwriteAsync(role, options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task IGuildChannel.RemovePermissionOverwriteAsync(IUser user, RequestOptions options)
            => await RemovePermissionOverwriteAsync(user, options).ConfigureAwait(false);

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));

        //IChannel
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable(); //Overridden in Text/Voice
        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id)); //Overridden in Text/Voice
    }
}
