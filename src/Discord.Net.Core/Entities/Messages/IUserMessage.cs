using System;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic message sent by a user.
    /// </summary>
    public interface IUserMessage : IMessage
    {
        /// <summary>
        ///     Gets the referenced message if it is a crosspost, channel follow add, pin, or reply message.
        /// </summary>
        /// <returns>
        ///     The referenced message, if any is associated and still exists.
        /// </returns>
        IUserMessage ReferencedMessage { get; }

        /// <summary>
        ///     Modifies this message.
        /// </summary>
        /// <remarks>
        ///     This method modifies this message with the specified properties. To see an example of this
        ///     method and what properties are available, please refer to <see cref="MessageProperties"/>.
        /// </remarks>
        /// <example>
        ///     <para>The following example replaces the content of the message with <c>Hello World!</c>.</para>
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
        ///     Publishes (crossposts) this message.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for publishing this message.
        /// </returns>
        /// <remarks>
        ///     <note type="warning">
        ///         This call will throw an <see cref="InvalidOperationException"/> if attempted in a non-news channel.
        ///     </note>
        ///     This method will publish (crosspost) the message. Please note, publishing (crossposting), is only available in news channels.
        /// </remarks>
        Task CrosspostAsync(RequestOptions options = null);

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
