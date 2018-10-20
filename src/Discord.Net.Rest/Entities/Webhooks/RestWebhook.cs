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

        /// <inheritdoc />
        public ulong ChannelId { get; }
        /// <inheritdoc />
        public string Token { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string AvatarId { get; private set; }
        /// <inheritdoc />
        public ulong? GuildId { get; private set; }
        /// <inheritdoc />
        public IUser Creator { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestWebhook(BaseDiscordClient discord, IGuild guild, ulong id, string token, ulong channelId)
            : base(discord, id)
        {
            Guild = guild;
            Token = token;
            ChannelId = channelId;
        }
        internal RestWebhook(BaseDiscordClient discord, ITextChannel channel, ulong id, string token, ulong channelId)
            : this(discord, channel.Guild, id, token, channelId)
        {
            Channel = channel;
        }

        internal static RestWebhook Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestWebhook(discord, guild, model.Id, model.Token, model.ChannelId);
            entity.Update(model);
            return entity;
        }
        internal static RestWebhook Create(BaseDiscordClient discord, ITextChannel channel, Model model)
        {
            var entity = new RestWebhook(discord, channel, model.Id, model.Token, model.ChannelId);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Creator.IsSpecified)
                Creator = RestUser.Create(Discord, model.Creator.Value);
            if (model.GuildId.IsSpecified)
                GuildId = model.GuildId.Value;
            if (model.Name.IsSpecified)
                Name = model.Name.Value;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetWebhookAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc />
        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
           => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public async Task ModifyAsync(Action<WebhookProperties> func, RequestOptions options = null)
        {
            var model = await WebhookHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => WebhookHelper.DeleteAsync(this, Discord, options);

        public override string ToString() => $"Webhook: {Name}:{Id}";
        private string DebuggerDisplay => $"Webhook: {Name} ({Id})";

        //IWebhook
        /// <inheritdoc />
        IGuild IWebhook.Guild 
            => Guild ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        /// <inheritdoc />
        ITextChannel IWebhook.Channel 
            => Channel ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        /// <inheritdoc />
        Task IWebhook.ModifyAsync(Action<WebhookProperties> func, RequestOptions options)
            => ModifyAsync(func, options);
    }
}
