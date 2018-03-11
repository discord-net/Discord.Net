using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    public abstract class SocketUser : SocketEntity<ulong>, IUser
    {
        public abstract bool IsBot { get; internal set; }
        public abstract string Username { get; internal set; }
        public abstract ushort DiscriminatorValue { get; internal set; }
        public abstract string AvatarId { get; internal set; }
        public abstract bool IsWebhook { get; }
        internal abstract SocketGlobalUser GlobalUser { get; }
        internal abstract SocketPresence Presence { get; set; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public string Mention => MentionUtils.MentionUser(Id);
        public IActivity Activity => Presence.Activity;
        public UserStatus Status => Presence.Status;

        internal SocketUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal virtual bool Update(ClientState state, Model model)
        {
            bool hasChanges = false;
            if (model.Avatar.IsSpecified && model.Avatar.Value != AvatarId)
            {
                AvatarId = model.Avatar.Value;
                hasChanges = true;
            }
            if (model.Discriminator.IsSpecified)
            {
                var newVal = ushort.Parse(model.Discriminator.Value);
                if (newVal != DiscriminatorValue)
                {
                    DiscriminatorValue = ushort.Parse(model.Discriminator.Value);
                    hasChanges = true;
                }
            }
            if (model.Bot.IsSpecified && model.Bot.Value != IsBot)
            {
                IsBot = model.Bot.Value;
                hasChanges = true;
            }
            if (model.Username.IsSpecified && model.Username.Value != Username)
            {
                Username = model.Username.Value;
                hasChanges = true;
            }
            return hasChanges;
        }

        public async Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
            => GlobalUser.DMChannel ?? await UserHelper.CreateDMChannelAsync(this, Discord, options) as IDMChannel;

        public string GetCustomAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public string GetDefaultAvatarUrl()
        {
            switch (DiscriminatorValue % 5)
            {
                case 0: return DiscordConfig.MainUrl + "assets/6debd47ed13483642cf09e832ed0bc1b.png";
                case 1: return DiscordConfig.MainUrl + "assets/322c936a8c8be1b803cd94861bdfa868.png";
                case 2: return DiscordConfig.MainUrl + "assets/dd4dbc0016779df1378e7812eabaa04d.png";
                case 3: return DiscordConfig.MainUrl + "assets/0e291f67c9274a1abdddeb3fd919cbaa.png";
                case 4: return DiscordConfig.MainUrl + "assets/1cbd08c76f8af6dddce02c5138971129.png";
            }
        }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return GetCustomAvatarUrl(format, size) ?? GetDefaultAvatarUrl();
        }

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;
    }
}
