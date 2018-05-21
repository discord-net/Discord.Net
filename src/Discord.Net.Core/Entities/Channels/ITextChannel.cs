using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel in a guild that can send and receive messages.
    /// </summary>
    public interface ITextChannel : IMessageChannel, IMentionable, IGuildChannel
    {
        /// <summary>
        ///     Determines whether the channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the channel has the NSFW flag enabled; otherwise, <c>false</c>.
        /// </returns>
        bool IsNsfw { get; }

        /// <summary>
        ///     Gets the current topic for this text channel.
        /// </summary>
        /// <returns>
        ///     The topic set in the channel, or <c>null</c> if none is set.
        /// </returns>
        string Topic { get; }

        /// <summary>
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///     This method can only remove messages that are posted within 14 days!
        ///     </note>
        /// </remarks>
        /// <param name="messages">The messages to be bulk-deleted.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
        /// <summary>
        ///     Bulk-deletes multiple messages.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///     This method can only remove messages that are posted within 14 days!
        ///     </note>
        /// </remarks>
        /// <param name="messageIds">The IDs of the messages to be bulk-deleted.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null);

        /// <summary>
        ///     Modifies this text channel.
        /// </summary>
        /// <param name="func">The properties to modify the channel with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Creates a webhook in this text channel.
        /// </summary>
        /// <param name="name">The name of the webhook.</param>
        /// <param name="avatar">The avatar of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The created webhook.
        /// </returns>
        Task<IWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null);
        /// <summary>
        ///     Gets the webhook in this text channel with the provided ID.
        /// </summary>
        /// <param name="id">The ID of the webhook.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A webhook associated with the <paramref name="id"/>, or <c>null</c> if not found.
        /// </returns>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets the webhooks for this text channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A collection of webhooks.
        /// </returns>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);
    }
}
