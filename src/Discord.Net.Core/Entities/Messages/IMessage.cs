using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a message object.
    /// </summary>
    public interface IMessage : ISnowflakeEntity, IDeletable
    {
        /// <summary>
        ///     Gets the type of this message.
        /// </summary>
        MessageType Type { get; }
        /// <summary>
        ///     Gets the source type of this message.
        /// </summary>
        MessageSource Source { get; }
        /// <summary>
        ///     Gets the value that indicates whether this message was meant to be read-aloud by Discord.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if this message was sent as a text-to-speech message; otherwise <see langword="false" />.
        /// </returns>
        bool IsTTS { get; }
        /// <summary>
        ///     Gets the value that indicates whether this message is pinned.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if this message was added to its channel's pinned messages; otherwise <see langword="false" />.
        /// </returns>
        bool IsPinned { get; }
        /// <summary>
        ///     Gets the value that indicates whether or not this message's embeds are suppressed.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if the embeds in this message have been suppressed (made invisible); otherwise <see langword="false" />.
        /// </returns>
        bool IsSuppressed { get; }
        /// <summary>
        ///     Gets the value that indicates whether this message mentioned everyone.
        /// </summary>
        /// <returns>
        ///     <see langword="true" /> if this message mentioned everyone; otherwise <see langword="false" />.
        /// </returns>
        bool MentionedEveryone { get; }
        /// <summary>
        ///     Gets the content for this message.
        /// </summary>
        /// <remarks>
        ///     This will be empty if the privileged <see cref="GatewayIntents.MessageContent"/> is disabled.
        /// </remarks>
        /// <returns>
        ///     A string that contains the body of the message; note that this field may be empty if there is an embed.
        /// </returns>
        string Content { get; }
        /// <summary>
        ///     Gets the clean content for this message.
        /// </summary>
        /// <remarks>
        ///     This will be empty if the privileged <see cref="GatewayIntents.MessageContent"/> is disabled.
        /// </remarks>
        /// <returns>
        ///     A string that contains the body of the message stripped of mentions, markdown, emojis and pings; note that this field may be empty if there is an embed.
        /// </returns>
        string CleanContent { get; }
        /// <summary>
        ///     Gets the time this message was sent.
        /// </summary>
        /// <returns>
        ///     Time of when the message was sent.
        /// </returns>
        DateTimeOffset Timestamp { get; }
        /// <summary>
        ///     Gets the time of this message's last edit.
        /// </summary>
        /// <returns>
        ///     Time of when the message was last edited; <see langword="null" /> if the message is never edited.
        /// </returns>
        DateTimeOffset? EditedTimestamp { get; }

        /// <summary>
        ///     Gets the source channel of the message.
        /// </summary>
        IMessageChannel Channel { get; }
        /// <summary>
        ///     Gets the author of this message.
        /// </summary>
        IUser Author { get; }

        /// <summary>
        ///     Gets the thread that was started from this message.
        /// </summary>
        /// <returns>
        ///    An <see cref="IThreadChannel"/> object if this message has thread attached; otherwise <see langword="null"/>.
        /// </returns>
        IThreadChannel Thread { get; }

        /// <summary>
        ///     Gets all attachments included in this message.
        /// </summary>
        /// <remarks>
        ///     This property gets a read-only collection of attachments associated with this message. Depending on the
        ///     user's end-client, a sent message may contain one or more attachments. For example, mobile users may
        ///     attach more than one file in their message, while the desktop client only allows for one.
        /// </remarks>
        /// <returns>
        ///     A read-only collection of attachments.
        /// </returns>
        IReadOnlyCollection<IAttachment> Attachments { get; }
        /// <summary>
        ///     Gets all embeds included in this message.
        /// </summary>
        /// <remarks>
        ///     This property gets a read-only collection of embeds associated with this message. Depending on the
        ///     message, a sent message may contain one or more embeds. This is usually true when multiple link previews
        ///     are generated; however, only one <see cref="EmbedType.Rich"/> <see cref="Embed"/> can be featured.
        /// </remarks>
        /// <returns>
        ///     A read-only collection of embed objects.
        /// </returns>
        IReadOnlyCollection<IEmbed> Embeds { get; }
        /// <summary>
        ///     Gets all tags included in this message's content.
        /// </summary>
        IReadOnlyCollection<ITag> Tags { get; }
        /// <summary>
        ///     Gets the IDs of channels mentioned in this message.
        /// </summary>
        /// <returns>
        ///     A read-only collection of channel IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedChannelIds { get; }
        /// <summary>
        ///     Gets the IDs of roles mentioned in this message.
        /// </summary>
        /// <returns>
        ///     A read-only collection of role IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedRoleIds { get; }
        /// <summary>
        ///     Gets the IDs of users mentioned in this message.
        /// </summary>
        /// <returns>
        ///     A read-only collection of user IDs.
        /// </returns>
        IReadOnlyCollection<ulong> MentionedUserIds { get; }
        /// <summary>
        ///     Gets the activity associated with a message.
        /// </summary>
        /// <remarks>
        ///     Sent with Rich Presence-related chat embeds. This often refers to activity that requires end-user's
        ///     interaction, such as a Spotify Invite activity.
        /// </remarks>
        /// <returns>
        ///     A message's activity, if any is associated.
        /// </returns>
        MessageActivity Activity { get; }
        /// <summary>
        ///     Gets the application associated with a message.
        /// </summary>
        /// <remarks>
        ///     Sent with Rich-Presence-related chat embeds.
        /// </remarks>
        /// <returns>
        ///     A message's application, if any is associated.
        /// </returns>
        MessageApplication Application { get; }

        /// <summary>
        ///     Gets the reference to the original message if it is a crosspost, channel follow add, pin, or reply message.
        /// </summary>
        /// <remarks>
        ///     Sent with cross-posted messages, meaning they were published from news channels
        ///     and received by subscriber channels, channel follow adds, pins, and message replies.
        /// </remarks>
        /// <returns>
        ///     A message's reference, if any is associated.
        /// </returns>
        MessageReference Reference { get; }

        /// <summary>
        ///     Gets all reactions included in this message.
        /// </summary>
        IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions { get; }

        /// <summary>
        ///     The <see cref="IMessageComponent"/>'s attached to this message
        /// </summary>
        IReadOnlyCollection<IMessageComponent> Components { get; }

        /// <summary>
        ///     Gets all stickers items included in this message.
        /// </summary>
        /// <returns>
        ///     A read-only collection of sticker item objects.
        /// </returns>
        IReadOnlyCollection<IStickerItem> Stickers { get; }

        /// <summary>
        ///     Gets the flags related to this message.
        /// </summary>
        /// <remarks>
        ///     This value is determined by bitwise OR-ing <see cref="MessageFlags"/> values together.
        /// </remarks>
        /// <returns>
        ///     A message's flags, if any is associated.
        /// </returns>
        MessageFlags? Flags { get; }

        /// <summary>
        ///     Gets the interaction this message is a response to.
        /// </summary>
        /// <returns>
        ///     A <see cref="IMessageInteraction"/> if the message is a response to an interaction; otherwise <see langword="null"/>.
        /// </returns>
        [Obsolete("This property will be deprecated soon. Use IUserMessage.InteractionMetadata instead.")]
        IMessageInteraction Interaction { get; }

        /// <summary>
        ///     Gets the data of the role subscription purchase or renewal that prompted this <see cref="MessageType.RoleSubscriptionPurchase"/> message.
        /// </summary> 
        /// <returns>
        ///     A <see cref="MessageRoleSubscriptionData"/> if the message is a role subscription purchase message; otherwise <see langword="null"/>.
        /// </returns>
        MessageRoleSubscriptionData RoleSubscriptionData { get; }

        /// <summary>
        ///     Gets the purchase notification for this message.
        /// </summary>
        PurchaseNotification PurchaseNotification { get; }
        
        /// <summary>
        ///     Gets the call data of the message.
        /// </summary>
        MessageCallData? CallData { get; }

        /// <summary>
        ///     Adds a reaction to this message.
        /// </summary>
        /// <example>
        ///     <para>The following example adds the reaction, <c>💕</c>, to the message.</para>
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
        ///     <para>The following example removes the reaction, <c>💕</c>, added by the message author from the message.</para>
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
        ///     <para>The following example removes the reaction, <c>💕</c>, added by the user with ID 84291986575613952 from the message.</para>
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
        ///     Removes all reactions with a specific emoji from this message.
        /// </summary>
        /// <param name="emote">The emoji used to react to this message.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <returns>
        ///     A task that represents the asynchronous removal operation.
        /// </returns>
        Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions options = null);

        /// <summary>
        ///     Gets all users that reacted to a message with a given emote.
        /// </summary>
        /// <remarks>
        ///     <note type="important">
        ///         The returned collection is an asynchronous enumerable object; one must call 
        ///         <see cref="AsyncEnumerableExtensions.FlattenAsync{T}"/> to access the users as a
        ///         collection.
        ///     </note>
        ///     <note type="warning">
        ///         Do not fetch too many users at once! This may cause unwanted preemptive rate limit or even actual
        ///         rate limit, causing your bot to freeze!
        ///     </note>
        ///     This method will attempt to fetch the number of reactions specified under <paramref name="limit"/>. 
        ///     The library will attempt to split up the requests according to your <paramref name="limit"/> and 
        ///     <see cref="DiscordConfig.MaxUserReactionsPerBatch"/>. In other words, should the user request 500 reactions,
        ///     and the <see cref="Discord.DiscordConfig.MaxUserReactionsPerBatch"/> constant is <c>100</c>, the request will
        ///     be split into 5 individual requests; thus returning 5 individual asynchronous responses, hence the need
        ///     of flattening.
        /// </remarks>
        /// <example>
        ///     <para>The following example gets the users that have reacted with the emoji <c>💕</c> to the message.</para>
        ///     <code language="cs">
        ///     var emoji = new Emoji("\U0001f495");
        ///     var reactedUsers = await message.GetReactionUsersAsync(emoji, 100).FlattenAsync();
        ///     </code>
        /// </example>
        /// <param name="emoji">The emoji that represents the reaction that you wish to get.</param>
        /// <param name="limit">The number of users to request.</param>
        /// <param name="options">The options to be used when sending the request.</param>
        /// <param name="type">The type of the reaction you wish to get users for.</param>
        /// <returns>
        ///      Paged collection of users.
        /// </returns>
        IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null,
            ReactionType type = ReactionType.Normal);
    }
}
