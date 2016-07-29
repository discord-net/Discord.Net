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
        public event Func<Task> VoiceStateUpdated
        {
            add { _voiceStateUpdatedEvent.Add(value); }
            remove { _voiceStateUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _voiceStateUpdatedEvent = new AsyncEvent<Func<Task>>();

        //Messages
        public event Func<Task> MessageReceived
        {
            add { _messageReceivedEvent.Add(value); }
            remove { _messageReceivedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _messageReceivedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> MessageUpdated
        {
            add { _messageUpdatedEvent.Add(value); }
            remove { _messageUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _messageUpdatedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> MessageDeleted
        {
            add { _messageDeletedEvent.Add(value); }
            remove { _messageDeletedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _messageDeletedEvent = new AsyncEvent<Func<Task>>();
    }
}
