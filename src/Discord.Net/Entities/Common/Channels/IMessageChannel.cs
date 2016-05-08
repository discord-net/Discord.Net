using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageChannel : IChannel
    {
        /// <summary> Gets the message in this message channel with the given id, or null if none was found. </summary>
        Task<IMessage> GetMessage(ulong id);
        /// <summary> Gets the last N messages from this message channel. </summary>
        Task<IEnumerable<IMessage>> GetMessages(int limit = DiscordConfig.MaxMessagesPerBatch);
        /// <summary> Gets a collection of messages in this channel. </summary>
        Task<IEnumerable<IMessage>> GetMessages(ulong fromMessageId, Direction dir, int limit = DiscordConfig.MaxMessagesPerBatch);

        /// <summary> Sends a message to this text channel. </summary>
        Task<IMessage> SendMessage(string text, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<IMessage> SendFile(string filePath, string text = null, bool isTTS = false);
        /// <summary> Sends a file to this text channel, with an optional caption. </summary>
        Task<IMessage> SendFile(Stream stream, string filename, string text = null, bool isTTS = false);
        
        /// <summary> Bulk deletes multiple messages. </summary>
        Task DeleteMessages(IEnumerable<IMessage> messages);

        /// <summary> Broadcasts the "user is typing" message to all users in this channel, lasting 10 seconds.</summary>
        Task TriggerTyping();
    }
}
