using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    public interface IUserMessage : IMessage
    {
        /// <summary> Modifies this message. </summary>
        Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null);
        /// <summary> Adds this message to its channel's pinned messages. </summary>
        Task PinAsync(RequestOptions options = null);
        /// <summary> Removes this message from its channel's pinned messages. </summary>
        Task UnpinAsync(RequestOptions options = null);

        /// <summary> Returns all reactions included in this message. </summary>
        IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

        /// <summary> Adds a reaction to this message. </summary>
        Task AddReactionAsync(IEmote emote, RequestOptions options = null);
        /// <summary> Removes a reaction from message. </summary>
        Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null);
        /// <summary> Removes all reactions from this message. </summary>
        Task RemoveAllReactionsAsync(RequestOptions options = null);
        Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(string emoji, int limit = 100, ulong? afterUserId = null, RequestOptions options = null);

        /// <summary> Transforms this message's text into a human readable form by resolving its tags. </summary>
        string Resolve(
            TagHandling userHandling = TagHandling.Name,
            TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name,
            TagHandling everyoneHandling = TagHandling.Ignore,
            TagHandling emojiHandling = TagHandling.Name);
    }
}
