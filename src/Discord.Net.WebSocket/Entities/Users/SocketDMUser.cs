using System;
using System.Diagnostics;
using PresenceModel = Discord.API.Presence;

namespace Discord.WebSocket
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class SocketDMUser : ISocketUser
    {
        internal bool IsAttached => true;
        bool IEntity<ulong>.IsAttached => IsAttached;

        public SocketGlobalUser User { get; }

        public DiscordSocketClient Discord => User.Discord;

        public Game Game => Presence.Game;
        public UserStatus Status => Presence.Status;
        public Presence Presence => User.Presence; //{ get; private set; }

        public ulong Id => User.Id;
        public string AvatarUrl => User.AvatarUrl;
        public DateTimeOffset CreatedAt => User.CreatedAt;
        public string Discriminator => User.Discriminator;
        public ushort DiscriminatorValue => User.DiscriminatorValue;
        public bool IsBot => User.IsBot;
        public string Mention => MentionUtils.Mention(this);
        public string Username => User.Username;

        public SocketDMUser(SocketGlobalUser user)
        {
            User = user;
        }

        public void Update(PresenceModel model)
        {
            User.Update(model, source);
        }

        public SocketDMUser Clone() => MemberwiseClone() as SocketDMUser;
        ISocketUser ISocketUser.Clone() => Clone();

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
