using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Webhook;

namespace Discord.Webhook
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class RestInternalWebhook : IWebhook
    {
        private DiscordWebhookClient _client;

        public ulong Id { get; }
        public string Token { get; }

        public ulong ChannelId { get; private set; }
        public string Name { get; private set; }
        public string AvatarId { get; private set; }
        public ulong? GuildId { get; private set; }
        public ulong? ApplicationId { get; private set; }

        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestInternalWebhook(DiscordWebhookClient apiClient, Model model)
        {
            _client = apiClient;
            Id = model.Id;
            ChannelId = model.Id;
            Token = model.Token;
        }
        internal static RestInternalWebhook Create(DiscordWebhookClient client, Model model)
        {
            var entity = new RestInternalWebhook(client, model);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            if (ChannelId != model.ChannelId)
                ChannelId = model.ChannelId;
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.GuildId.IsSpecified)
                GuildId = model.GuildId.Value;
            if (model.Name.IsSpecified)
                Name = model.Name.Value;

            ApplicationId = model.ApplicationId;
        }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
           => CDN.GetUserAvatarUrl(Id, AvatarId, size, format);

        public async Task ModifyAsync(Action<WebhookProperties> func, RequestOptions options = null)
        {
            var model = await WebhookClientHelper.ModifyAsync(_client, func, options).ConfigureAwait(false);
            Update(model);
        }

        public Task DeleteAsync(RequestOptions options = null)
            => WebhookClientHelper.DeleteAsync(_client, options);

        public override string ToString() => $"Webhook: {Name}:{Id}";
        private string DebuggerDisplay => $"Webhook: {Name} ({Id})";

        IUser IWebhook.Creator => null;
        IIntegrationChannel IWebhook.Channel => null;
        IGuild IWebhook.Guild => null;
    }
}
