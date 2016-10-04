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
        new Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false);

        SocketMessage GetCachedMessage(ulong id);
        /// <summary> Gets a message from this message channel with the given id, or null if not found. </summary>
        Task<IMessage> GetMessageAsync(ulong id);
        /// <summary> Gets the last N messages from this message channel. </summary>
        Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        Task<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of pinned messages in this channel. </summary>
        new Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync();
    }
}
