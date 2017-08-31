using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Webhook;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestWebhook : RestEntity<ulong>, IWebhook, IUpdateable
    {
        internal IGuild Guild { get; private set; }
        internal ITextChannel Channel { get; private set; }
        public string Token { get; private set; }
        public string Name { get; private set; }
        public string AvatarId { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong GuildId { get; private set; }
        public IUser Creator { get; private set; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestWebhook(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal RestWebhook(BaseDiscordClient discord, ITextChannel channel, ulong id)
            : this(discord, channel.Guild, id)
        {
            Channel = channel;
        }
        internal static RestWebhook Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestWebhook(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }
        internal static RestWebhook Create(BaseDiscordClient discord, ITextChannel channel, Model model)
        {
            var entity = new RestWebhook(discord, channel, model.Id);
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            Token = model.Token;
            ChannelId = model.ChannelId;
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Creator.IsSpecified)
                Creator = RestUser.Create(Discord, model.Creator.Value);
            if (model.GuildId.IsSpecified)
                GuildId = model.GuildId.Value;
            if (model.Name.IsSpecified)
                Name = model.Name.Value;
        }

        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetWebhookAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
           => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public async Task ModifyAsync(Action<WebhookProperties> func, RequestOptions options = null)
        {
            var model = await WebhookHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        public Task DeleteAsync(RequestOptions options = null)
            => WebhookHelper.DeleteAsync(this, Discord, options);

        public async Task<RestTextChannel> GetChannelAsync(RequestOptions options = null)
        {
            Channel = await ClientHelper.GetChannelAsync(Discord, ChannelId, options) as RestTextChannel;
            Guild = Channel.Guild;

            return Channel as RestTextChannel;
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id})";

        //IWebhook
        IGuild IWebhook.Guild 
            => Guild ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        ITextChannel IWebhook.Channel 
            => Channel ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        Task IWebhook.ModifyAsync(Action<WebhookProperties> func, RequestOptions options)
            => ModifyAsync(func, options);
    }
}
