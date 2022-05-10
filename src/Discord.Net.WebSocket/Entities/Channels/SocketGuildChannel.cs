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
        #region SocketGuildChannel
        private ImmutableArray<Overwrite> _overwrites;

        /// <summary>
        ///     Gets the guild associated with this channel.
        /// </summary>
        /// <returns>
        ///     A guild object that this channel belongs to.
        /// </returns>
        public SocketGuild Guild { get; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public int Position { get; private set; }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<Overwrite> PermissionOverwrites => _overwrites;
        /// <summary>
        ///     Gets a collection of users that are able to view the channel.
        /// </summary>
        /// <remarks>
        ///     If this channel is a voice channel, a collection of users who are currently connected to this channel
        ///     is returned.
        /// </remarks>
        /// <returns>
        ///     A read-only collection of users that can access the channel (i.e. the users seen in the user list).
        /// </returns>
        public new virtual IReadOnlyCollection<SocketGuildUser> Users => ImmutableArray.Create<SocketGuildUser>();

        internal SocketGuildChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static SocketGuildChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            return model.Type switch
            {
                ChannelType.News => SocketNewsChannel.Create(guild, state, model),
                ChannelType.Text => SocketTextChannel.Create(guild, state, model),
                ChannelType.Voice => SocketVoiceChannel.Create(guild, state, model),
                ChannelType.Category => SocketCategoryChannel.Create(guild, state, model),
                ChannelType.PrivateThread or ChannelType.PublicThread or ChannelType.NewsThread => SocketThreadChannel.Create(guild, state, model),
                ChannelType.Stage => SocketStageChannel.Create(guild, state, model),
                _ => new SocketGuildChannel(guild.Discord, model.Id, guild),
            };
        }
        /// <inheritdoc />
        internal override void Update(ClientState state, Model model)
        {
            Name = model.Name.Value;
            Position = model.Position.GetValueOrDefault(0);

            var overwrites = model.PermissionOverwrites.GetValueOrDefault(new API.Overwrite[0]);
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
        ///     Gets the permission overwrite for a specific user.
        /// </summary>
        /// <param name="user">The user to get the overwrite from.</param>
        /// <returns>
        ///     An overwrite object for the targeted user; <c>null</c> if none is set.
        /// </returns>
        public virtual OverwritePermissions? GetPermissionOverwrite(IUser user)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == user.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }
        /// <summary>
        ///     Gets the permission overwrite for a specific role.
        /// </summary>
        /// <param name="role">The role to get the overwrite from.</param>
        /// <returns>
        ///     An overwrite object for the targeted role; <c>null</c> if none is set.
        /// </returns>
        public virtual OverwritePermissions? GetPermissionOverwrite(IRole role)
        {
            for (int i = 0; i < _overwrites.Length; i++)
            {
                if (_overwrites[i].TargetId == role.Id)
                    return _overwrites[i].Permissions;
            }
            return null;
        }

        /// <summary>
        ///     Adds or updates the permission overwrite for the given user.
        /// </summary>
        /// <param name="user">The user to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the user.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
        /// </returns>
        public virtual async Task AddPermissionOverwriteAsync(IUser user, OverwritePermissions permissions, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, user, permissions, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Adds or updates the permission overwrite for the given role.
        /// </summary>
        /// <param name="role">The role to add the overwrite to.</param>
        /// <param name="permissions">The overwrite to add to the role.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous permission operation for adding the specified permissions to the channel.
        /// </returns>
        public virtual async Task AddPermissionOverwriteAsync(IRole role, OverwritePermissions permissions, RequestOptions options = null)
        {
            await ChannelHelper.AddPermissionOverwriteAsync(this, Discord, role, permissions, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Removes the permission overwrite for the given user, if one exists.
        /// </summary>
        /// <param name="user">The user to remove the overwrite from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
        /// </returns>
        public virtual async Task RemovePermissionOverwriteAsync(IUser user, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, user, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Removes the permission overwrite for the given role, if one exists.
        /// </summary>
        /// <param name="role">The role to remove the overwrite from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task representing the asynchronous operation for removing the specified permissions from the channel.
        /// </returns>
        public virtual async Task RemovePermissionOverwriteAsync(IRole role, RequestOptions options = null)
        {
            await ChannelHelper.RemovePermissionOverwriteAsync(this, Discord, role, options).ConfigureAwait(false);
        }

        public new virtual SocketGuildUser GetUser(ulong id) => null;

        /// <summary>
        ///     Gets the name of the channel.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="SocketGuildChannel.Name"/>.
        /// </returns>
        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}, Guild)";
        internal new SocketGuildChannel Clone() => MemberwiseClone() as SocketGuildChannel;
#endregion

        #region SocketChannel
        /// <inheritdoc />
        internal override IReadOnlyCollection<SocketUser> GetUsersInternal() => Users;
        /// <inheritdoc />
        internal override SocketUser GetUserInternal(ulong id) => GetUser(id);
        #endregion

        #region IGuildChannel
        /// <inheritdoc />
        IGuild IGuildChannel.Guild => Guild;
        /// <inheritdoc />
        ulong IGuildChannel.GuildId => Guild.Id;

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
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable(); //Overridden in Text/Voice
        /// <inheritdoc />
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id)); //Overridden in Text/Voice
        #endregion

        #region IChannel
        /// <inheritdoc />
        string IChannel.Name => Name;
        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable(); //Overridden in Text/Voice
        /// <inheritdoc />
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id)); //Overridden in Text/Voice
        #endregion
    }
}
