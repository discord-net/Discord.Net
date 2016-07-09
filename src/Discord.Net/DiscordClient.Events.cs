using System;
using System.Runtime.CompilerServices;

namespace Discord
{
    public partial class DiscordClient
    {
        public event EventHandler Ready = delegate { };
        //public event EventHandler<DisconnectedEventArgs> LoggedOut = delegate { };
        /// <summary>When a new channel is created, relevant to the current user.
        /// Discord API Event name: CHANNEL_CREATE.</summary>
        public event EventHandler<ChannelEventArgs> ChannelCreated = delegate { };
        /// <summary>When a channel relevant to the current user is deleted.
        /// Discord API Event name: CHANNEL_DELETE.</summary>
        public event EventHandler<ChannelEventArgs> ChannelDestroyed = delegate { };
        /// <summary>When a channel is updated. Discord API Event name: CHANNEL_UPDATE.</summary>
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated = delegate { };
        [Obsolete]
        public event EventHandler<MessageEventArgs> MessageAcknowledged = delegate { };
        /// <summary>When a message is deleted. Discord API Event name: MESSAGE_DELETE.</summary>
        public event EventHandler<MessageEventArgs> MessageDeleted = delegate { };
        /// <summary>When a message is created. Discord API Event name: MESSAGE_CREATE.</summary>
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        [Obsolete]
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };
        /// <summary>When a message is updated. Discord API Event name: MESSAGE_UPDATE.</summary>
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated = delegate { };
        /// <summary>When properties about the user change. Discord API Event name: USER_UPDATE.</summary>
        public event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated = delegate { };
        /// <summary>When a server role is created. Discord API Event name: GUILD_ROLE_CREATE.</summary>
        public event EventHandler<RoleEventArgs> RoleCreated = delegate { };
        /// <summary>When a server role is updated. Discord API Event name: GUILD_ROLE_UPDATE.</summary>
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated = delegate { };
        /// <summary>When a server role is deleted. Discord API Event name: GUILD_ROLE_DELETE.</summary>
        public event EventHandler<RoleEventArgs> RoleDeleted = delegate { };
        /// <summary>When the current user joins a new server. Discord API Event name: GUILD_CREATE.</summary>
        public event EventHandler<ServerEventArgs> JoinedServer = delegate { };
        /// <summary>When the user leaves or is removed from a server.
        /// Discord API Event name: GUILD_DELETE.</summary>
        public event EventHandler<ServerEventArgs> LeftServer = delegate { };
        /// <summary>When a server becomes available (current user is initially connecting,
        /// or a server becomes available again).
        /// Discord API Event name: GUILD_CREATE or GUILD_MEMBERS_CHUNK.</summary>
        public event EventHandler<ServerEventArgs> ServerAvailable = delegate { };
        /// <summary>When a server is updated. Discord API Event name: GUILD_UPDATE.</summary>
        public event EventHandler<ServerUpdatedEventArgs> ServerUpdated = delegate { };
        /// <summary>When a server becomes unavailable during a server outage.
        /// Discord API Event name: GUILD_DELETE.</summary>
        public event EventHandler<ServerEventArgs> ServerUnavailable = delegate { };
        /// <summary>When a user is banned from a server. Discord API Event name: GUILD_BAN_ADD.</summary>
        public event EventHandler<UserEventArgs> UserBanned = delegate { };
        /// <summary>When a user starts typing in a channel. Discord API Event name: TYPING_START.</summary>
        public event EventHandler<ChannelUserEventArgs> UserIsTyping = delegate { };
        /// <summary>When a new user joins a server. Discord API Event name: GUILD_MEMBER_ADD.</summary>
        public event EventHandler<UserEventArgs> UserJoined = delegate { };
        /// <summary>When a user is removed from a server (leave/kick/ban).
        /// Discord API Event name: GUILD_MEMBER_REMOVE.</summary>
        public event EventHandler<UserEventArgs> UserLeft = delegate { };
        /// <summary>When a server member is updated.
        /// Discord API Event name: GUILD_MEMBER_UPDATE or PRESENCE_UPDATE or VOICE_STATE_UPDATE.</summary>
        public event EventHandler<UserUpdatedEventArgs> UserUpdated = delegate { };
        /// <summary>When a user is unbanned from a server. Discord API Event name: GUILD_BAN_REMOVE.</summary>
        public event EventHandler<UserEventArgs> UserUnbanned = delegate { };

        private void OnReady()
            => OnEvent(Ready);
        /*private void OnLoggedOut(bool wasUnexpected, Exception ex)
            => OnEvent(LoggedOut, new DisconnectedEventArgs(wasUnexpected, ex));*/
            
        private void OnChannelCreated(Channel channel)
            => OnEvent(ChannelCreated, new ChannelEventArgs(channel));
        private void OnChannelDestroyed(Channel channel)
            => OnEvent(ChannelDestroyed, new ChannelEventArgs(channel));
        private void OnChannelUpdated(Channel before, Channel after)
            => OnEvent(ChannelUpdated, new ChannelUpdatedEventArgs(before, after));

        private void OnMessageAcknowledged(Message msg)
            => OnEvent(MessageAcknowledged, new MessageEventArgs(msg));
        private void OnMessageDeleted(Message msg)
            => OnEvent(MessageDeleted, new MessageEventArgs(msg));
        private void OnMessageReceived(Message msg)
            => OnEvent(MessageReceived, new MessageEventArgs(msg));
        internal void OnMessageSent(Message msg)
            => OnEvent(MessageSent, new MessageEventArgs(msg));
        private void OnMessageUpdated(Message before, Message after)
            => OnEvent(MessageUpdated, new MessageUpdatedEventArgs(before, after));

        private void OnProfileUpdated(Profile before, Profile after)
            => OnEvent(ProfileUpdated, new ProfileUpdatedEventArgs(before, after));

        private void OnRoleCreated(Role role)
            => OnEvent(RoleCreated, new RoleEventArgs(role));
        private void OnRoleDeleted(Role role)
            => OnEvent(RoleDeleted, new RoleEventArgs(role));
        private void OnRoleUpdated(Role before, Role after)
            => OnEvent(RoleUpdated, new RoleUpdatedEventArgs(before, after));

        private void OnJoinedServer(Server server)
            => OnEvent(JoinedServer, new ServerEventArgs(server));
        private void OnLeftServer(Server server)
            => OnEvent(LeftServer, new ServerEventArgs(server));
        private void OnServerAvailable(Server server)
            => OnEvent(ServerAvailable, new ServerEventArgs(server));
        private void OnServerUpdated(Server before, Server after)
            => OnEvent(ServerUpdated, new ServerUpdatedEventArgs(before, after));
        private void OnServerUnavailable(Server server)
            => OnEvent(ServerUnavailable, new ServerEventArgs(server));

        private void OnUserBanned(User user)
            => OnEvent(UserBanned, new UserEventArgs(user));
        private void OnUserIsTypingUpdated(Channel channel, User user)
            => OnEvent(UserIsTyping, new ChannelUserEventArgs(channel, user));
        private void OnUserJoined(User user)
            => OnEvent(UserJoined, new UserEventArgs(user));
        private void OnUserLeft(User user)
            => OnEvent(UserLeft, new UserEventArgs(user));
        private void OnUserUnbanned(User user)
            => OnEvent(UserUnbanned, new UserEventArgs(user));
        private void OnUserUpdated(User before, User after)
            => OnEvent(UserUpdated, new UserUpdatedEventArgs(before, after));

        private void OnEvent<T>(EventHandler<T> handler, T eventArgs, [CallerMemberName] string callerName = null)
        {
            try { handler(this, eventArgs); }
            catch (Exception ex)
            {
                Logger.Error($"{callerName.Substring(2)}'s handler encountered an error", ex);
            }
        }
        private void OnEvent(EventHandler handler, [CallerMemberName] string callerName = null)
        {
            try { handler(this, EventArgs.Empty); }
            catch (Exception ex)
            {
                Logger.Error($"{callerName.Substring(2)}'s handler encountered an error", ex);
            }
        }
    }
}
