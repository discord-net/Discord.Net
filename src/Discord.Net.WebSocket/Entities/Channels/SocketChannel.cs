using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Channel;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketChannel : SocketEntity<ulong>, IChannel
    {
        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public IReadOnlyCollection<SocketUser> Users => GetUsersInternal();

        internal SocketChannel(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static ISocketPrivateChannel CreatePrivate(DiscordSocketClient discord, ClientState state, Model model)
        {
            switch (model.Type)
            {
                case ChannelType.DM:
                    return SocketDMChannel.Create(discord, state, model);
                case ChannelType.Group:
                    return SocketGroupChannel.Create(discord, state, model);
                default:
                    throw new InvalidOperationException($"Unexpected channel type: {model.Type}");
            }
        }
        internal abstract void Update(ClientState state, Model model);

        //User
        public SocketUser GetUser(ulong id) => GetUserInternal(id);
        internal abstract SocketUser GetUserInternal(ulong id);
        internal abstract IReadOnlyCollection<SocketUser> GetUsersInternal();

        internal SocketChannel Clone() => MemberwiseClone() as SocketChannel;

        //IChannel
        string IChannel.Name => null;

        Task<IUser> IChannel.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
            => Task.FromResult<IUser>(null); //Overridden
        IAsyncEnumerable<IReadOnlyCollection<IUser>> IChannel.GetUsersAsync(CacheMode mode, RequestOptions options)
            => AsyncEnumerable.Empty<IReadOnlyCollection<IUser>>(); //Overridden
    }
}
