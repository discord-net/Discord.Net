using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic guild scheduled event.
    /// </summary>
    public interface IGuildScheduledEvent : IEntity<ulong>
    {
        /// <summary>
        ///     Gets the guild this event is scheduled in.
        /// </summary>
        IGuild Guild { get; }

        /// <summary>
        ///     Gets the optional channel id where this event will be hosted.
        /// </summary>
        ulong? ChannelId { get; }

        /// <summary>
        ///     Gets the user who created the event.
        /// </summary>
        IUser Creator { get; }

        /// <summary>
        ///     Gets the name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the description of the event.
        /// </summary>
        /// <remarks>
        ///     This field is <see langword="null"/> when the event doesn't have a discription.
        /// </remarks>
        string Description { get; }

        /// <summary>
        ///     Gets the banner asset id of the event.
        /// </summary>
        string CoverImageId { get; }

        /// <summary>
        ///     Gets the start time of the event.
        /// </summary>
        DateTimeOffset StartTime { get; }

        /// <summary>
        ///     Gets the optional end time of the event.
        /// </summary>
        DateTimeOffset? EndTime { get; }

        /// <summary>
        ///     Gets the privacy level of the event.
        /// </summary>
        GuildScheduledEventPrivacyLevel PrivacyLevel { get; }

        /// <summary>
        ///     Gets the status of the event.
        /// </summary>
        GuildScheduledEventStatus Status { get; }

        /// <summary>
        ///     Gets the type of the event.
        /// </summary>
        GuildScheduledEventType Type { get; }

        /// <summary>
        ///     Gets the optional entity id of the event. The "entity" of the event
        ///     can be a stage instance event as is seperate from <see cref="ChannelId"/>.
        /// </summary>
        ulong? EntityId { get; }

        /// <summary>
        ///     Gets the location of the event if the <see cref="Type"/> is external.
        /// </summary>
        string Location { get; }

        /// <summary>
        ///     Gets the user count of the event.
        /// </summary>
        int? UserCount { get; }

        /// <summary>
        ///     Gets this events banner image url.
        /// </summary>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>The cover images url.</returns>
        string GetCoverImageUrl(ImageFormat format = ImageFormat.Auto, ushort size = 1024);

        /// <summary>
        ///     Starts the event.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous start operation.
        /// </returns>
        Task StartAsync(RequestOptions options = null);
        /// <summary>
        ///     Ends or canceles the event.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous end operation.
        /// </returns>
        Task EndAsync(RequestOptions options = null);

        /// <summary>
        ///     Modifies the guild event.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the event with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<GuildScheduledEventsProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Deletes the current event.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous delete operation.
        /// </returns>
        Task DeleteAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of N users interested in the event.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     This method will attempt to fetch all users that are interested in the event.
        ///     The library will attempt to split up the requests according to and <see cref="DiscordConfig.MaxGuildEventUsersPerBatch"/>.
        ///     In other words, if there are 300 users, and the <see cref="Discord.DiscordConfig.MaxGuildEventUsersPerBatch"/> constant
        ///     is <c>100</c>, the request will be split into 3 individual requests; thus returning 3 individual asynchronous
        ///     responses, hence the need of flattening.
        /// </remarks>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of users.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of N users interested in the event.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual users as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many users at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of users specified under <paramref name="limit"/> around
        ///     the user <paramref name="fromUserId"/> depending on the <paramref name="dir"/>. The library will
        ///     attempt to split up the requests according to your <paramref name="limit"/> and 
        ///     <see cref="DiscordConfig.MaxGuildEventUsersPerBatch"/>. In other words, should the user request 500 users,
        ///     and the <see cref="Discord.DiscordConfig.MaxGuildEventUsersPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <param name="fromUserId">The ID of the starting user to get the users from.</param>
        /// <param name="dir">The direction of the users to be gotten from.</param>
        /// <param name="limit">The numbers of users to be gotten from.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of users.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(ulong fromUserId, Direction dir, int limit = DiscordConfig.MaxGuildEventUsersPerBatch, RequestOptions options = null);
    }
}
