using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a webhook deletion.
    /// </summary>
    public class WebhookDeleteAuditLogData : IAuditLogData
    {
        private WebhookDeleteAuditLogData(ulong id, ulong channel, WebhookType type, string name, string avatar)
        {
            WebhookId = id;
            ChannelId = channel;
            Name = name;
            Type = type;
            Avatar = avatar;
        }

        internal static WebhookDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var avatarHashModel = changes.FirstOrDefault(x => x.ChangedProperty == "avatar_hash");

            var channelId = channelIdModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var type = typeModel.OldValue.ToObject<WebhookType>(discord.ApiClient.Serializer);
            var name = nameModel.OldValue.ToObject<string>(discord.ApiClient.Serializer);
            var avatarHash = avatarHashModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer);

            return new WebhookDeleteAuditLogData(entry.TargetId.Value, channelId, type, name, avatarHash);
        }

        /// <summary>
        ///     Gets the ID of the webhook that was deleted.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the webhook that was deleted.
        /// </returns>
        public ulong WebhookId { get; }
        /// <summary>
        ///     Gets the ID of the channel that the webhook could send to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the channel that the webhook could send
        ///     to.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the type of the webhook that was deleted.
        /// </summary>
        /// <returns>
        ///     The type of webhook that was deleted.
        /// </returns>
        public WebhookType Type { get; }
        /// <summary>
        ///     Gets the name of the webhook that was deleted.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the webhook that was deleted.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the hash value of the webhook's avatar.
        /// </summary>
        /// <returns>
        ///     A string containing the hash of the webhook's avatar.
        /// </returns>
        public string Avatar { get; }
    }
}
