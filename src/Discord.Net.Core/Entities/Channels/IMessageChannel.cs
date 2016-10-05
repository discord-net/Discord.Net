using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageChannel : IChannel
    {
        /// <summary> Sends a message to this message channel. </summary>
        Task<IUserMessage> SendMessageAsync(string text, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<IUserMessage> SendFileAsync(string filePath, string text = null, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<IUserMessage> SendFileAsync(Stream stream, string filename, string text = null, bool isTTS = false);

        /// <summary> Gets a message from this message channel with the given id, or null if not found. </summary>
        Task<IMessage> GetMessageAsync(ulong id, CacheMode mode = CacheMode.AllowDownload);
        /// <summary> Gets the last N messages from this message channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(int limit = DiscordConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload);
        /// <summary> Gets a collection of messages in this channel. </summary>
        IAsyncEnumerable<IReadOnlyCollection<IMessage>> GetMessagesAsync(IMessage fromMessage, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch, CacheMode mode = CacheMode.AllowDownload);
        /// <summary> Gets a collection of pinned messages in this channel. </summary>
        Task<IReadOnlyCollection<IMessage>> GetPinnedMessagesAsync();
        /// <summary> Bulk deletes multiple messages. </summary>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages);        

        /// <summary> Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.</summary>
        IDisposable EnterTypingState();
    }
}
