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
        public event Func<RpcVoiceState, Task> VoiceStateCreated
        {
            add { _voiceStateCreatedEvent.Add(value); }
            remove { _voiceStateCreatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<RpcVoiceState, Task>> _voiceStateCreatedEvent = new AsyncEvent<Func<RpcVoiceState, Task>>();

        public event Func<RpcVoiceState, Task> VoiceStateUpdated
        {
            add { _voiceStateUpdatedEvent.Add(value); }
            remove { _voiceStateUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<RpcVoiceState, Task>> _voiceStateUpdatedEvent = new AsyncEvent<Func<RpcVoiceState, Task>>();

        public event Func<RpcVoiceState, Task> VoiceStateDeleted
        {
            add { _voiceStateDeletedEvent.Add(value); }
            remove { _voiceStateDeletedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<RpcVoiceState, Task>> _voiceStateDeletedEvent = new AsyncEvent<Func<RpcVoiceState, Task>>();

        public event Func<ulong, Task> SpeakingStarted
        {
            add { _speakingStartedEvent.Add(value); }
            remove { _speakingStartedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Task>> _speakingStartedEvent = new AsyncEvent<Func<ulong, Task>>();
        public event Func<ulong, Task> SpeakingStopped
        {
            add { _speakingStoppedEvent.Add(value); }
            remove { _speakingStoppedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Task>> _speakingStoppedEvent = new AsyncEvent<Func<ulong, Task>>();

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
