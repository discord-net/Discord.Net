using Discord.Entities.Channels.Threads;

namespace Discord;

/// <summary>
///     Represents a generic channel in a guild that can send and receive messages.
/// </summary>
public interface ITextChannel : IMessageChannel, IMentionable, INestedChannel, IIntegrationChannel,
    IModifyable<ModifyTextChannelProperties>
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
    ///     Gets the current slow-mode delay for this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    /// </returns>
    int SlowModeInterval { get; }

    /// <summary>
    ///     Gets the current default slow-mode delay for threads in this channel.
    /// </summary>
    /// <returns>
    ///     An <see cref="int" /> representing the time in seconds required before the user can send another
    ///     message; <c>0</c> if disabled.
    /// </returns>
    int DefaultSlowModeInterval { get; }

    /// <summary>
    ///     Gets the default auto-archive duration for client-created threads in this channel.
    /// </summary>
    /// <remarks>
    ///     The value of this property does not affect API thread creation, it will not respect this value.
    /// </remarks>
    /// <returns>
    ///     The default auto-archive duration for thread creation in this channel.
    /// </returns>
    ThreadArchiveDuration DefaultArchiveDuration { get; }

    /// <summary>
    ///     Creates a thread within this <see cref="ITextChannel" />.
    /// </summary>
    /// <remarks>
    ///     When <paramref name="message" /> is <see langword="null" /> the thread type will be based off of the
    ///     channel its created in. When called on a <see cref="ITextChannel" />, it creates a
    ///     <see cref="ThreadType.PublicThread" />.
    ///     When called on a <see cref="INewsChannel" />, it creates a <see cref="ThreadType.NewsThread" />. The id of the
    ///     created
    ///     thread will be the same as the id of the message, and as such a message can only have a
    ///     single thread created from it.
    /// </remarks>
    /// <param name="name">The name of the thread.</param>
    /// <param name="type">
    ///     The type of the thread.
    ///     <para>
    ///         <b>Note: </b>This parameter is not used if the <paramref name="message" /> parameter is not specified.
    ///     </para>
    /// </param>
    /// <param name="autoArchiveDuration">
    ///     The duration on which this thread archives after.
    ///     <para>
    ///         <b>Note: </b> Options <see cref="ThreadArchiveDuration.OneWeek" /> and
    ///         <see cref="ThreadArchiveDuration.ThreeDays" />
    ///         are only available for guilds that are boosted. You can check in the <see cref="IGuild.Features" /> to see if
    ///         the
    ///         guild has the <b>THREE_DAY_THREAD_ARCHIVE</b> and <b>SEVEN_DAY_THREAD_ARCHIVE</b>.
    ///     </para>
    /// </param>
    /// <param name="message">The message which to start the thread from.</param>
    /// <param name="invitable">
    ///     Whether non-moderators can add other non-moderators to a thread; only available when creating a
    ///     private thread
    /// </param>
    /// <param name="slowmode">The amount of seconds a user has to wait before sending another message (0-21600)</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous create operation. The task result contains a <see cref="IThreadChannel" />
    /// </returns>
    Task<IThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread,
        ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay,
        ulong? messageId = null, bool? invitable = null, int? slowmode = null, RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Gets a collection of active threads within this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
    ///     a collection of active threads.
    /// </returns>
    Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions? options = null,
        CancellationToken token = default);
}
