using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Webhook;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class RestWebhook : RestEntity<ulong>, IWebhook, IUpdateable
    {
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

        internal IGuild Guild { get; }
        internal ITextChannel Channel { get; }
        private string DebuggerDisplay => $"Webhook: {Name} ({Id})";

        public async Task UpdateAsync(RequestOptions options = null)
        {
            var model = await Discord.ApiClient.GetWebhookAsync(Id, options).ConfigureAwait(false);
            Update(model);
        }

        public ulong ChannelId { get; }
        public string Token { get; }

        public string Name { get; private set; }
        public string AvatarId { get; private set; }
        public ulong? GuildId { get; private set; }
        public IUser Creator { get; private set; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public Task DeleteAsync(RequestOptions options = null)
            => WebhookHelper.DeleteAsync(this, Discord, options);

        //IWebhook
        IGuild IWebhook.Guild
            => Guild ?? throw new InvalidOperationException(
                   "Unable to return this entity's parent unless it was fetched through that object.");

        ITextChannel IWebhook.Channel
            => Channel ?? throw new InvalidOperationException(
                   "Unable to return this entity's parent unless it was fetched through that object.");

        Task IWebhook.ModifyAsync(Action<WebhookProperties> func, RequestOptions options)
            => ModifyAsync(func, options);

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

        public async Task ModifyAsync(Action<WebhookProperties> func, RequestOptions options = null)
        {
            var model = await WebhookHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        public override string ToString() => $"Webhook: {Name}:{Id}";
    }
}
