using Discord.Rest;
using System;
using System.Threading.Tasks;
using Model = Discord.API.User;
using PresenceModel = Discord.API.Presence;

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

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public string Mention => MentionUtils.MentionUser(Id);
        public Game? Game => Presence.Game;
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
        internal virtual void Update(ClientState state, PresenceModel model)
        {
            Presence = SocketPresence.Create(model);
            //Update(state, model.User);
        }

        public Task<RestDMChannel> CreateDMChannelAsync(RequestOptions options = null)
            => UserHelper.CreateDMChannelAsync(this, Discord, options);

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";
        internal SocketUser Clone() => MemberwiseClone() as SocketUser;

        //IUser
        Task<IDMChannel> IUser.GetDMChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IDMChannel>(GlobalUser.DMChannel);
        async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
            => await CreateDMChannelAsync(options).ConfigureAwait(false);
    }
}
