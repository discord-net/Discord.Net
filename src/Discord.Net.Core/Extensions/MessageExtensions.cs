using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides extension methods for <see cref="IMessage" />.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        ///     Gets a URL that jumps to the message.
        /// </summary>
        /// <param name="msg">The message to jump to.</param>
        /// <returns>
        ///     A string that contains a URL for jumping to the message in chat.
        /// </returns>
        public static string GetJumpUrl(this IMessage msg)
        {
            var channel = msg.Channel;
            return $"https://discord.com/channels/{(channel is IDMChannel ? "@me" : $"{(channel as ITextChannel).GuildId}")}/{channel.Id}/{msg.Id}";
        }

        /// <summary>
        ///     Add multiple reactions to a message.
        /// </summary>
        /// <remarks>
        ///     This method does not bulk add reactions! It will send a request for each reaction inculded.
        /// </remarks>
        /// <example>
        /// <code language="cs">
        /// IEmote A = new Emoji("🅰");
        /// IEmote B = new Emoji("🅱");
        /// await msg.AddReactionsAsync(new[] { A, B });
        /// </code>
        /// </example>
        /// <param name="msg">The message to add reactions to.</param>
        /// <param name="reactions">An array of reactions to add to the message</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for adding a reaction to this message.
        /// </returns>
        /// <seealso cref="IMessage.AddReactionAsync(IEmote, RequestOptions)"/>
        /// <seealso cref="IEmote"/>
        public static async Task AddReactionsAsync(this IUserMessage msg, IEmote[] reactions, RequestOptions options = null)
        {
            foreach (var rxn in reactions)
                await msg.AddReactionAsync(rxn, options).ConfigureAwait(false);
        }
        /// <summary>
        ///     Remove multiple reactions from a message.
        /// </summary>
        /// <remarks>
        ///     This method does not bulk remove reactions! If you want to clear reactions from a message,
        ///     <see cref="IMessage.RemoveAllReactionsAsync(RequestOptions)"/>
        /// </remarks>
        /// <example>
        /// <code language="cs">
        /// await msg.RemoveReactionsAsync(currentUser, new[] { A, B });
        /// </code>
        /// </example>
        /// <param name="msg">The message to remove reactions from.</param>
        /// <param name="reactions">An array of reactions to remove from the message</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation for removing a reaction to this message.
        /// </returns>
        /// <seealso cref="IMessage.RemoveReactionAsync(IEmote, IUser, RequestOptions)"/>
        /// <seealso cref="IEmote"/>
        public static async Task RemoveReactionsAsync(this IUserMessage msg, IUser user, IEmote[] reactions, RequestOptions options = null)
        {
            foreach (var rxn in reactions)
                await msg.RemoveReactionAsync(rxn, user, options).ConfigureAwait(false);
        }

        /// <summary>
        ///     Sends an inline reply that references a message.
        /// </summary>
        /// <param name="text">The message to be sent.</param>
        /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
        /// <param name="embed">The <see cref="Discord.EmbedType.Rich"/> <see cref="Embed"/> to be sent.</param>
        /// <param name="allowedMentions">
        ///     Specifies if notifications are sent for mentioned users and roles in the message <paramref name="text"/>.
        ///     If <c>null</c>, all mentioned roles and users will be notified.
        /// </param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents an asynchronous send operation for delivering the message. The task result
        ///     contains the sent message.
        /// </returns>
        public static async Task<IUserMessage> ReplyAsync(this IUserMessage msg, string text = null, bool isTTS = false, Embed embed = null, AllowedMentions allowedMentions = null, RequestOptions options = null)
        {
            return await msg.Channel.SendMessageAsync(text, isTTS, embed, options, allowedMentions, new MessageReference(messageId: msg.Id)).ConfigureAwait(false);
        }
    }
}
