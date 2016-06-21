using System;
using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class User : SnowflakeEntity, IUser
    {
        private string _avatarId;
        private ushort _discriminator;

        public bool IsBot { get; private set; }
        public string Username { get; private set; }

        public override DiscordClient Discord { get { throw new NotSupportedException(); } }

        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, _avatarId);
        public string Discriminator => _discriminator.ToString("D4");
        public string Mention => MentionUtils.Mention(this, false);
        public string NicknameMention => MentionUtils.Mention(this, true);
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

            _avatarId = model.Avatar;
            _discriminator = ushort.Parse(model.Discriminator);
            IsBot = model.Bot;
            Username = model.Username;
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
