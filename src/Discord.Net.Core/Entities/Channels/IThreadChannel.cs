using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a thread channel inside of a guild.
    /// </summary>
    public interface IThreadChannel : ITextChannel
    {
        /// <summary>
        ///     Gets the type of the current thread channel.
        /// </summary>
        ThreadType Type { get; }

        /// <summary>
        ///     Gets whether or not the current user has joined this thread.
        /// </summary>
        bool HasJoined { get; }

        /// <summary>
        ///     Gets whether or not the current thread is archived.
        /// </summary>
        bool IsArchived { get; }

        /// <summary>
        ///     Gets the duration of time before the thread is automatically archived after no activity.
        /// </summary>
        ThreadArchiveDuration AutoArchiveDuration { get; }

        /// <summary>
        ///     Gets the timestamp when the thread's archive status was last changed, used for calculating recent activity.
        /// </summary>
        DateTimeOffset ArchiveTimestamp { get; }

        /// <summary>
        ///     Gets whether or not the current thread is locked.
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        ///     Gets an approximate count of users in a thread, stops counting after 50.
        /// </summary>
        int MemberCount { get; }

        /// <summary>
        ///     Gets an approximate count of messages in a thread, stops counting after 50.
        /// </summary>
        int MessageCount { get; }

        /// <summary>
        ///     Joins the current thread.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous join operation.
        /// </returns>
        Task JoinAsync(RequestOptions options = null);

        /// <summary>
        ///     Leaves the current thread.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous leave operation.
        /// </returns>
        Task LeaveAsync(RequestOptions options = null);

        /// <summary>
        ///     Adds a user to this thread.
        /// </summary>
        /// <param name="user">The <see cref="IGuildUser"/> to add.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of adding a member to a thread.
        /// </returns>
        Task AddUserAsync(IGuildUser user, RequestOptions options = null);

        /// <summary>
        ///     Removes a user from this thread.
        /// </summary>
        /// <param name="user">The <see cref="IGuildUser"/> to remove from this thread.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation of removing a user from this thread.
        /// </returns>
        Task RemoveUserAsync(IGuildUser user, RequestOptions options = null);
    }
}
