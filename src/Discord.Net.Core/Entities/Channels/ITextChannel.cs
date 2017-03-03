using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface ITextChannel : IMessageChannel, IMentionable, IGuildChannel
    {
        /// <summary> Gets the current topic for this text channel. </summary>
        string Topic { get; }

        /// <summary> Modifies this text channel. </summary>
        Task ModifyAsync(Action<TextChannelProperties> func, RequestOptions options = null);
    }
}