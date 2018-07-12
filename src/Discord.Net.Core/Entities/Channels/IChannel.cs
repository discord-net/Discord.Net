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
        ///     Gets a collection of all users in this channel.
        /// </summary>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A paged collection containing a collection of users that can access this channel. Flattening the
        ///     paginated response into a collection of users with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
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
