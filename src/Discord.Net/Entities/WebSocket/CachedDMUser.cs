using System;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedDMUser : ICachedUser
    {
        public CachedGlobalUser User { get; }

        public Game Game { get; private set; }
        public UserStatus Status { get; private set; }

        public DiscordSocketClient Discord => User.Discord;
        
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
            Status = model.Status;
            Game = model.Game != null ? new Game(model.Game) : null;
        }

        public CachedDMUser Clone() => MemberwiseClone() as CachedDMUser;
        ICachedUser ICachedUser.Clone() => Clone();
    }
}
