using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.IMessageModel;

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
        private ImmutableArray<Attachment> _attachments = ImmutableArray.Create<Attachment>();
        private ImmutableArray<Embed> _embeds = ImmutableArray.Create<Embed>();
        private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();
        private ulong[] _roleMentions;
        private ulong? _referencedMessageId;
        //private ImmutableArray<SocketRole> _roleMentions = ImmutableArray.Create<SocketRole>();
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

        internal SocketUserMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal new static SocketUserMessage Create(DiscordSocketClient discord, ClientStateManager state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            var entity = new SocketUserMessage(discord, model.Id, channel, author, MessageHelper.GetSource(model));
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            SocketGuild guild = (Channel as SocketGuildChannel)?.Guild;

            _isTTS = model.IsTextToSpeech;
            _isPinned = model.Pinned;
            _editedTimestampTicks = model.EditedTimestamp;
            _isMentioningEveryone = model.MentionEveryone;
            _roleMentions = model.RoleMentionIds;
            _referencedMessageId = model.ReferenceMessageId;

            if (model.Attachments != null && model.Attachments.Length > 0)
            {
                var value = model.Attachments;
                if (value.Length > 0)
                {
                    var attachments = ImmutableArray.CreateBuilder<Attachment>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        attachments.Add(Attachment.Create(value[i]));
                    _attachments = attachments.ToImmutable();
                }
                else
                    _attachments = ImmutableArray.Create<Attachment>();
            }

            if (model.Embeds != null && model.Embeds.Length > 0)
            {
                var value = model.Embeds;
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

            if (model.Content != null)
            {
                var text = model.Content;
                _tags = MessageHelper.ParseTags(text, Channel, guild, MentionedUsers);
                model.Content = text;
            }

            if (model.Stickers != null && model.Stickers.Length > 0)
            {
                var value = model.Stickers;
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
        public async Task CrosspostAsync(RequestOptions options = null)
        {
            if (!(Channel is INewsChannel))
            {
                throw new InvalidOperationException("Publishing (crossposting) is only valid in news channels.");
            }

            await MessageHelper.CrosspostAsync(this, Discord, options);
        }

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
        internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;
    }
}
