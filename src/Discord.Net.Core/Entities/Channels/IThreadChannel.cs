using System;
using System.Collections.Generic;
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
        ///     Gets whether non-moderators can add other non-moderators to a thread.
        /// </summary>
        /// <remarks>
        ///     This property is only available on private threads.
        /// </remarks>
        bool? IsInvitable { get; }

        /// <summary>
        ///     Gets ids of tags applied to a forum thread
        /// </summary>
        /// <remarks>
        ///     This property is only available on forum threads.
        /// </remarks>
        IReadOnlyCollection<ulong> AppliedTags { get; }

        /// <summary>
        ///     Gets when the thread was created.
        /// </summary>
        /// <remarks>
        ///     This property is only populated for threads created after 2022-01-09, hence the default date of this
        ///     property will be that date.
        /// </remarks>
        new DateTimeOffset CreatedAt { get; }

        /// <summary>
        ///     Gets the id of the creator of the thread.
        /// </summary>
        ulong OwnerId { get; }

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

        /// <summary>
        ///     Modifies this thread channel.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <seealso cref="ThreadChannelProperties"/>
        Task ModifyAsync(Action<ThreadChannelProperties> func, RequestOptions options = null);
    }
}
