using Model = Discord.API.Gateway.Reaction;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based reaction object.
    /// </summary>
    public class SocketReaction : IReaction
    {
        /// <summary>
        ///     Gets the ID of the user who added the reaction.
        /// </summary>
        /// <remarks>
        ///     This property retrieves the snowflake identifier of the user responsible for this reaction. This
        ///     property will always contain the user identifier in event that
        ///     <see cref="Discord.WebSocket.SocketReaction.User" /> cannot be retrieved.
        /// </remarks>
        /// <returns>
        ///     A user snowflake identifier associated with the user.
        /// </returns>
        public ulong UserId { get; }
        /// <summary>
        ///     Gets the user who added the reaction if possible.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This property attempts to retrieve a WebSocket-cached user that is responsible for this reaction from
        ///         the client. In other words, when the user is not in the WebSocket cache, this property may not
        ///         contain a value, leaving the only identifiable information to be
        ///         <see cref="Discord.WebSocket.SocketReaction.UserId" />.
        ///     </para>
        ///     <para>
        ///         If you wish to obtain an identifiable user object, consider utilizing
        ///         <see cref="Discord.Rest.DiscordRestClient" /> which will attempt to retrieve the user from REST.
        ///     </para>
        /// </remarks>
        /// <returns>
        ///     A user object where possible; a value is not always returned.
        /// </returns>
        /// <seealso cref="Optional{T}"/>
        public Optional<IUser> User { get; }
        /// <summary>
        ///     Gets the ID of the message that has been reacted to.
        /// </summary>
        /// <returns>
        ///     A message snowflake identifier associated with the message.
        /// </returns>
        public ulong MessageId { get; }
        /// <summary>
        ///     Gets the message that has been reacted to if possible.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based message where possible; a value is not always returned.
        /// </returns>
        /// <seealso cref="Optional{T}"/>
        public Optional<SocketUserMessage> Message { get; }
        /// <summary>
        ///     Gets the channel where the reaction takes place in.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based message channel.
        /// </returns>
        public ISocketMessageChannel Channel { get; }
        /// <inheritdoc />
        public IEmote Emote { get; }

        internal SocketReaction(ISocketMessageChannel channel, ulong messageId, Optional<SocketUserMessage> message, ulong userId, Optional<IUser> user, IEmote emoji)
        {
            Channel = channel;
            MessageId = messageId;
            Message = message;
            UserId = userId;
            User = user;
            Emote = emoji;
        }
        internal static SocketReaction Create(Model model, ISocketMessageChannel channel, Optional<SocketUserMessage> message, Optional<IUser> user)
        {
            IEmote emote;
            if (model.Emoji.Id.HasValue)
                emote = new Emote(model.Emoji.Id.Value, model.Emoji.Name, model.Emoji.Animated.GetValueOrDefault());
            else
                emote = new Emoji(model.Emoji.Name);
            return new SocketReaction(channel, model.MessageId, message, model.UserId, user, emote);
        }

        /// <inheritdoc />
        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            if (other == this)
                return true;

            var otherReaction = other as SocketReaction;
            if (otherReaction == null)
                return false;

            return UserId == otherReaction.UserId && MessageId == otherReaction.MessageId && Emote.Equals(otherReaction.Emote);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = UserId.GetHashCode();
                hashCode = (hashCode * 397) ^ MessageId.GetHashCode();
                hashCode = (hashCode * 397) ^ Emote.GetHashCode();
                return hashCode;
            }
        }
    }
}
