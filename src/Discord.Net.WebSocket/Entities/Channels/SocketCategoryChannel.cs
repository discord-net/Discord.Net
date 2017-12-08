using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio;
using Discord.Rest;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketCategoryChannel : SocketGuildChannel, ICategoryChannel
    {
        public override IReadOnlyCollection<SocketGuildUser> Users
            => Guild.Users.Where(x => x.VoiceChannel?.Id == Id).ToImmutableArray();

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

        private string DebuggerDisplay => $"{Name} ({Id}, Category)";
        internal new SocketCategoryChannel Clone() => MemberwiseClone() as SocketCategoryChannel;

        // IGuildChannel
        IAsyncEnumerable<IReadOnlyCollection<IGuildUser>> IGuildChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
        Task<IGuildUser> IGuildChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
        Task<IInviteMetadata> IGuildChannel.CreateInviteAsync(int? maxAge, int? maxUses, bool isTemporary, bool isUnique, RequestOptions options)
            => throw new NotSupportedException();
        Task<IReadOnlyCollection<IInviteMetadata>> IGuildChannel.GetInvitesAsync(RequestOptions options)
            => throw new NotSupportedException();

        //IChannel
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => throw new NotSupportedException();
    }
}
