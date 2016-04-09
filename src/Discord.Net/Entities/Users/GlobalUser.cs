using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    public class GlobalUser : IUser, IEntity<ulong>
    {
        private string _avatarId;
        private int _refCount;

        /// <inheritdoc />
        public ulong Id { get; }
        /// <inheritdoc />
        public DiscordClient Discord { get; }

        public string Username { get; private set; }
        public ushort Discriminator { get; private set; }
        public bool IsBot { get; private set; }
        public string CurrentGame { get; private set; }
        public UserStatus Status { get; private set; }

        public string AvatarUrl => CDN.GetUserAvatarUrl(Id, _avatarId);
        public string Mention => MentionHelper.Mention(this);

        internal GlobalUser(ulong id, DiscordClient discord)
        {
            Id = id;
            Discord = discord;
        }
        internal virtual void Update(Model model)
        {
            Username = model.Username;
            Discriminator = model.Discriminator;
            IsBot = model.Bot;
            _avatarId = model.Avatar;
        }

        public virtual Task Update() { throw new NotSupportedException(); }

        public async Task<DMChannel> CreateDMChannel() => await Discord.GetOrCreateDMChannel(Id);

        internal void Attach(IUser user)
        {
            //Only ever called from the gateway thread
            _refCount++;
        }
        internal void Detach(IUser user)
        {
            //Only ever called from the gateway thread
            if (--_refCount == 0)
                Discord.RemoveUser(this);
        }
    }
}
