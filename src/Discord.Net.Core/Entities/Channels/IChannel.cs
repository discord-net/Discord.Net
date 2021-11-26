using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel.
    /// </summary>
    public interface IChannel : ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of this channel.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this channel.
        /// </returns>
        string Name { get; }

        /// <summary>
        ///     Gets a collection of users that are able to view the channel or are currently in this channel.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the individual messages as a
        ///         collection.
        ///     </note>
        ///     This method will attempt to fetch all users that is able to view this channel or is currently in this channel.
        ///     The library will attempt to split up the requests according to and <see cref="DiscordConfig.MaxUsersPerBatch"/>.
        ///     In other words, if there are 3000 users, and the <see cref="Discord.DiscordConfig.MaxUsersPerBatch"/> constant
        ///     is <c>1000</c>, the request will be split into 3 individual requests; thus returning 53individual asynchronous
        ///     responses, hence the need of flattening.
        /// </remarks>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of users.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetUsersAsync(CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets a user in this channel.
        /// </summary>
        /// <param name="id">The snowflake identifier of the user (e.g. <c>168693960628371456</c>).</param>
        /// <param name="mode">The <see cref="CacheMode" /> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a user object that
        ///     represents the found user; <c>null</c> if none is found.
        /// </returns>
        Task<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
    }
}
