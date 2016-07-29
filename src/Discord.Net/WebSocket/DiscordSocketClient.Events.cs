using System;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    //TODO: Add event docstrings
    public partial class DiscordSocketClient
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
        public event Func<int, int, Task> LatencyUpdated
        {
            add { _latencyUpdatedEvent.Add(value); }
            remove { _latencyUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();

        //Channels
        public event Func<IChannel, Task> ChannelCreated
        {
            add { _channelCreatedEvent.Add(value); }
            remove { _channelCreatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IChannel, Task>> _channelCreatedEvent = new AsyncEvent<Func<IChannel, Task>>();
        public event Func<IChannel, Task> ChannelDestroyed
        {
            add { _channelDestroyedEvent.Add(value); }
            remove { _channelDestroyedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IChannel, Task>> _channelDestroyedEvent = new AsyncEvent<Func<IChannel, Task>>();
        public event Func<IChannel, IChannel, Task> ChannelUpdated
        {
            add { _channelUpdatedEvent.Add(value); }
            remove { _channelUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IChannel, IChannel, Task>> _channelUpdatedEvent = new AsyncEvent<Func<IChannel, IChannel, Task>>();

        //Messages
        public event Func<IMessage, Task> MessageReceived
        {
            add { _messageReceivedEvent.Add(value); }
            remove { _messageReceivedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IMessage, Task>> _messageReceivedEvent = new AsyncEvent<Func<IMessage, Task>>();
        public event Func<ulong, Optional<IMessage>, Task> MessageDeleted
        {
            add { _messageDeletedEvent.Add(value); }
            remove { _messageDeletedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Optional<IMessage>, Task>> _messageDeletedEvent = new AsyncEvent<Func<ulong, Optional<IMessage>, Task>>();
        public event Func<Optional<IMessage>, IMessage, Task> MessageUpdated
        {
            add { _messageUpdatedEvent.Add(value); }
            remove { _messageUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Optional<IMessage>, IMessage, Task>> _messageUpdatedEvent = new AsyncEvent<Func<Optional<IMessage>, IMessage, Task>>();

        //Roles
        public event Func<IRole, Task> RoleCreated
        {
            add { _roleCreatedEvent.Add(value); }
            remove { _roleCreatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IRole, Task>> _roleCreatedEvent = new AsyncEvent<Func<IRole, Task>>();
        public event Func<IRole, Task> RoleDeleted
        {
            add { _roleDeletedEvent.Add(value); }
            remove { _roleDeletedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IRole, Task>> _roleDeletedEvent = new AsyncEvent<Func<IRole, Task>>();
        public event Func<IRole, IRole, Task> RoleUpdated
        {
            add { _roleUpdatedEvent.Add(value); }
            remove { _roleUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IRole, IRole, Task>> _roleUpdatedEvent = new AsyncEvent<Func<IRole, IRole, Task>>();

        //Guilds
        public event Func<IGuild, Task> JoinedGuild
        {
            add { _joinedGuildEvent.Add(value); }
            remove { _joinedGuildEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, Task>> _joinedGuildEvent = new AsyncEvent<Func<IGuild, Task>>();
        public event Func<IGuild, Task> LeftGuild
        {
            add { _leftGuildEvent.Add(value); }
            remove { _leftGuildEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, Task>> _leftGuildEvent = new AsyncEvent<Func<IGuild, Task>>();
        public event Func<IGuild, Task> GuildAvailable
        {
            add { _guildAvailableEvent.Add(value); }
            remove { _guildAvailableEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, Task>> _guildAvailableEvent = new AsyncEvent<Func<IGuild, Task>>();
        public event Func<IGuild, Task> GuildUnavailable
        {
            add { _guildUnavailableEvent.Add(value); }
            remove { _guildUnavailableEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, Task>> _guildUnavailableEvent = new AsyncEvent<Func<IGuild, Task>>();
        public event Func<IGuild, Task> GuildMembersDownloaded
        {
            add { _guildMembersDownloadedEvent.Add(value); }
            remove { _guildMembersDownloadedEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, Task>> _guildMembersDownloadedEvent = new AsyncEvent<Func<IGuild, Task>>();
        public event Func<IGuild, IGuild, Task> GuildUpdated
        {
            add { _guildUpdatedEvent.Add(value); }
            remove { _guildUpdatedEvent.Remove(value); }
        }
        private AsyncEvent<Func<IGuild, IGuild, Task>> _guildUpdatedEvent = new AsyncEvent<Func<IGuild, IGuild, Task>>();

        //Users
        public event Func<IGuildUser, Task> UserJoined
        {
            add { _userJoinedEvent.Add(value); }
            remove { _userJoinedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGuildUser, Task>> _userJoinedEvent = new AsyncEvent<Func<IGuildUser, Task>>();
        public event Func<IGuildUser, Task> UserLeft
        {
            add { _userLeftEvent.Add(value); }
            remove { _userLeftEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGuildUser, Task>> _userLeftEvent = new AsyncEvent<Func<IGuildUser, Task>>();
        public event Func<IUser, IGuild, Task> UserBanned
        {
            add { _userBannedEvent.Add(value); }
            remove { _userBannedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IUser, IGuild, Task>> _userBannedEvent = new AsyncEvent<Func<IUser, IGuild, Task>>();
        public event Func<IUser, IGuild, Task> UserUnbanned
        {
            add { _userUnbannedEvent.Add(value); }
            remove { _userUnbannedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IUser, IGuild, Task>> _userUnbannedEvent = new AsyncEvent<Func<IUser, IGuild, Task>>();
        public event Func<IGuildUser, IGuildUser, Task> UserUpdated
        {
            add { _userUpdatedEvent.Add(value); }
            remove { _userUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGuildUser, IGuildUser, Task>> _userUpdatedEvent = new AsyncEvent<Func<IGuildUser, IGuildUser, Task>>();
        public event Func<IGuildUser, IPresence, IPresence, Task> UserPresenceUpdated
        {
            add { _userPresenceUpdatedEvent.Add(value); }
            remove { _userPresenceUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGuildUser, IPresence, IPresence, Task>> _userPresenceUpdatedEvent = new AsyncEvent<Func<IGuildUser, IPresence, IPresence, Task>>();
        public event Func<IUser, IVoiceState, IVoiceState, Task> UserVoiceStateUpdated
        {
            add { _userVoiceStateUpdatedEvent.Add(value); }
            remove { _userVoiceStateUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IUser, IVoiceState, IVoiceState, Task>> _userVoiceStateUpdatedEvent = new AsyncEvent<Func<IUser, IVoiceState, IVoiceState, Task>>();
        public event Func<ISelfUser, ISelfUser, Task> CurrentUserUpdated
        {
            add { _selfUpdatedEvent.Add(value); }
            remove { _selfUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ISelfUser, ISelfUser, Task>> _selfUpdatedEvent = new AsyncEvent<Func<ISelfUser, ISelfUser, Task>>();
        public event Func<IUser, IChannel, Task> UserIsTyping
        {
            add { _userIsTypingEvent.Add(value); }
            remove { _userIsTypingEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IUser, IChannel, Task>> _userIsTypingEvent = new AsyncEvent<Func<IUser, IChannel, Task>>();
        public event Func<IGroupUser, Task> RecipientAdded
        {
            add { _recipientAddedEvent.Add(value); }
            remove { _recipientAddedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGroupUser, Task>> _recipientAddedEvent = new AsyncEvent<Func<IGroupUser, Task>>();
        public event Func<IGroupUser, Task> RecipientRemoved
        {
            add { _recipientRemovedEvent.Add(value); }
            remove { _recipientRemovedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<IGroupUser, Task>> _recipientRemovedEvent = new AsyncEvent<Func<IGroupUser, Task>>();

        //TODO: Add PresenceUpdated? VoiceStateUpdated?, VoiceConnected, VoiceDisconnected;
    }
}
