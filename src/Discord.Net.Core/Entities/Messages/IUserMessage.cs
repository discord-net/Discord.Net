using Discord.API.Rest;
using System;
using System.Threading.Tasks;

namespace Discord
{
    public interface IUserMessage : IMessage, IDeletable
    {
        /// <summary> Modifies this message. </summary>
        Task ModifyAsync(Action<ModifyMessageParams> func);
        /// <summary> Adds this message to its channel's pinned messages. </summary>
        Task PinAsync();
        /// <summary> Removes this message from its channel's pinned messages. </summary>
        Task UnpinAsync();

        /// <summary> Transforms this message's text into a human readable form, resolving mentions to that object's name. </summary>
        string Resolve(int startIndex, int length,
            UserMentionHandling userHandling = UserMentionHandling.Name,
            ChannelMentionHandling channelHandling = ChannelMentionHandling.Name,
            RoleMentionHandling roleHandling = RoleMentionHandling.Name,
            EveryoneMentionHandling everyoneHandling = EveryoneMentionHandling.Ignore);
        /// <summary> Transforms this message's text into a human readable form, resolving mentions to that object's name. </summary>
        string Resolve(
            UserMentionHandling userHandling = UserMentionHandling.Name,
            ChannelMentionHandling channelHandling = ChannelMentionHandling.Name,
            RoleMentionHandling roleHandling = RoleMentionHandling.Name,
            EveryoneMentionHandling everyoneHandling = EveryoneMentionHandling.Ignore);
    }
}
