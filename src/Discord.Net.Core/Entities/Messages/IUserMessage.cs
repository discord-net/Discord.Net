using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface IUserMessage : IMessage
    {
        /// <summary> Modifies this message. </summary>
        Task ModifyAsync(Action<ModifyMessageParams> func, RequestOptions options = null);
        /// <summary> Adds this message to its channel's pinned messages. </summary>
        Task PinAsync(RequestOptions options = null);
        /// <summary> Removes this message from its channel's pinned messages. </summary>
        Task UnpinAsync(RequestOptions options = null);
        
        /// <summary> Transforms this message's text into a human readable form by resolving its tags. </summary>
        string Resolve(
            TagHandling userHandling = TagHandling.Name,
            TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name,
            TagHandling everyoneHandling = TagHandling.Ignore,
            TagHandling emojiHandling = TagHandling.Name);
    }
}
