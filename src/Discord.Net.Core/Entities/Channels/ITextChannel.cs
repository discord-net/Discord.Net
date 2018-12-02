using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel in a guild that can send and receive messages.
    /// </summary>
    public interface ITextChannel : IMessageChannel, IMentionable, INestedChannel
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
        ///     An <see cref="Int32"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        /// </returns>
        int SlowModeInterval { get; }

        /// <summary>
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <example>
        ///     The following example gets 250 messages from the channel and deletes them.
        ///     <code lang="cs">
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
        ///     Creates a webhook in this text channel.
        /// </summary>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The avatar of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous creation operation. The task result contains the newly created
        ///     webhook.
        /// </returns>
        Task<IWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null);
        /// <summary>
        ///     Gets a webhook available in this text channel.
        /// </summary>
        /// <param name="id">The identifier of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a webhook associated
        ///     with the identifier; <c>null</c> if the webhook is not found.
        /// </returns>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets the webhooks available in this text channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
        ///     of webhooks that is available in this channel.
        /// </returns>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);
    }
}
