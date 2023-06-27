using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public abstract class SocketChannel : SocketCacheableEntity<ulong, IChannelModel>, IChannel
    {
        public string Name
            => Model.Name;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        internal SocketChannel(DiscordSocketClient discord, ulong id, IChannelModel model)
            : base(discord, id)
        {

        }

        public Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions? options = null) => throw new NotImplementedException();
    }
}
