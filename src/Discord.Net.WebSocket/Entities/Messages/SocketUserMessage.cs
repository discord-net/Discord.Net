using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents a WebSocket-based message sent by a user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUserMessage : SocketMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private IUserMessage _referencedMessage;
        private ImmutableArray<Attachment> _attachments = ImmutableArray.Create<Attachment>();
        private ImmutableArray<Embed> _embeds = ImmutableArray.Create<Embed>();
        private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();
        private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();
        private ImmutableArray<SocketSticker> _stickers = ImmutableArray.Create<SocketSticker>();

        /// <inheritdoc />
        public override bool IsTTS => _isTTS;
        /// <inheritdoc />
        public override bool IsPinned => _isPinned;
        /// <inheritdoc />
        public override bool IsSuppressed => Flags.HasValue && Flags.Value.HasFlag(MessageFlags.SuppressEmbeds);
        /// <inheritdoc />
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        /// <inheritdoc />
        public override bool MentionedEveryone => _isMentioningEveryone;
        /// <inheritdoc />
        public override IReadOnlyCollection<Attachment> Attachments => _attachments;
        /// <inheritdoc />
        public override IReadOnlyCollection<Embed> Embeds => _embeds;
        /// <inheritdoc />
        public override IReadOnlyCollection<ITag> Tags => _tags;
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketGuildChannel> MentionedChannels => MessageHelper.FilterTagsByValue<SocketGuildChannel>(TagType.ChannelMention, _tags);
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketRole> MentionedRoles => _roleMentions;
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketSticker> Stickers => _stickers;
        /// <inheritdoc />
        public IUserMessage ReferencedMessage => _referencedMessage;

        /// <inheritdoc />
        public IMessageInteractionMetadata InteractionMetadata { get; internal set; }

        /// <inheritdoc />
        public Poll? Poll { get; internal set; }

        /// <inheritdoc />
        public MessageResolvedData ResolvedData { get; internal set; }

        internal SocketUserMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal new static SocketUserMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            var entity = new SocketUserMessage(discord, model.Id, channel, author, MessageHelper.GetSource(model));
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;

            if (model.IsTextToSpeech.IsSpecified)
                _isTTS = model.IsTextToSpeech.Value;
            if (model.Pinned.IsSpecified)
                _isPinned = model.Pinned.Value;
            if (model.EditedTimestamp.IsSpecified)
                _editedTimestampTicks = model.EditedTimestamp.Value?.UtcTicks;
            if (model.MentionEveryone.IsSpecified)
                _isMentioningEveryone = model.MentionEveryone.Value;
            if (model.RoleMentions.IsSpecified)
                _roleMentions = model.RoleMentions.Value.Select(x => guild.GetRole(x)).ToImmutableArray();

            if (model.Attachments.IsSpecified)
            {
                var value = model.Attachments.Value;
                if (value.Length > 0)
                {
                    var attachments = ImmutableArray.CreateBuilder<Attachment>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        attachments.Add(Attachment.Create(value[i], Discord));
                    _attachments = attachments.ToImmutable();
                }
                else
                    _attachments = ImmutableArray.Create<Attachment>();
            }

            if (model.Embeds.IsSpecified)
            {
                var value = model.Embeds.Value;
                if (value.Length > 0)
                {
                    var embeds = ImmutableArray.CreateBuilder<Embed>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        embeds.Add(value[i].ToEntity());
                    _embeds = embeds.ToImmutable();
                }
                else
                    _embeds = ImmutableArray.Create<Embed>();
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                _tags = MessageHelper.ParseTags(text, Channel, guild, MentionedUsers);
                model.Content = text;
            }

            if (model.ReferencedMessage.IsSpecified && model.ReferencedMessage.Value != null)
            {
                var refMsg = model.ReferencedMessage.Value;
                ulong? webhookId = refMsg.WebhookId.ToNullable();
                SocketUser refMsgAuthor = null;
                if (refMsg.Author.IsSpecified)
                {
                    if (guild != null)
                    {
                        if (webhookId != null)
                            refMsgAuthor = SocketWebhookUser.Create(guild, state, refMsg.Author.Value, webhookId.Value);
                        else
                            refMsgAuthor = guild.GetUser(refMsg.Author.Value.Id);
                    }
                    else
                        refMsgAuthor = (Channel as SocketChannel)?.GetUser(refMsg.Author.Value.Id);
                    if (refMsgAuthor == null)
                        refMsgAuthor = SocketUnknownUser.Create(Discord, state, refMsg.Author.Value);
                }
                else
                    // Message author wasn't specified in the payload, so create a completely anonymous unknown user
                    refMsgAuthor = new SocketUnknownUser(Discord, id: 0);
                _referencedMessage = SocketUserMessage.Create(Discord, state, refMsgAuthor, Channel, refMsg);
            }

            if (model.StickerItems.IsSpecified)
            {
                var value = model.StickerItems.Value;
                if (value.Length > 0)
                {
                    var stickers = ImmutableArray.CreateBuilder<SocketSticker>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var stickerItem = value[i];
                        SocketSticker sticker = null;

                        if (guild != null)
                            sticker = guild.GetSticker(stickerItem.Id);

                        if (sticker == null)
                            sticker = Discord.GetSticker(stickerItem.Id);

                        // if they want to auto resolve
                        if (Discord.AlwaysResolveStickers)
                        {
                            sticker = Task.Run(async () => await Discord.GetStickerAsync(stickerItem.Id).ConfigureAwait(false)).GetAwaiter().GetResult();
                        }

                        // if its still null, create an unknown
                        if (sticker == null)
                            sticker = SocketUnknownSticker.Create(Discord, stickerItem);

                        stickers.Add(sticker);
                    }

                    _stickers = stickers.ToImmutable();
                }
                else
                    _stickers = ImmutableArray.Create<SocketSticker>();
            }

            if (model.Resolved.IsSpecified)
            {
                var users = model.Resolved.Value.Users.IsSpecified
                    ? model.Resolved.Value.Users.Value.Select(x => RestUser.Create(Discord, x.Value)).ToImmutableArray()
                    : ImmutableArray<RestUser>.Empty;

                var members = model.Resolved.Value.Members.IsSpecified
                    ? model.Resolved.Value.Members.Value.Select(x =>
                    {
                        x.Value.User = model.Resolved.Value.Users.Value.TryGetValue(x.Key, out var user)
                            ? user
                            : null;

                        return RestGuildUser.Create(Discord, guild, x.Value);
                    }).ToImmutableArray()
                    : ImmutableArray<RestGuildUser>.Empty;

                var roles = model.Resolved.Value.Roles.IsSpecified
                    ? model.Resolved.Value.Roles.Value.Select(x => RestRole.Create(Discord, guild, x.Value)).ToImmutableArray()
                    : ImmutableArray<RestRole>.Empty;

                var channels = model.Resolved.Value.Channels.IsSpecified
                    ? model.Resolved.Value.Channels.Value.Select(x => RestChannel.Create(Discord, x.Value, guild)).ToImmutableArray()
                    : ImmutableArray<RestChannel>.Empty;

                ResolvedData = new MessageResolvedData(users, members, roles, channels);
            }

            if (model.InteractionMetadata.IsSpecified)
                InteractionMetadata = model.InteractionMetadata.Value.ToInteractionMetadata();

            if (model.Poll.IsSpecified)
                Poll = model.Poll.Value.ToEntity();
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">Only the author of a message may modify the message.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Message content is too long, length must be less or equal to <see cref="DiscordConfig.MaxMessageSize"/>.</exception>
        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
            => MessageHelper.ModifyAsync(this, Discord, func, options);

        /// <inheritdoc />
        public Task PinAsync(RequestOptions options = null)
            => MessageHelper.PinAsync(this, Discord, options);
        /// <inheritdoc />
        public Task UnpinAsync(RequestOptions options = null)
            => MessageHelper.UnpinAsync(this, Discord, options);

        public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, startIndex, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);
        /// <inheritdoc />
        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, 0, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">This operation may only be called on a <see cref="INewsChannel"/> channel.</exception>
        public Task CrosspostAsync(RequestOptions options = null)
        {
            if (!(Channel is INewsChannel))
            {
                throw new InvalidOperationException("Publishing (crossposting) is only valid in news channels.");
            }

            return MessageHelper.CrosspostAsync(this, Discord, options);
        }

        /// <inheritdoc />
        public Task EndPollAsync(RequestOptions options = null)
            => MessageHelper.EndPollAsync(Channel.Id, Id, Discord, options);

        /// <inheritdoc />
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetPollAnswerVotersAsync(uint answerId, int? limit = null, ulong? afterId = null,
            RequestOptions options = null)
            => MessageHelper.GetPollAnswerVotersAsync(Channel.Id, Id, afterId, answerId, limit, Discord, options);

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
        internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;
    }
}
