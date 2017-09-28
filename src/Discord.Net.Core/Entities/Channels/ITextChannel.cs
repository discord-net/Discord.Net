using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface ITextChannel : IMessageChannel, IMentionable, IGuildChannel
    {
        /// <summary> Checks if the channel is NSFW. </summary>
        bool IsNsfw { get; }

        /// <summary> Gets the current topic for this text channel. </summary>
        string Topic { get; }

        /// <summary> Bulk deletes multiple messages. </summary>
        Task DeleteMessagesAsync(IEnumerable<IMessage> messages, RequestOptions options = null);
        /// <summary> Bulk deletes multiple messages. </summary>
        Task DeleteMessagesAsync(IEnumerable<ulong> messageIds, RequestOptions options = null);

        /// <summary> Modifies this text channel. </summary>
        Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null);
    }
}