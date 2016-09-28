using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketChannel : SocketEntity<ulong>, IChannel
    {
        internal SocketChannel(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketChannel Create(DiscordSocketClient discord, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return SocketTextChannel.Create(discord, model);
                case ChannelType.Voice:
                    return SocketVoiceChannel.Create(discord, model);
                case ChannelType.DM:
                    return SocketDMChannel.Create(discord, model);
                case ChannelType.Group:
                    return SocketGroupChannel.Create(discord, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }

        //IChannel
        IReadOnlyCollection<IUser> IChannel.CachedUsers => ImmutableArray.Create<IUser>();

        IUser IChannel.GetCachedUser(ulong id)
            => null;
        Task<IUser> IChannel.GetUserAsync(ulong id)
            => Task.FromResult<IUser>(null);
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync()
            => ImmutableArray.Create<IReadOnlyCollection<IUser>>().ToAsyncEnumerable();
    }
}
