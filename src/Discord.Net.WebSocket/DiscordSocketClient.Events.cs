using System;
using System.Threading.Tasks;
using Discord.API;

namespace Discord.WebSocket
{
    public partial class DiscordSocketClient
    {
        //General
        /// <summary> Fired when connected to the Discord gateway. </summary>
        public event Func<Task> Connected
        {
            add { _connectedEvent.Add(value); }
            remove { _connectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        /// <summary> Fired when disconnected to the Discord gateway. </summary>
        public event Func<Exception, Task> Disconnected
        {
            add { _disconnectedEvent.Add(value); }
            remove { _disconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        /// <summary> Fired when guild data has finished downloading. </summary>
        public event Func<Task> Ready
        {
            add { _readyEvent.Add(value); }
            remove { _readyEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();
        /// <summary> Fired when a heartbeat is received from the Discord gateway. </summary>
        public event Func<int, int, Task> LatencyUpdated
        {
            add { _latencyUpdatedEvent.Add(value); }
            remove { _latencyUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

        internal DiscordSocketClient(DiscordSocketConfig config, DiscordRestApiClient client) : base(config, client)
        {
        }
    }
}
