using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Webhook;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestWebhook : RestEntity<ulong>, IWebhook, IUpdateable
    {
        #region RestWebhook

        internal IGuild Guild { get; private set; }
        internal IIntegrationChannel Channel { get; private set; }

        /// <inheritdoc />
        public string Token { get; }

        /// <inheritdoc />
        public ulong? ChannelId { get; private set; }

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public string AvatarId { get; private set; }

        /// <inheritdoc />
        public ulong? GuildId { get; private set; }

        /// <inheritdoc />
        public IUser Creator { get; private set; }

        /// <inheritdoc />
        public ulong? ApplicationId { get; private set; }

        /// <inheritdoc />
        public WebhookType Type { get; private set; }

        /// <summary>
        ///     Gets the partial guild of the followed channel. <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>.
        /// </summary>
        public PartialGuild PartialGuild { get; private set; }

        /// <summary>
        ///     Gets the id of the followed channel. <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>.
        /// </summary>
        public ulong? FollowedChannelId { get; private set; }

        /// <summary>
        ///     Gets the name of the followed channel. <see langword="null"/> if <see cref="Type"/> is not <see cref="WebhookType.ChannelFollower"/>.
        /// </summary>
        public string FollowedChannelName { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

        internal RestWebhook(BaseDiscordClient discord, IGuild guild, ulong id, string token, ulong? channelId, WebhookType type, PartialGuild partialGuild,
            ulong? followedChannelId, string followedChannelName)
            : base(discord, id)
        {
            Guild = guild;
            Token = token;
            ChannelId = channelId;
            Type = type;
            PartialGuild = partialGuild;
            FollowedChannelId = followedChannelId;
            FollowedChannelName = followedChannelName;
        }

        internal RestWebhook(BaseDiscordClient discord, IIntegrationChannel channel, ulong id, string token, ulong? channelId, WebhookType type, PartialGuild partialGuild,
            ulong? followedChannelId, string followedChannelName)
            : this(discord, channel.Guild, id, token, channelId, type, partialGuild, followedChannelId, followedChannelName)
        {
            Channel = channel;
        }

        internal static RestWebhook Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestWebhook(discord, guild, model.Id, model.Token.GetValueOrDefault(null), model.ChannelId, model.Type,
                model.Guild.IsSpecified ? PartialGuildExtensions.Create(model.Guild.Value) : null,
                model.Channel.IsSpecified ? model.Channel.Value.Id : null,
                model.Channel.IsSpecified ? model.Channel.Value.Name.GetValueOrDefault(null) : null
                );
            entity.Update(model);
            return entity;
        }

        internal static RestWebhook Create(BaseDiscordClient discord, IIntegrationChannel channel, Model model)
        {
            var entity = new RestWebhook(discord, channel, model.Id, model.Token.GetValueOrDefault(null), model.ChannelId, model.Type,
                model.Guild.IsSpecified ? PartialGuildExtensions.Create(model.Guild.Value) : null,
                model.Channel.IsSpecified ? model.Channel.Value.Id : null,
                model.Channel.IsSpecified ? model.Channel.Value.Name.GetValueOrDefault(null) : null);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            if (ChannelId != model.ChannelId)
                ChannelId = model.ChannelId;
            if (model.Avatar.IsSpecified)
                AvatarId = model.Avatar.Value;
            if (model.Creator.IsSpecified)
                Creator = RestUser.Create(Discord, model.Creator.Value);
            if (model.GuildId.IsSpecified)
                GuildId = model.GuildId.Value;
            if (model.Name.IsSpecified)
                Name = model.Name.Value;

            ApplicationId = model.ApplicationId;
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
        #endregion

        #region IWebhook
        /// <inheritdoc />
        IGuild IWebhook.Guild
            => Guild ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        /// <inheritdoc />
        IIntegrationChannel IWebhook.Channel
            => Channel ?? throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
        /// <inheritdoc />
        Task IWebhook.ModifyAsync(Action<WebhookProperties> func, RequestOptions options)
            => ModifyAsync(func, options);
        #endregion
    }
}
