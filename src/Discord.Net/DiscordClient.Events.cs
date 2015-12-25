using System;
using System.Runtime.CompilerServices;

namespace Discord
{
    public partial class DiscordClient
    {
        public event EventHandler Connected = delegate { };
        public event EventHandler<DisconnectedEventArgs> Disconnected = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelCreated = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelDestroyed = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelUpdated = delegate { };
        public event EventHandler<MessageEventArgs> MessageAcknowledged = delegate { };
        public event EventHandler<MessageEventArgs> MessageDeleted = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };
        public event EventHandler<MessageEventArgs> MessageUpdated = delegate { };
        public event EventHandler<ProfileEventArgs> ProfileUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleCreated = delegate { };
        public event EventHandler<RoleEventArgs> RoleUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleDeleted = delegate { };
        public event EventHandler<ServerEventArgs> JoinedServer = delegate { };
        public event EventHandler<ServerEventArgs> LeftServer = delegate { };
        public event EventHandler<ServerEventArgs> ServerAvailable = delegate { };
        public event EventHandler<ServerEventArgs> ServerUpdated = delegate { };
        public event EventHandler<ServerEventArgs> ServerUnavailable = delegate { };
        public event EventHandler<BanEventArgs> UserBanned = delegate { };
        public event EventHandler<ChannelUserEventArgs> UserIsTypingUpdated = delegate { };
        public event EventHandler<UserEventArgs> UserJoined = delegate { };
        public event EventHandler<UserEventArgs> UserLeft = delegate { };
        public event EventHandler<UserEventArgs> UserPresenceUpdated = delegate { };
        public event EventHandler<UserEventArgs> UserUpdated = delegate { };
        public event EventHandler<BanEventArgs> UserUnbanned = delegate { };
        public event EventHandler<UserEventArgs> UserVoiceStateUpdated = delegate { };

        private void OnConnected()
            => OnEvent(Connected);
        private void OnDisconnected(bool wasUnexpected, Exception ex)
            => OnEvent(Disconnected, new DisconnectedEventArgs(wasUnexpected, ex));

        private void OnChannelCreated(Channel channel)
            => OnEvent(ChannelCreated, new ChannelEventArgs(channel));
        private void OnChannelDestroyed(Channel channel)
            => OnEvent(ChannelDestroyed, new ChannelEventArgs(channel));
        private void OnChannelUpdated(Channel channel)
            => OnEvent(ChannelUpdated, new ChannelEventArgs(channel));
        
        private void OnMessageAcknowledged(Message msg)
            => OnEvent(MessageAcknowledged, new MessageEventArgs(msg));
        private void OnMessageDeleted(Message msg)
            => OnEvent(MessageDeleted, new MessageEventArgs(msg));
        private void OnMessageReceived(Message msg)
            => OnEvent(MessageReceived, new MessageEventArgs(msg));
        /*private void OnMessageSent(Message msg)
            => OnEvent(MessageSent, new MessageEventArgs(msg));*/
        private void OnMessageUpdated(Message msg)
            => OnEvent(MessageUpdated, new MessageEventArgs(msg));

        private void OnProfileUpdated(Profile profile)
            => OnEvent(ProfileUpdated, new ProfileEventArgs(profile));

        private void OnRoleCreated(Role role)
            => OnEvent(RoleCreated, new RoleEventArgs(role));
        private void OnRoleDeleted(Role role)
            => OnEvent(RoleDeleted, new RoleEventArgs(role));
        private void OnRoleUpdated(Role role)
            => OnEvent(RoleUpdated, new RoleEventArgs(role));

        private void OnJoinedServer(Server server)
            => OnEvent(JoinedServer, new ServerEventArgs(server));
        private void OnLeftServer(Server server)
            => OnEvent(LeftServer, new ServerEventArgs(server));
        private void OnServerAvailable(Server server)
            => OnEvent(ServerAvailable, new ServerEventArgs(server));
        private void OnServerUpdated(Server server)
            => OnEvent(ServerUpdated, new ServerEventArgs(server));
        private void OnServerUnavailable(Server server)
            => OnEvent(ServerUnavailable, new ServerEventArgs(server));

        private void OnUserBanned(Server server, ulong userId)
            => OnEvent(UserBanned, new BanEventArgs(server, userId));
        private void OnUserIsTypingUpdated(Channel channel, User user)
            => OnEvent(UserIsTypingUpdated, new ChannelUserEventArgs(channel, user));
        private void OnUserJoined(User user)
            => OnEvent(UserJoined, new UserEventArgs(user));
        private void OnUserLeft(User user)
            => OnEvent(UserLeft, new UserEventArgs(user));
        private void OnUserPresenceUpdated(User user)
            => OnEvent(UserPresenceUpdated, new UserEventArgs(user));
        private void OnUserUnbanned(Server server, ulong userId)
            => OnEvent(UserUnbanned, new BanEventArgs(server, userId));
        private void OnUserUpdated(User user)
            => OnEvent(UserUpdated, new UserEventArgs(user));
        private void OnUserVoiceStateUpdated(User user)
            => OnEvent(UserVoiceStateUpdated, new UserEventArgs(user));

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
