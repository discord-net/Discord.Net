using Discord.Rest;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    public class SocketUser : SocketEntity<ulong>, IUser
    {
        public bool IsBot { get; private set; }
        public string Username { get; private set; }
        public ushort DiscriminatorValue { get; private set; }
        public string AvatarId { get; private set; }

        public string AvatarUrl => API.CDN.GetUserAvatarUrl(Id, AvatarId);
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public string Mention => MentionUtils.MentionUser(Id);
        public virtual Game? Game => null;
        public virtual UserStatus Status => UserStatus.Unknown;

        internal SocketUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static SocketUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }
        internal virtual void Update(Model model)
        {
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Discriminator.IsSpecified)
                DiscriminatorValue = ushort.Parse(model.Discriminator.Value);
            if (model.Bot.IsSpecified)
                IsBot = model.Bot.Value;
            if (model.Username.IsSpecified)
                Username = model.Username.Value;
        }

        public virtual async Task UpdateAsync()
            => Update(await UserHelper.GetAsync(this, Discord));

        public Task<IDMChannel> CreateDMChannelAsync()
            => UserHelper.CreateDMChannelAsync(this, Discord);

        IDMChannel IUser.GetCachedDMChannel() => null;
    }
}
