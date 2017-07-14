using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    //TODO: Add event docstrings
    public partial class DiscordShardedClient
    {
        /// <summary>
        /// Fired when a shard is connected to the Discord gateway.
        /// </summary>
        public event Func<DiscordSocketClient, Task> ShardConnected {
            add { _shardConnectedEvent.Add(value); }
            remove { _shardConnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<DiscordSocketClient, Task>> _shardConnectedEvent = new AsyncEvent<Func<DiscordSocketClient, Task>>();
        /// <summary>
        /// Fired when a shard is disconnected from the Discord gateway.
        /// </summary>
        public event Func<DiscordSocketClient, Exception, Task> ShardDisconnected {
            add { _shardDisconnectedEvent.Add(value); }
            remove { _shardDisconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<DiscordSocketClient, Exception, Task>> _shardDisconnectedEvent = new AsyncEvent<Func<DiscordSocketClient, Exception, Task>>();
    }
}
