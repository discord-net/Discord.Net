using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic message sent by a user.
    /// </summary>
    public interface IUserMessage : IMessage
    {
        /// <summary>
        ///     Modifies this message.
        /// </summary>
        /// <remarks>
        ///     This method modifies this message with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
        /// </remarks>
        /// <example>
        ///     The following example replaces the content of the message with <c>Hello World!</c>.
        ///     <code language="cs">
        ///     await msg.ModifyAsync(x =&gt; x.Content = "Hello World!");
        ///     </code>
        /// </example>
        /// <param name="func">A delegate containing the properties to modify the message with.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null);
        /// <summary>
        ///     Modifies the suppression of this message.
        /// </summary>
        /// <remarks>
        ///     This method modifies whether or not embeds in this message are suppressed (hidden).
        /// </remarks>
        /// <param name="suppressEmbeds">Whether or not embeds in this message should be suppressed.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous modification operation.
        /// </returns>
        Task ModifySuppressionAsync(bool suppressEmbeds, RequestOptions options = null);
        /// <summary>
        ///     Adds this message to its channel's pinned messages.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for pinning this message.
        /// </returns>
        Task PinAsync(RequestOptions options = null);
        /// <summary>
        ///     Removes this message from its channel's pinned messages.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for unpinning this message.
        /// </returns>
        Task UnpinAsync(RequestOptions options = null);

        /// <summary>
        ///     Transforms this message's text into a human-readable form by resolving its tags.
        /// </summary>
        /// <param name="userHandling">Determines how the user tag should be handled.</param>
        /// <param name="channelHandling">Determines how the channel tag should be handled.</param>
        /// <param name="roleHandling">Determines how the role tag should be handled.</param>
        /// <param name="everyoneHandling">Determines how the @everyone tag should be handled.</param>
        /// <param name="emojiHandling">Determines how the emoji tag should be handled.</param>
        string Resolve(
            TagHandling userHandling = TagHandling.Name,
            TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name,
            TagHandling everyoneHandling = TagHandling.Ignore,
            TagHandling emojiHandling = TagHandling.Name);
    }
}
