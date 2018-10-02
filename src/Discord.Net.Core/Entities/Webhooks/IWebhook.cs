using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a webhook object on Discord.
    /// </summary>
    public interface IWebhook : IDeletable, ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the token of this webhook.
        /// </summary>
        string Token { get; }

        /// <summary>
        ///     Gets the default name of this webhook.
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     Gets the ID of this webhook's default avatar.
        /// </summary>
        string AvatarId { get; }
        /// <summary>
        ///     Gets the URL to this webhook's default avatar.
        /// </summary>
        string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);

        /// <summary>
        ///     Gets the channel for this webhook.
        /// </summary>
        ITextChannel Channel { get; }
        /// <summary>
        ///     Gets the ID of the channel for this webhook.
        /// </summary>
        ulong ChannelId { get; }

        /// <summary>
        ///     Gets the guild owning this webhook.
        /// </summary>
        IGuild Guild { get; }
        /// <summary>
        ///     Gets the ID of the guild owning this webhook.
        /// </summary>
        ulong? GuildId { get; }

        /// <summary>
        ///     Gets the user that created this webhook.
        /// </summary>
        IUser Creator { get; }

        /// <summary>
        ///     Modifies this webhook.
        /// </summary>
        Task ModifyAsync(Action<WebhookProperties> func, RequestOptions options = null);
    }
}
