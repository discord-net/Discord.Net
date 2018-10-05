using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based message.
    /// </summary>
    public abstract class SocketMessage : SocketEntity<ulong>, IMessage
    {
        private long _timestampTicks;

        /// <summary>
        ///     Gets the author of this message.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based user object.
        /// </returns>
        public SocketUser Author { get; }
        /// <summary>
        ///     Gets the source channel of the message.
        /// </summary>
        /// <returns>
        ///     A WebSocket-based message channel.
        /// </returns>
        public ISocketMessageChannel Channel { get; }
        /// <inheritdoc />
        public MessageSource Source { get; }

        /// <inheritdoc />
        public string Content { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public virtual bool IsTTS => false;
        /// <inheritdoc />
        public virtual bool IsPinned => false;
        /// <inheritdoc />
        public virtual DateTimeOffset? EditedTimestamp => null;

        /// <inheritdoc />
        public MessageActivity Activity { get; private set; }

        /// <inheritdoc />
        public MessageApplication Application { get; private set; }

        /// <summary>
        ///     Returns all attachments included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of attachments.
        /// </returns>
        public virtual IReadOnlyCollection<Attachment> Attachments => ImmutableArray.Create<Attachment>();
        /// <summary>
        ///     Returns all embeds included in this message.
        /// </summary>
        /// <returns>
        ///     Collection of embed objects.
        /// </returns>
        public virtual IReadOnlyCollection<Embed> Embeds => ImmutableArray.Create<Embed>();
        /// <summary>
        ///     Returns the channels mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based guild channels.
        /// </returns>
        public virtual IReadOnlyCollection<SocketGuildChannel> MentionedChannels => ImmutableArray.Create<SocketGuildChannel>();
        /// <summary>
        ///     Returns the roles mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based roles.
        /// </returns>
        public virtual IReadOnlyCollection<SocketRole> MentionedRoles => ImmutableArray.Create<SocketRole>();
        /// <summary>
        ///     Returns the users mentioned in this message.
        /// </summary>
        /// <returns>
        ///     Collection of WebSocket-based users.
        /// </returns>
        public virtual IReadOnlyCollection<SocketUser> MentionedUsers => ImmutableArray.Create<SocketUser>();
        /// <inheritdoc />
        public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();

        /// <inheritdoc />
        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal SocketMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }
        internal static SocketMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            if (model.Type == MessageType.Default)
                return SocketUserMessage.Create(discord, state, author, channel, model);
            else
                return SocketSystemMessage.Create(discord, state, author, channel, model);
        }
        internal virtual void Update(ClientState state, Model model)
        {
            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
                Content = model.Content.Value;

            if (model.Application.IsSpecified)
            {
                // create a new Application from the API model
                Application = new MessageApplication()
                {
                    Id = model.Application.Value.Id,
                    CoverImage = model.Application.Value.CoverImage,
                    Description = model.Application.Value.Description,
                    Icon = model.Application.Value.Icon,
                    Name = model.Application.Value.Name
                };
            }

            if (model.Activity.IsSpecified)
            {
                // create a new Activity from the API model
                Activity = new MessageActivity()
                {
                    Type = model.Activity.Value.Type.Value,
                    PartyId = model.Activity.Value.PartyId.Value
                };
            }
        }

        /// <inheritdoc />
        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        /// <summary>
        ///     Gets the content of the message.
        /// </summary>
        /// <returns>
        ///     Content of the message.
        /// </returns>
        public override string ToString() => Content;
        internal SocketMessage Clone() => MemberwiseClone() as SocketMessage;

        //IMessage
        /// <inheritdoc />
        IUser IMessage.Author => Author;
        /// <inheritdoc />
        IMessageChannel IMessage.Channel => Channel;
        /// <inheritdoc />
        MessageType IMessage.Type => MessageType.Default;
        /// <inheritdoc />
        IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;
        /// <inheritdoc />
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedChannelIds => MentionedChannels.Select(x => x.Id).ToImmutableArray();
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedRoleIds => MentionedRoles.Select(x => x.Id).ToImmutableArray();
        /// <inheritdoc />
        IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();
    }
}
