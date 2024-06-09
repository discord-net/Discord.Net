namespace Discord;

/// <summary>
///     Represents a forum channel in a guild that can create posts.
/// </summary>
public interface IForumChannel : INestedChannel, IIntegrationChannel, IModifyable<ModifyForumChannelProperties>
{
    /// <summary>
    ///     Gets a value that indicates whether the channel is NSFW.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the channel has the NSFW flag enabled; otherwise <see langword="false" />.
    /// </returns>
    bool IsNsfw { get; }

    /// <summary>
    ///     Gets the current topic for this text channel.
    /// </summary>
    /// <returns>
    ///     A string representing the topic set in the channel; <see langword="null" /> if none is set.
    /// </returns>
    string? Topic { get; }

    /// <summary>
    ///     Gets the default archive duration for a newly created post.
    /// </summary>
    ThreadArchiveDuration DefaultAutoArchiveDuration { get; }

    /// <summary>
    ///     Gets a collection of tags inside of this forum channel.
    /// </summary>
    IReadOnlyCollection<IForumTag> Tags { get; }

    /// <summary>
    ///     Gets the current rate limit on creating posts in this forum channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <see langword="null" /> if disabled.
    /// </returns>
    int? ThreadCreationInterval { get; }

    /// <summary>
    ///     Gets the current default slow-mode delay for threads in this forum channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <see langword="null" /> if disabled.
    /// </returns>
    int? DefaultSlowModeInterval { get; }

    /// <summary>
    ///     Gets the emoji to show in the add reaction button on a thread in a forum channel
    /// </summary>
    ILoadableEntity<IEmote> DefaultReactionEmoji { get; }

    /// <summary>
    ///     Gets the rule used to order posts in forum channels.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see langword="null" />, which indicates a preferred sort order hasn't been set
    /// </remarks>
    ForumSortOrder? DefaultSortOrder { get; }

    /// <summary>
    ///     Gets the rule used to display posts in a forum channel.
    /// </summary>
    ForumLayout DefaultLayout { get; }

    /// <summary>
    ///     Creates a new post (thread) within the forum.
    /// </summary>
    /// <param name="title">The title of the post.</param>
    /// <param name="message">The message to be sent</param>
    /// <param name="archiveDuration">The archive duration of the post.</param>
    /// <param name="slowmode">The slowmode for the posts thread.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation.
    /// </returns>
    Task<IThreadChannel> CreatePostAsync(
        string title, CreateMessageProperties message,
        ThreadArchiveDuration archiveDuration = ThreadArchiveDuration.OneDay,
        int? slowmode = null, RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Gets a collection of active threads within this forum channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
    ///     a collection of active threads.
    /// </returns>
    Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(
        RequestOptions? options = null, CancellationToken token = default
    );

    /// <summary>
    ///     Gets a collection of publicly archived threads within this forum channel.
    /// </summary>
    /// <param name="limit">The optional limit of how many to get.</param>
    /// <param name="before">The optional date to return threads created before this timestamp.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
    ///     a collection of publicly archived threads.
    /// </returns>
    Task<IReadOnlyCollection<IThreadChannel>> GetPublicArchivedThreadsAsync(
        int? limit = null, DateTimeOffset? before = null,
        RequestOptions? options = null, CancellationToken token = default
    );

    /// <summary>
    ///     Gets a collection of privately archived threads within this forum channel.
    /// </summary>
    /// <remarks>
    ///     The bot requires the <see cref="GuildPermission.ManageThreads" /> permission in order to execute this request.
    /// </remarks>
    /// <param name="limit">The optional limit of how many to get.</param>
    /// <param name="before">The optional date to return threads created before this timestamp.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
    ///     a collection of privately archived threads.
    /// </returns>
    Task<IReadOnlyCollection<IThreadChannel>> GetPrivateArchivedThreadsAsync(
        int? limit = null, DateTimeOffset? before = null,
        RequestOptions? options = null, CancellationToken token = default
    );

    /// <summary>
    ///     Gets a collection of privately archived threads that the current bot has joined within this forum channel.
    /// </summary>
    /// <param name="limit">The optional limit of how many to get.</param>
    /// <param name="before">The optional date to return threads created before this timestamp.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
    ///     a collection of privately archived threads.
    /// </returns>
    Task<IReadOnlyCollection<IThreadChannel>> GetJoinedPrivateArchivedThreadsAsync(
        int? limit = null, DateTimeOffset? before = null,
        RequestOptions? options = null, CancellationToken token = default
    );
}
