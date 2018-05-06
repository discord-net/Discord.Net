using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic user.
    /// </summary>
    public interface IUser : ISnowflakeEntity, IMentionable, IPresence
    {
        /// <summary>
        ///     Gets the ID of this user's avatar.
        /// </summary>
        string AvatarId { get; }
        /// <summary>
        ///     Returns a URL to this user's avatar.
        /// </summary>
        /// <param name="format">The format to return.</param>
        /// <param name="size">
        /// The size of the image to return in. This can be any power of two between 16 and 2048.
        /// </param>
        /// <returns>
        ///     User's avatar URL.
        /// </returns>
        string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128);
        /// <summary>
        ///     Returns the URL to this user's default avatar.
        /// </summary>
        string GetDefaultAvatarUrl();
        /// <summary>
        ///     Gets the per-username unique ID for this user.
        /// </summary>
        string Discriminator { get; }
        /// <summary>
        ///     Gets the per-username unique ID for this user.
        /// </summary>
        ushort DiscriminatorValue { get; }
        /// <summary>
        ///     Gets <c>true</c> if this user is a bot user.
        /// </summary>
        bool IsBot { get; }
        /// <summary>
        ///     Gets <c>true</c> if this user is a webhook user.
        /// </summary>
        bool IsWebhook { get; }
        /// <summary>
        ///     Gets the username for this user.
        /// </summary>
        string Username { get; }

        /// <summary>
        ///     Returns a direct message channel to this user, or create one if it does not already exist.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable Task containing the DM channel.
        /// </returns>
        Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null);
    }
}
