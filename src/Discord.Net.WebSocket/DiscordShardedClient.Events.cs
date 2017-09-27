using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class DiscordShardedClient
    {
        //General
        /// <summary> Fired when a shard is connected to the Discord gateway. </summary>
        public event Func<DiscordSocketClient, Task> ShardConnected 
        {
            add { _shardConnectedEvent.Add(value); }
            remove { _shardConnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<DiscordSocketClient, Task>> _shardConnectedEvent = new AsyncEvent<Func<DiscordSocketClient, Task>>();
        /// <summary> Fired when a shard is disconnected from the Discord gateway. </summary>
        public event Func<Exception, DiscordSocketClient, Task> ShardDisconnected 
        {
            add { _shardDisconnectedEvent.Add(value); }
            remove { _shardDisconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Exception, DiscordSocketClient, Task>> _shardDisconnectedEvent = new AsyncEvent<Func<Exception, DiscordSocketClient, Task>>();
        /// <summary> Fired when a guild data for a shard has finished downloading. </summary>
        public event Func<DiscordSocketClient, Task> ShardReady 
        {
            add { _shardReadyEvent.Add(value); }
            remove { _shardReadyEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<DiscordSocketClient, Task>> _shardReadyEvent = new AsyncEvent<Func<DiscordSocketClient, Task>>();
        /// <summary> Fired when a shard receives a heartbeat from the Discord gateway. </summary>
        public event Func<int, int, DiscordSocketClient, Task> ShardLatencyUpdated 
        {
            add { _shardLatencyUpdatedEvent.Add(value); }
            remove { _shardLatencyUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<int, int, DiscordSocketClient, Task>> _shardLatencyUpdatedEvent = new AsyncEvent<Func<int, int, DiscordSocketClient, Task>>();
    }
}