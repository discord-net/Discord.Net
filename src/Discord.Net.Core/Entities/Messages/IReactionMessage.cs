using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a message where reactions can be added or removed.
    /// </summary>
    public interface IReactionMessage : IMessage
    {
        /// <summary>
        ///     Gets all reactions included in this message.
        /// </summary>
        IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

        /// <summary>
        ///     Adds a reaction to this message.
        /// </summary>
        /// <example>
        ///     The following example adds the reaction, <c>💕</c>, to the message.
        ///     <code language="cs">
        ///     await msg.AddReactionAsync(new Emoji("\U0001f495"));
        ///     </code>
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
        ///     The following example removes the reaction, <c>💕</c>, added by the message author from the message.
        ///     <code language="cs">
        ///     await msg.RemoveReactionAsync(new Emoji("\U0001f495"), msg.Author);
        ///     </code>
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
        ///     Removes a reaction from message.
        /// </summary>
        /// <example>
        ///     The following example removes the reaction, <c>💕</c>, added by the user with ID 84291986575613952 from the message.
        ///     <code language="cs">
        ///     await msg.RemoveReactionAsync(new Emoji("\U0001f495"), 84291986575613952);
        ///     </code>
        /// </example>
        /// <param name="emote">The emoji used to react to this message.</param>
        /// <param name="userId">The ID of the user that added the emoji.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for removing a reaction to this message.
        /// </returns>
        /// <seealso cref="IEmote"/>
        Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null);
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
        ///     The following example gets the users that have reacted with the emoji <c>💕</c> to the message.
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
    }
}
