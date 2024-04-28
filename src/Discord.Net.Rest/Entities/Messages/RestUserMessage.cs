using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a REST-based message sent by a user.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUserMessage : RestMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private IUserMessage _referencedMessage;
        private ImmutableArray<Attachment> _attachments = ImmutableArray.Create<Attachment>();
        private ImmutableArray<Embed> _embeds = ImmutableArray.Create<Embed>();
        private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();
        private ImmutableArray<ulong> _roleMentionIds = ImmutableArray.Create<ulong>();
        private ImmutableArray<StickerItem> _stickers = ImmutableArray.Create<StickerItem>();

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
        public override IReadOnlyCollection<ulong> MentionedChannelIds => MessageHelper.FilterTagsByKey(TagType.ChannelMention, _tags);
        /// <inheritdoc />
        public override IReadOnlyCollection<ulong> MentionedRoleIds => _roleMentionIds;
        /// <inheritdoc />
        public override IReadOnlyCollection<ITag> Tags => _tags;
        /// <inheritdoc />
        public override IReadOnlyCollection<StickerItem> Stickers => _stickers;
        /// <inheritdoc />
        public IUserMessage ReferencedMessage => _referencedMessage;

        /// <inheritdoc />
        public IMessageInteractionMetadata InteractionMetadata { get; internal set; }

        /// <inheritdoc />
        public MessageResolvedData ResolvedData { get; internal set; }

        /// <inheritdoc />
        public IReadOnlyCollection<MessageSnapshot> ForwardedMessages { get; internal set; }

        internal RestUserMessage(BaseDiscordClient discord, ulong id, IMessageChannel channel, IUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal new static RestUserMessage Create(BaseDiscordClient discord, IMessageChannel channel, IUser author, Model model)
        {
            var entity = new RestUserMessage(discord, model.Id, channel, author, MessageHelper.GetSource(model));
            entity.Update(model);
            return entity;
        }

        internal override void Update(Model model)
        {
            base.Update(model);

            if (model.IsTextToSpeech.IsSpecified)
                _isTTS = model.IsTextToSpeech.Value;
            if (model.Pinned.IsSpecified)
                _isPinned = model.Pinned.Value;
            if (model.EditedTimestamp.IsSpecified)
                _editedTimestampTicks = model.EditedTimestamp.Value?.UtcTicks;
            if (model.MentionEveryone.IsSpecified)
                _isMentioningEveryone = model.MentionEveryone.Value;
            if (model.RoleMentions.IsSpecified)
                _roleMentionIds = model.RoleMentions.Value.ToImmutableArray();

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

            var guildId = (Channel as IGuildChannel)?.GuildId;
            var guild = guildId != null ? (Discord as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).Result : null;
            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                _tags = MessageHelper.ParseTags(text, null, guild, MentionedUsers);
                model.Content = text;
            }

            if (model.ReferencedMessage.IsSpecified && model.ReferencedMessage.Value != null)
            {
                var refMsg = model.ReferencedMessage.Value;
                IUser refMsgAuthor = MessageHelper.GetAuthor(Discord, guild, refMsg.Author.Value, refMsg.WebhookId.ToNullable());
                _referencedMessage = RestUserMessage.Create(Discord, Channel, refMsgAuthor, refMsg);
            }

            if (model.StickerItems.IsSpecified)
            {
                var value = model.StickerItems.Value;
                if (value.Length > 0)
                {
                    var stickers = ImmutableArray.CreateBuilder<StickerItem>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        stickers.Add(new StickerItem(Discord, value[i]));
                    _stickers = stickers.ToImmutable();
                }
                else
                    _stickers = ImmutableArray.Create<StickerItem>();
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

                        return RestGuildUser.Create(Discord, guild, x.Value, guildId);
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

            if (model.MessageSnapshots.IsSpecified)
            {
                ForwardedMessages = model.MessageSnapshots.Value.Select(x =>
                    new MessageSnapshot(RestMessage.Create(Discord, null, null, x.Message),
                        x.GuildId.IsSpecified ? x.GuildId.Value : null)).ToImmutableArray();
            }
            else
                ForwardedMessages = ImmutableArray<MessageSnapshot>.Empty;
        }

        /// <inheritdoc />
        public async Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await MessageHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

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

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
    }
}
