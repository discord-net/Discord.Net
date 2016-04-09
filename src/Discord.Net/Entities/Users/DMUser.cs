using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    public class DMUser : IUser
    {
        private readonly GlobalUser _user;

        public DMChannel Channel { get; }
        
        /// <inheritdoc />
        public DiscordClient Discord => _user.Discord;
        /// <inheritdoc />
        public ulong Id => _user.Id;
        /// <inheritdoc />
        public string Username => _user.Username;
        /// <inheritdoc />
        public ushort Discriminator => _user.Discriminator;
        /// <inheritdoc />
        public bool IsBot => _user.IsBot;
        /// <inheritdoc />
        public string CurrentGame => _user.CurrentGame;
        /// <inheritdoc />
        public UserStatus Status => _user.Status;
        /// <inheritdoc />
        public string AvatarUrl => _user.AvatarUrl;
        /// <inheritdoc />
        public string Mention => _user.Mention;

        internal DMUser(GlobalUser user, DMChannel channel)
        {
            _user = user;
            Channel = channel;
        }

        public void Update(Model model) => _user.Update(model);

        public virtual Task Update() { throw new NotSupportedException(); }
    }
}
