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
        ///     Gets whether the channel is NSFW.
        /// </summary>
        bool IsNsfw { get; }

        /// <summary>
        ///     Gets the current topic for this text channel.
        /// </summary>
        string Topic { get; }

        /// <summary>
        ///     Bulk deletes multiple messages.
        /// </summary>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
        /// <summary>
        ///     Bulk deletes multiple messages.
        /// </summary>
        Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null);

        /// <summary>
        ///     Modifies this text channel.
        /// </summary>
        Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null);

        /// <summary>
        ///     Creates a webhook in this text channel.
        /// </summary>
        Task<IWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null);
        /// <summary>
        ///     Gets the webhook in this text channel with the provided ID, or <see langword="null"/> if not found.
        /// </summary>
        Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);
        /// <summary>
        ///     Gets the webhooks for this text channel.
        /// </summary>
        Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);
    }
}
