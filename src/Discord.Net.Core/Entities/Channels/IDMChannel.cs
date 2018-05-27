using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic direct-message channel.
    /// </summary>
    public interface IDMChannel : IMessageChannel, IPrivateChannel
    {
        /// <summary>
        ///     Gets the recipient of all messages in this channel.
        /// </summary>
        IUser Recipient { get; }

        /// <summary>
        ///     Closes this private channel, removing it from your channel list.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        Task CloseAsync(RequestOptions options = null);
    }
}
