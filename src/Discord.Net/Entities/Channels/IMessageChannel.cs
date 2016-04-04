using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageChannel : IChannel
    {
        /// <summary> Gets the message in this text channel with the given id, or null if none was found. </summary>
        Task<Message> GetMessage(ulong id);
        /// <summary> Gets the last N messages from this text channel. </summary>
        /// <param name="limit"> The maximum number of messages to retrieve. </param>
        Task<IEnumerable<Message>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        /// <param name="limit"> The maximum number of messages to retrieve. </param>
        /// <param name="relativeMessageId"> The message to start downloading relative to. </param>
        /// <param name="relativeDir"> The direction, from relativeMessageId, to download messages in. </param>
        Task<IEnumerable<Message>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch, ulong? relativeMessageId = null, Relative relativeDir = Relative.Before);

        /// <summary> Sends a message to this text channel. </summary>
        Task<Message> SendMessage(string text, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<Message> SendFile(string filePath, string text = null, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<Message> SendFile(Stream stream, string filename, string text = null, bool isTTS = false);

        /// <summary> Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.</summary>
        Task TriggerTyping();
    }
}
