using Discord.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.User;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcUser : RpcEntity<ulong>, IUser
    {
        public bool IsBot { get; private set; }
        public string Username { get; private set; }
        public ushort DiscriminatorValue { get; private set; }
        public string AvatarId { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public string Mention => MentionUtils.MentionUser(Id);
        public virtual bool IsWebhook => false;
        public virtual Game? Game => null;
        public virtual UserStatus Status => UserStatus.Offline;

        internal RpcUser(DiscordRpcClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RpcUser Create(DiscordRpcClient discord, Model model)
            => Create(discord, model, null);
        internal static RpcUser Create(DiscordRpcClient discord, Model model, ulong? webhookId)
        {
            RpcUser entity;
            if (webhookId.HasValue)
                entity = new RpcWebhookUser(discord, model.Id, webhookId.Value);
            else
                entity = new RpcUser(discord, model.Id);
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

        public Task<RestDMChannel> CreateDMChannelAsync(RequestOptions options = null)
            => UserHelper.CreateDMChannelAsync(this, Discord, options);

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public override string ToString() => $"{Username}#{Discriminator}";
        private string DebuggerDisplay => $"{Username}#{Discriminator} ({Id}{(IsBot ? ", Bot" : "")})";

        //IUser
        Task<IDMChannel> IUser.GetDMChannelAsync(CacheMode mode, RequestOptions options)
            => Task.FromResult<IDMChannel>(null);
        async Task<IDMChannel> IUser.CreateDMChannelAsync(RequestOptions options)
            => await CreateDMChannelAsync(options).ConfigureAwait(false);
    }
}
