using System;
using System.Runtime.CompilerServices;

namespace Discord
{
    public partial class DiscordClient
    {
        public event EventHandler Ready = delegate { };
        //public event EventHandler<DisconnectedEventArgs> LoggedOut = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelCreated = delegate { };
        public event EventHandler<ChannelEventArgs> ChannelDestroyed = delegate { };
        public event EventHandler<ChannelUpdatedEventArgs> ChannelUpdated = delegate { };
        public event EventHandler<MessageEventArgs> MessageAcknowledged = delegate { };
        public event EventHandler<MessageEventArgs> MessageDeleted = delegate { };
        public event EventHandler<MessageEventArgs> MessageReceived = delegate { };
        public event EventHandler<MessageEventArgs> MessageSent = delegate { };
        public event EventHandler<MessageUpdatedEventArgs> MessageUpdated = delegate { };
        public event EventHandler<ProfileUpdatedEventArgs> ProfileUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleCreated = delegate { };
        public event EventHandler<RoleUpdatedEventArgs> RoleUpdated = delegate { };
        public event EventHandler<RoleEventArgs> RoleDeleted = delegate { };
        public event EventHandler<ServerEventArgs> JoinedServer = delegate { };
        public event EventHandler<ServerEventArgs> LeftServer = delegate { };
        public event EventHandler<ServerEventArgs> ServerAvailable = delegate { };
        public event EventHandler<ServerUpdatedEventArgs> ServerUpdated = delegate { };
        public event EventHandler<ServerEventArgs> ServerUnavailable = delegate { };
        public event EventHandler<UserEventArgs> UserBanned = delegate { };
        public event EventHandler<ChannelUserEventArgs> UserIsTyping = delegate { };
        public event EventHandler<UserEventArgs> UserJoined = delegate { };
        public event EventHandler<UserEventArgs> UserLeft = delegate { };
        public event EventHandler<UserUpdatedEventArgs> UserUpdated = delegate { };
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
