using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    public abstract class User : IEntity<ulong>
    {
        private string _avatarId;

        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public abstract DiscordClient Discord { get; }

        public string Username { get; private set; }
        public ushort Discriminator { get; private set; }
        public bool IsBot { get; private set; }
        
        public string AvatarUrl => CDN.GetUserAvatarUrl(Id, _avatarId);
        public string Mention => MentionHelper.Mention(this);

        internal User(ulong id)
        {
            Id = id;
        }
        internal virtual void Update(Model model)
        {
            Username = model.Username;
            Discriminator = model.Discriminator;
            IsBot = model.Bot;
            _avatarId = model.Avatar;
        }

        public virtual Task Update() { throw new NotSupportedException(); }

        public async Task<DMChannel> CreateDMChannel() => await Discord.GetOrCreateDMChannel(Id); //TODO: We dont want both this and .Channel to appear on DMUser
    }
}
