using System;
using System.Threading.Tasks;

namespace Discord.Rpc
{
    public partial class DiscordRpcClient
    {
        //General
        public event Func<Task> Connected
        {
            add { _connectedEvent.Add(value); }
            remove { _connectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, Task> Disconnected
        {
            add { _disconnectedEvent.Add(value); }
            remove { _disconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        public event Func<Task> Ready
        {
            add { _readyEvent.Add(value); }
            remove { _readyEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();

        //Guild
        public event Func<Task> GuildUpdated
        {
            add { _guildUpdatedEvent.Add(value); }
            remove { _guildUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _guildUpdatedEvent = new AsyncEvent<Func<Task>>();

        //Voice
        public event Func<ulong, Task> SpeakingStarted
        {
            add { _speakingStarted.Add(value); }
            remove { _speakingStarted.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Task>> _speakingStarted = new AsyncEvent<Func<ulong, Task>>();
        public event Func<ulong, Task> SpeakingStopped
        {
            add { _speakingStopped.Add(value); }
            remove { _speakingStopped.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Task>> _speakingStopped = new AsyncEvent<Func<ulong, Task>>();

        //Messages
        public event Func<RpcMessage, Task> MessageReceived
        {
            add { _messageReceivedEvent.Add(value); }
            remove { _messageReceivedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<RpcMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<RpcMessage, Task>>();
        public event Func<RpcMessage, Task> MessageUpdated
        {
            add { _messageUpdatedEvent.Add(value); }
            remove { _messageUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<RpcMessage, Task>> _messageUpdatedEvent = new AsyncEvent<Func<RpcMessage, Task>>();
        public event Func<ulong, ulong, Task> MessageDeleted
        {
            add { _messageDeletedEvent.Add(value); }
            remove { _messageDeletedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, ulong, Task>> _messageDeletedEvent = new AsyncEvent<Func<ulong, ulong, Task>>();
    }
}
