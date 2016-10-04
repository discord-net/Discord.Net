using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Rest
{
    public interface IRestMessageChannel : IMessageChannel
    {
        /// <summary> Sends a message to this message channel. </summary>
        new Task<RestUserMessage> SendMessageAsync(string text, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        new Task<RestUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false);

        /// <summary> Gets a message from this message channel with the given id, or null if not found. </summary>
        Task<RestMessage> GetMessageAsync(ulong id);
        /// <summary> Gets the last N messages from this message channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<RestMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of pinned messages in this channel. </summary>
        new Task<IReadOnlyCollection<RestMessage>> GetPinnedMessagesAsync();
    }
}
