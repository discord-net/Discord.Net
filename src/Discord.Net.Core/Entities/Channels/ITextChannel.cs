using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel in a guild that can send and receive messages.
    /// </summary>
    public interface ITextChannel : IMessageChannel, IMentionable, INestedChannel, IIntegrationChannel
    {
        /// <summary>
        ///     Gets a value that indicates whether the channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the channel has the NSFW flag enabled; otherwise <c>false</c>.
        /// </returns>
        bool IsNsfw { get; }

        /// <summary>
        ///     Gets the current topic for this text channel.
        /// </summary>
        /// <returns>
        ///     A string representing the topic set in the channel; <c>null</c> if none is set.
        /// </returns>
        string Topic { get; }

        /// <summary>
        ///     Gets the current slow-mode delay for this channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        /// </returns>
        int SlowModeInterval { get; }

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
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <example>
        ///     <para>The following example gets 250 messages from the channel and deletes them.</para>
        ///     <code language="cs">
        ///     var messages = await textChannel.GetMessagesAsync(250).FlattenAsync();
        ///     await textChannel.DeleteMessagesAsync(messages);
        ///     </code>
        /// </example>
        /// <remarks>
        ///     This method attempts to remove the messages specified in bulk.
        ///     <note type="important">
        ///         Due to the limitation set by Discord, this method can only remove messages that are posted within 14 days!
        ///     </note>
        /// </remarks>
        /// <param name="messages">The messages to be bulk-deleted.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous bulk-removal operation.
        /// </returns>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
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
        /// <returns>
        ///     A task that represents the asynchronous bulk-removal operation.
        /// </returns>
        Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null);

        /// <summary>
        ///     Modifies this text channel.
        /// </summary>
        /// <param name="func">The delegate containing the properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        /// <seealso cref="TextChannelProperties"/>
        Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Creates a thread within this <see cref="ITextChannel"/>.
        /// </summary>
        /// <remarks>
        ///     When <paramref name="message"/> is <see langword="null"/> the thread type will be based off of the
        ///     channel its created in. When called on a <see cref="ITextChannel"/>, it creates a <see cref="ThreadType.PublicThread"/>.
        ///     When called on a <see cref="INewsChannel"/>, it creates a <see cref="ThreadType.NewsThread"/>. The id of the created
        ///     thread will be the same as the id of the message, and as such a message can only have a
        ///     single thread created from it.
        /// </remarks>
        /// <param name="name">The name of the thread.</param>
        /// <param name="type">
        ///     The type of the thread.
        ///     <para>
        ///         <b>Note: </b>This parameter is not used if the <paramref name="message"/> parameter is not specified.
        ///     </para>
        /// </param>
        /// <param name="autoArchiveDuration">
        ///     The duration on which this thread archives after.
        ///     <para>
        ///         <b>Note: </b> Options <see cref="ThreadArchiveDuration.OneWeek"/> and <see cref="ThreadArchiveDuration.ThreeDays"/>
        ///         are only available for guilds that are boosted. You can check in the <see cref="IGuild.Features"/> to see if the 
        ///         guild has the <b>THREE_DAY_THREAD_ARCHIVE</b> and <b>SEVEN_DAY_THREAD_ARCHIVE</b>.
        ///     </para>
        /// </param>
        /// <param name="message">The message which to start the thread from.</param>
        /// <param name="invitable">Whether non-moderators can add other non-moderators to a thread; only available when creating a private thread</param>
        /// <param name="slowmode">The amount of seconds a user has to wait before sending another message (0-21600)</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous create operation. The task result contains a <see cref="IThreadChannel"/>
        /// </returns>
        Task<IThreadChannel> CreateThreadAsync(string name, ThreadType type = ThreadType.PublicThread, ThreadArchiveDuration autoArchiveDuration = ThreadArchiveDuration.OneDay,
            IMessage message = null, bool? invitable = null, int? slowmode = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of active threads within this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous get operation for retrieving the threads. The task result contains
        ///     a collection of active threads.
        /// </returns>
        Task<IReadOnlyCollection<IThreadChannel>> GetActiveThreadsAsync(RequestOptions options = null);
    }
}
