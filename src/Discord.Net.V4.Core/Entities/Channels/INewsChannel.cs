using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic news channel in a guild that can send and receive messages.
    /// </summary>
    public interface INewsChannel : ITextChannel
    {
        /// <summary>
        ///     Follow this channel to send messages to a target channel.
        /// </summary>
        /// <returns>
        ///     The Id of the created webhook.
        /// </returns>
        Task<ulong> FollowAnnouncementChannelAsync(ulong channelId, RequestOptions options);
    }
}
