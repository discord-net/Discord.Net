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
    ///     Represents a WebSocket-based category channel.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
    {
        #region SocketCategoryChannel
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => Permissions.GetValue(
               Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
               ChannelPermission.ViewChannel)).ToImmutableArray();

        /// <summary>
        ///     Gets the child channels of this category.
        /// </summary>
        /// <returns>
        ///     A read-only collection of <see cref="SocketGuildChannel" /> whose
        ///     <see cref="Discord.INestedChannel.CategoryId" /> matches the snowflake identifier of this category
        ///     channel.
        /// </returns>
        public IReadOnlyCollection<SocketGuildChannel> Channels
            => Guild.Channels.Where(x => x is INestedChannel nestedChannel && nestedChannel.CategoryId == Id).ToImmutableArray();

        internal SocketCategoryChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
        }
        internal new static SocketCategoryChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketCategoryChannel(guild?.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }
        #endregion

        #region Users
        /// <inheritdoc />
        public override SocketGuildUser GetUser(ulong id)
        {
            var user = Guild.GetUser(id);
            if (user != null)
            {
                var guildPerms = Permissions.ResolveGuild(Guild, user);
                var channelPerms = Permissions.ResolveChannel(Guild, user, this, guildPerms);
                if (Permissions.GetValue(channelPerms, ChannelPermission.ViewChannel))
                    return user;
            }
            return null;
        }

        private string DebuggerDisplay => $"{Name} ({Id}, Category)";
        internal new SocketCategoryChannel Clone() => MemberwiseClone() as SocketCategoryChannel;
        #endregion

        #region IGuildChannel

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode,
            RequestOptions options)
        {
            return mode == CacheMode.AllowDownload
                ? ChannelHelper.GetUsersAsync(this, Guild, Discord, null, null, options)
                : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        }
        /// <inheritdoc />
        async Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            var user = GetUser(id);
            if (user is not null || mode == CacheMode.CacheOnly)
                return user;

            return await ChannelHelper.GetUserAsync(this, Guild, Discord, id, options).ConfigureAwait(false);
        }
        #endregion

        #region IChannel

        /// <inheritdoc />
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
        {
            return mode == CacheMode.AllowDownload
                ? ChannelHelper.GetUsersAsync(this, Guild, Discord, null, null, options)
                : ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        }
        /// <inheritdoc />
        async Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            var user = GetUser(id);
            if (user is not null || mode == CacheMode.CacheOnly)
                return user;

            return await ChannelHelper.GetUserAsync(this, Guild, Discord, id, options).ConfigureAwait(false);
        }
        #endregion
    }
}
