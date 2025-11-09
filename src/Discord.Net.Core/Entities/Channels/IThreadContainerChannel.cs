using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a channel that can contain and query threads.
    /// </summary>
    public interface IThreadContainerChannel
    {
        /// <summary>
        ///     Gets a collection of active threads within this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of active threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of publicly archived threads within this channel.
        /// </summary>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of publicly archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetPublicArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of privately archived threads within this channel.
        /// </summary>
        /// <remarks>
        ///     The bot requires the <see cref="GuildPermission.ManageThreads"/> permission in order to execute this request.
        /// </remarks>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of privately archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of privately archived threads that the current bot has joined within this channel.
        /// </summary>
        /// <param name="limit">The optional limit of how many to get.</param>
        /// <param name="before">The optional date to return threads created before this timestamp.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of privately archived threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(int? limit = null, DateTimeOffset? before = null, RequestOptions options = null);
    }
}
