using System;

namespace Discord
{
    public partial class DiscordClient
    {
        public event EventHandler Ready = delegate { };
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
    }
}
