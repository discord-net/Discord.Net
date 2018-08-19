using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public partial class DiscordSocketClient
    {
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();

        //General
        /// <summary> Fired when connected to the Discord gateway. </summary>
        public event Func<Task> Connected
        {
            add => _connectedEvent.Add(value);
            remove => _connectedEvent.Remove(value);
        }

        /// <summary> Fired when disconnected to the Discord gateway. </summary>
        public event Func<Exception, Task> Disconnected
        {
            add => _disconnectedEvent.Add(value);
            remove => _disconnectedEvent.Remove(value);
        }

        /// <summary> Fired when guild data has finished downloading. </summary>
        public event Func<Task> Ready
        {
            add => _readyEvent.Add(value);
            remove => _readyEvent.Remove(value);
        }

        /// <summary> Fired when a heartbeat is received from the Discord gateway. </summary>
        public event Func<int, int, Task> LatencyUpdated
        {
            add => _latencyUpdatedEvent.Add(value);
            remove => _latencyUpdatedEvent.Remove(value);
        }
    }
}
