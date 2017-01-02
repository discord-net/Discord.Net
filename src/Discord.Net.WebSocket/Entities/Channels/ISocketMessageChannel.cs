using Discord.Rest;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ISocketMessageChannel : IMessageChannel
    {
        /// <summary> Gets all messages in this channel's cache. </summary>
        IReadOnlyCollection<SocketMessage> CachedMessages { get; }

        /// <summary> Sends a message to this message channel. </summary>
        new Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false, Embed embed = null, RequestOptions options = null);
#if NETSTANDARD1_3
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false, RequestOptions options = null);
#endif
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, RequestOptions options = null);

        SocketMessage GetCachedMessage(ulong id);
        /// <summary> Gets the last N messages from this message channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IReadOnlyCollection<SocketMessage> GetCachedMessages(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of pinned messages in this channel. </summary>
        new Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync(RequestOptions options = null);
    }
}
