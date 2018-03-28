using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    /// <summary> The WebSocket variant of <see cref="IUser"/>. Represents a Discord user. </summary>
    public abstract class SocketUser : SocketEntity<ulong>, IUser
    {
        /// <inheritdoc />
        public abstract bool IsBot { get; internal set; }
        /// <inheritdoc />
        public abstract string Username { get; internal set; }
        /// <inheritdoc />
        public abstract ushort DiscriminatorValue { get; internal set; }
        /// <inheritdoc />
        public abstract string AvatarId { get; internal set; }
        /// <inheritdoc />
        public abstract bool IsWebhook { get; }
        internal abstract SocketGlobalUser GlobalUser { get; }
        internal abstract SocketPresence Presence { get; set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string Discriminator => DiscriminatorValue.ToString("D4");
        /// <inheritdoc />
        public string Mention => MentionUtils.MentionUser(Id);
        /// <inheritdoc />
        public IActivity Activity => Presence.Activity;
        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
            => GlobalUser.DMChannel ?? await UserHelper.CreateDMChannelAsync(this, Discord, options) as IDMChannel;

        /// <inheritdoc />
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        /// <summary> Gets the username and the discriminator. </summary>
        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;
    }
}
