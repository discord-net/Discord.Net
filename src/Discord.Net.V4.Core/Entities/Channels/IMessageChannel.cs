namespace Discord;

/// <summary>
///     Represents a generic channel that can send and receive messages.
/// </summary>
public interface IMessageChannel : IChannel
{
    /// <summary>
    ///     Deletes a message.
    /// </summary>
    /// <param name="messageId">The snowflake identifier of the message that would be removed.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous removal operation.
    /// </returns>
    /// <exception cref="MissingPermissionException">The current user doesn't have permission to delete the message.</exception>
    Task DeleteMessageAsync(ulong messageId, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Bulk-deletes multiple messages.
    /// </summary>
    /// <remarks>
    ///     This method attempts to remove the messages specified in bulk.
    ///     <note type="important">
    ///         Due to the limitation set by Discord, this method can only remove messages that are posted within 14 days!
    ///     </note>
    /// </remarks>
    /// <param name="messageIds">The snowflake identifier of the messages to be bulk-deleted.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous bulk-removal operation.
    /// </returns>
    /// <exception cref="MissingPermissionException">The current user doesn't have permission to delete the message.</exception>
    Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Sends a message to this message channel.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous send operation for delivering the message. The task result
    ///     contains the sent message.
    /// </returns>
    Task SendMessageAsync(CreateMessageProperties message, RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Gets a message from this message channel.
    /// </summary>
    /// <param name="id">The snowflake identifier of the message.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents an asynchronous get operation for retrieving the message. The task result contains
    ///     the retrieved message; <see langword="null" /> if no message is found with the specified identifier.
    /// </returns>
    Task<IMessage> GetMessageAsync(ulong id, RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Gets the last N messages from this message channel.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         The returned collection is an asynchronous enumerable object; one must call
    ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}" /> to access the individual messages as a
    ///         collection.
    ///     </note>
    ///     <note type="warning">
    ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
    ///         rate limit, causing your bot to freeze!
    ///     </note>
    ///     This method will attempt to fetch the number of messages specified under <paramref name="limit" />. The
    ///     library will attempt to split up the requests according to your <paramref name="limit" /> and
    ///     <see cref="DiscordConfig.MaxMessagesPerBatch" />. In other words, should the user request 500 messages,
    ///     and the <see cref="Discord.DiscordConfig.MaxMessagesPerBatch" /> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        int limit = DiscordConfig.MaxMessagesPerBatch, RequestOptions? options = null,
        CancellationToken token = default
    );

    /// <summary>
    ///     Gets a collection of messages in this channel.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         The returned collection is an asynchronous enumerable object; one must call
    ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}" /> to access the individual messages as a
    ///         collection.
    ///     </note>
    ///     <note type="warning">
    ///         Do not fetch too many messages at once! This may cause unwanted preemptive rate limit or even actual
    ///         rate limit, causing your bot to freeze!
    ///     </note>
    ///     This method will attempt to fetch the number of messages specified under <paramref name="limit" /> around
    ///     the message <paramref name="fromMessageId" /> depending on the <paramref name="dir" />. The library will
    ///     attempt to split up the requests according to your <paramref name="limit" /> and
    ///     <see cref="DiscordConfig.MaxMessagesPerBatch" />. In other words, should the user request 500 messages,
    ///     and the <see cref="Discord.DiscordConfig.MaxMessagesPerBatch" /> constant is <c>100</c>, the request will
    ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
    ///     of flattening.
    /// </remarks>
    /// <param name="fromMessageId">The ID of the starting message to get the messages from.</param>
    /// <param name="dir">The direction of the messages to be gotten from.</param>
    /// <param name="limit">The numbers of message to be gotten from.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// cache.
    /// </param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     Paged collection of messages.
    /// </returns>
    IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(
        ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch,
        RequestOptions? options = null, CancellationToken token = default
    );

    /// <summary>
    ///     Gets a collection of pinned messages in this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation for retrieving pinned messages in this channel.
    ///     The task result contains a collection of messages found in the pinned messages.
    /// </returns>
    Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions? options = null,
        CancellationToken token = default);

    /// <summary>
    ///     Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation that triggers the broadcast.
    /// </returns>
    Task TriggerTypingAsync(RequestOptions? options = null, CancellationToken token = default);

    /// <summary>
    ///     Continuously broadcasts the "user is typing" message to all users in this channel until the returned
    ///     object is disposed.
    /// </summary>
    /// <example>
    ///     <para>The following example keeps the client in the typing state until <c>LongRunningAsync</c> has finished.</para>
    ///     <code language="cs" region="EnterTypingState"
    ///         source="..\..\..\Discord.Net.Examples\Core\Entities\Channels\IMessageChannel.Examples.cs" />
    /// </example>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <param name="token">A <see cref="CancellationToken" /> used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A disposable object that, upon its disposal, will stop the client from broadcasting its typing state in
    ///     this channel.
    /// </returns>
    IDisposable EnterTypingState(RequestOptions? options = null, CancellationToken token = default);
}
