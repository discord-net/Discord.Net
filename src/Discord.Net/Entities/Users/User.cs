using System;
using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class User : SnowflakeEntity, IUser
    {
        protected string _avatarId;

        public bool IsBot { get; private set; }
        public string Username { get; private set; }
        public ushort DiscriminatorValue { get; private set; }

        public override DiscordRestClient Discord { get { throw new NotSupportedException(); } }

        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, _avatarId);
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public string Mention => MentionUtils.Mention(this);
        public virtual Game Game => null;
        public virtual UserStatus Status => UserStatus.Unknown;

        public User(Model model)
            : base(model.Id)
        {
            Update(model, UpdateSource.Creation);
        }
        public virtual void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            if (model.Avatar.IsSpecified)
                _avatarId = model.Avatar.Value;
            if (model.Discriminator.IsSpecified)
                DiscriminatorValue = ushort.Parse(model.Discriminator.Value);
            if (model.Bot.IsSpecified)
                IsBot = model.Bot.Value;
            if (model.Username.IsSpecified)
                Username = model.Username.Value;
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
