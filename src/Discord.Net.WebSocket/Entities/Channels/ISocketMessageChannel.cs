using Discord.Rest;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a generic WebSocket-based channel that can send and receive messages.
    /// </summary>
    public interface ISocketMessageChannel : IMessageChannel
    {
        /// <summary> Gets all messages in this channel's cache. </summary>
        IReadOnlyCollection<SocketMessage> CachedMessages { get; }

        /// <summary>
        ///     Sends a message to this message channel.
        /// </summary>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        new Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null);
#if FILESYSTEM
        /// <summary>
        ///     Sends a file to this message channel, with an optional caption.
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
        new Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);
#endif
        /// <summary>
        ///     Sends a file to this message channel, with an optional caption.
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
        new Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null);

        SocketMessage GetCachedMessage(ulong id);
        /// <summary> Gets the last N messages from this message channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary>
        ///     Gets a collection of pinned messages in this channel.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A collection of messages.
        /// </returns>
        new Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null);
    }
}
