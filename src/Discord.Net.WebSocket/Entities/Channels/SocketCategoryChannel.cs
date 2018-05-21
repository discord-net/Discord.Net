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
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => Permissions.GetValue(
               Permissions.ResolveChannel(Guild, x, this, Permissions.ResolveGuild(Guild, x)),
               ChannelPermission.ViewChannel)).ToImmutableArray();

        public IReadOnlyCollection<SocketGuildChannel> Channels
            => Guild.Channels.Where(x => x.CategoryId == Id).ToImmutableArray();

        internal SocketCategoryChannel(DiscordSocketClient discord, ulong id, SocketGuild guild)
            : base(discord, id, guild)
        {
        }
        internal new static SocketCategoryChannel Create(SocketGuild guild, ClientState state, Model model)
        {
            var entity = new SocketCategoryChannel(guild.Discord, model.Id, guild);
            entity.Update(state, model);
            return entity;
        }

        //Users
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

        // IGuildChannel
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IGuildUser>>(Users).ToAsyncEnumerable();
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IGuildUser>(GetUser(id));
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
        Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
            => throw new NotSupportedException();
        /// <inheritdoc />
        /// <exception cref="NotSupportedException">This method is not supported with category channels.</exception>
        Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => throw new NotSupportedException();

        //IChannel
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>(Users).ToAsyncEnumerable();
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(GetUser(id));
    }
}
