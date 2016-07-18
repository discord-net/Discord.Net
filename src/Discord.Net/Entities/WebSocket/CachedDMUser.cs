using System;
using System.Diagnostics;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class CachedDMUser : ICachedUser
    {
        public CachedGlobalUser User { get; }

        public DiscordSocketClient Discord => User.Discord;

        public Game Game => Presence.Game;
        public UserStatus Status => Presence.Status;
        public Presence Presence => User.Presence; //{ get; private set; }

        public ulong Id => User.Id;
        public string AvatarUrl => User.AvatarUrl;
        public DateTimeOffset CreatedAt => User.CreatedAt;
        public string Discriminator => User.Discriminator;
        public ushort DiscriminatorValue => User.DiscriminatorValue;
        public bool IsAttached => User.IsAttached;
        public bool IsBot => User.IsBot;
        public string Mention => User.Mention;
        public string NicknameMention => User.NicknameMention;
        public string Username => User.Username;

        public CachedDMUser(CachedGlobalUser user)
        {
            User = user;
        }

        public void Update(PresenceModel model, UpdateSource source)
        {
            User.Update(model, source);
        }

        public CachedDMUser Clone() => MemberwiseClone() as CachedDMUser;
        ICachedUser ICachedUser.Clone() => Clone();

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
