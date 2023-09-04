namespace Discord;

/// <summary>
///     Represents a generic news channel in a guild that can send and receive messages.
/// </summary>
public interface INewsChannel : ITextChannel
{
    /// <summary>
    ///     Follow this channel to send messages to a target channel.
    /// </summary>
    /// <param name="channelId">The target channel ID to receive messages.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken"/> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation of following the channel.
    /// </returns>
    Task<ulong> FollowAnnouncementChannelAsync(ulong channelId, RequestOptions? options = null, CancellationToken token = default);
}
