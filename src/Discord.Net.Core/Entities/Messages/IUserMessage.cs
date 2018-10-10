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
        /// <example>
        /// <code language="cs">
        /// await msg.ModifyAsync(x =&gt; x.Content = "Hello World!");
        /// </code>
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
        ///     Gets all reactions included in this message.
        /// </summary>
        IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

        /// <summary>
        ///     Adds a reaction to this message.
        /// </summary>
        /// <example>
        /// <code language="cs">
        /// await msg.AddReactionAsync(new Emoji("\U0001f495"));
        /// </code>
        /// </example>
        /// <param name="emote">The emoji used to react to this message.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for adding a reaction to this message.
        /// </returns>
        /// <seealso cref="IEmote"/>
        Task AddReactionAsync(IEmote emote, RequestOptions options = null);
        /// <summary>
        ///     Removes a reaction from message.
        /// </summary>
        /// <example>
        /// <code language="cs">
        /// await msg.RemoveReactionAsync(new Emoji("\U0001f495"), msg.Author);
        /// </code>
        /// </example>
        /// <param name="emote">The emoji used to react to this message.</param>
        /// <param name="user">The user that added the emoji.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for removing a reaction to this message.
        /// </returns>
        /// <seealso cref="IEmote"/>
        Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null);
        /// <summary>
        ///     Removes all reactions from this message.
        /// </summary>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        Task RemoveAllReactionsAsync(RequestOptions options = null);

        /// <summary>
        ///     Gets all users that reacted to a message with a given emote.
        /// </summary>
        /// <example>
        ///     <code language="cs">
        ///     var emoji = new Emoji("\U0001f495");
        ///     var reactedUsers = await message.GetReactionUsersAsync(emoji, 100).FlattenAsync();
        ///     </code>
        /// </example>
        /// <param name="emoji">The emoji that represents the reaction that you wish to get.</param>
        /// <param name="limit">The number of users to request.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A paged collection containing a read-only collection of users that has reacted to this message.
        ///     Flattening the paginated response into a collection of users with 
        ///     <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> is required if you wish to access the users.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null);

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
