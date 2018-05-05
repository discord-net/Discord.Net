using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic channel that can send and receive messages.
    /// </summary>
    public interface IMessageChannel : IChannel
    {
        /// <summary>
        ///     Sends a message to this message channel.
        /// </summary>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     An awaitable Task containing the message sent to the channel.
        /// </returns>
        Task<IUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null);
#if FILESYSTEM
        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <param name="filePath">The file path of the file.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <remarks>
        ///     If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you may
        ///     upload the file and refer to the file with "attachment://filename.ext" in the 
        ///     <see cref="Discord.EmbedBuilder.ImageUrl"/>.
        /// </remarks>
        /// <returns>
        ///     An awaitable Task containing the message sent to the channel.
        /// </returns>
        Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
#endif
        /// <summary>
        ///     Sends a file to this message channel with an optional caption.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> of the file to be sent.</param>
        /// <param name="filename">The name of the attachment.</param>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <remarks>
        ///     If you wish to upload an image and have it embedded in a <see cref="EmbedType.Rich"/> embed, you may
        ///     upload the file and refer to the file with "attachment://filename.ext" in the 
        ///     <see cref="Discord.EmbedBuilder.ImageUrl"/>.
        /// </remarks>
        /// <returns>
        ///     An awaitable Task containing the message sent to the channel.
        /// </returns>
        Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);

        /// <summary>
        ///     Gets a message from this message channel with the given id, or <see langword="null"/> if not found.
        /// </summary>
        /// <param name="id">The ID of the message.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     The message gotten from either the cache or the download, or <see langword="null"/> if none is found.
        /// </returns>
        Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets the last N messages from this message channel.
        /// </summary>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages. Flattening the paginated response into a collection of messages with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, 
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <param name="fromMessageId">The ID of the starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages. Flattening the paginated response into a collection of messages with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, 
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);

        /// <summary>
        ///     Gets a collection of messages in this channel.
        /// </summary>
        /// <param name="fromMessage">The starting message to get the messages from.</param>
        /// <param name="dir">The direction of the messages to be gotten from.</param>
        /// <param name="limit">The numbers of message to be gotten from.</param>
        /// <param name="mode">The <see cref="CacheMode"/> that determines whether the object should be fetched from
        /// cache.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     Paged collection of messages. Flattening the paginated response into a collection of messages with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the messages.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, 
            CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null);
        /// <summary>
        ///     Gets a collection of pinned messages in this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A collection of messages.
        /// </returns>
        Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync(RequestOptions options = null);

        /// <summary>
        ///     Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        Task TriggerTypingAsync(RequestOptions options = null);
        /// <summary>
        ///     Continuously broadcasts the "user is typing" message to all users in this channel until the returned
        ///     object is disposed.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        IDisposable EnterTypingState(RequestOptions options = null);
    }
}
