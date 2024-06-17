using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface IMessage :
    ISnowflakeEntity,
    IMessageActor
{
    #region Properties

    /// <summary>
    ///     Gets the type of this message.
    /// </summary>
    MessageType Type { get; }

    /// <summary>
    ///     Gets the source type of this message.
    /// </summary>
    MessageSource Source { get; }

    /// <summary>
    ///     Gets the flags related to this message.
    /// </summary>
    /// <remarks>
    ///     This value is determined by bitwise OR-ing <see cref="MessageFlags" /> values together.
    /// </remarks>
    MessageFlags Flags { get; }

    /// <summary>
    ///     Gets the value that indicates whether or not this message's embeds are suppressed.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the embeds in this message have been suppressed (made invisible); otherwise
    ///     <see langword="false" />.
    /// </returns>
    bool IsSuppressed
        => Flags.HasFlag(MessageFlags.SuppressEmbeds);

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
    ///     <see langword="true" /> if this message was added to its channel's pinned messages; otherwise
    ///     <see langword="false" />.
    /// </returns>
    bool IsPinned { get; }

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
    ///     This will be empty if the privileged <see cref="GatewayIntents.MessageContent" /> is disabled.
    /// </remarks>
    /// <returns>
    ///     A string that contains the body of the message; note that this field may be empty if there is an embed.
    /// </returns>
    string? Content { get; }

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
    ///     Gets the author of this message.
    /// </summary>
    ILoadableEntity<ulong, IUser> Author { get; }

    /// <summary>
    ///     Gets the thread that was started from this message.
    /// </summary>
    ILoadableEntity<ulong, IThreadChannel>? Thread { get; }

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
    IReadOnlyCollection<Attachment> Attachments { get; }

    /// <summary>
    ///     Gets all embeds included in this message.
    /// </summary>
    /// <remarks>
    ///     This property gets a read-only collection of embeds associated with this message. Depending on the
    ///     message, a sent message may contain one or more embeds. This is usually true when multiple link previews
    ///     are generated; however, only one <see cref="EmbedType.Rich" /> <see cref="Embed" /> can be featured.
    /// </remarks>
    /// <returns>
    ///     A read-only collection of embed objects.
    /// </returns>
    IReadOnlyCollection<Embed> Embeds { get; }

    IDefinedLoadableEntityEnumerable<ulong, IChannel> MentionedChannels { get; }

    IDefinedLoadableEntityEnumerable<ulong, IRole> MentionedRoles { get; }

    IDefinedLoadableEntityEnumerable<ulong, IUser> MentionedUsers { get; }

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
    ///     The <see cref="IMessageComponent" />'s attached to this message
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
    ///     Gets the interaction this message is a response to.
    /// </summary>
    /// <returns>
    ///     A <see cref="IMessageInteraction" /> if the message is a response to an interaction; otherwise
    ///     <see langword="null" />.
    /// </returns>
    IMessageInteraction? Interaction { get; }

    /// <summary>
    ///     Gets the data of the role subscription purchase or renewal that prompted this
    ///     <see cref="MessageType.RoleSubscriptionPurchase" /> message.
    /// </summary>
    /// <returns>
    ///     A <see cref="MessageRoleSubscriptionData" /> if the message is a role subscription purchase message; otherwise
    ///     <see langword="null" />.
    /// </returns>
    MessageRoleSubscriptionData? RoleSubscriptionData { get; }

    #endregion
}
