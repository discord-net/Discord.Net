using Discord.API.Rest;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class User : SnowflakeEntity, IUser
    {
        private string _avatarId;
        
        public string Discriminator { get; private set; }
        public bool IsBot { get; private set; }
        public string Username { get; private set; }

        public override DiscordClient Discord { get; }

        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, _avatarId);
        public string Mention => MentionUtils.Mention(this, false);
        public string NicknameMention => MentionUtils.Mention(this, true);
        public virtual Game? Game => null;
        public virtual UserStatus Status => UserStatus.Unknown;

        public User(DiscordClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Update(model, UpdateSource.Creation);
        }
        public virtual void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            _avatarId = model.Avatar;
            Discriminator = model.Discriminator;
            IsBot = model.Bot;
            Username = model.Username;
        }

        public async Task<IDMChannel> CreateDMChannelAsync()
        {
            var args = new CreateDMChannelParams { RecipientId = Id };
            var model = await Discord.ApiClient.CreateDMChannelAsync(args).ConfigureAwait(false);

            return new DMChannel(Discord, this, model);
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id})";
    }
}
