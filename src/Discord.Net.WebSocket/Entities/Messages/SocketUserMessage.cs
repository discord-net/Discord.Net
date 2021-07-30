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
        private ImmutableArray<ulong> _roleIdMentions = ImmutableArray.Create<ulong>();
        private ImmutableArray<SocketUser> _userMentions = ImmutableArray.Create<SocketUser>();
        private ImmutableArray<Sticker> _stickers = ImmutableArray.Create<Sticker>();

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
        public override IReadOnlyCollection<ulong> MentionedRoleIds => _roleIdMentions;
        /// <inheritdoc />
        public override IReadOnlyCollection<SocketUser> MentionedUsers => _userMentions;
        /// <inheritdoc />
        public override IReadOnlyCollection<Sticker> Stickers => _stickers;
        /// <inheritdoc />
        public IUserMessage ReferencedMessage => _referencedMessage;

        internal SocketUserMessage(DiscordSocketClient discord, ulong id, Cacheable<IMessageChannel, ulong> channel, SocketUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal static SocketUserMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, Cacheable<IMessageChannel, ulong> channel, Model model)
        {
            var entity = new SocketUserMessage(discord, model.Id, channel, author, MessageHelper.GetSource(model));
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

            SocketGuild guild = null;
            if (model.GuildId.IsSpecified)
                guild = state.GetGuild(model.GuildId.Value);
            else if (Channel.HasValue)
                guild = (Channel.Value as SocketGuildChannel)?.Guild;

            if (model.IsTextToSpeech.IsSpecified)
                _isTTS = model.IsTextToSpeech.Value;
            if (model.Pinned.IsSpecified)
                _isPinned = model.Pinned.Value;
            if (model.EditedTimestamp.IsSpecified)
                _editedTimestampTicks = model.EditedTimestamp.Value?.UtcTicks;
            if (model.MentionEveryone.IsSpecified)
                _isMentioningEveryone = model.MentionEveryone.Value;
            if (model.RoleMentions.IsSpecified)
                _roleIdMentions = model.RoleMentions.Value.ToImmutableArray();

            if (model.Attachments.IsSpecified)
            {
                var value = model.Attachments.Value;
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

            if (model.UserMentions.IsSpecified)
            {
                var value = model.UserMentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<SocketUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var val = value[i];
                        if (val.Object != null)
                        {
                            SocketUser user = null;
                            if (Channel.HasValue)
                                user = Channel.Value.GetUserAsync(val.Object.Id, CacheMode.CacheOnly).GetAwaiter().GetResult() as SocketUser;
                            if (user != null)
                                newMentions.Add(user);
                            else
                                newMentions.Add(SocketUnknownUser.Create(Discord, state, val.Object));
                        }
                    }
                    _userMentions = newMentions.ToImmutable();
                }
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                _tags = MessageHelper.ParseTags(text, Channel.HasValue ? Channel.Value : null, guild, _userMentions);
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
                    else if (Channel.HasValue)
                        refMsgAuthor = (Channel.Value as SocketChannel).GetUser(refMsg.Author.Value.Id);

                    if (refMsgAuthor == null)
                        refMsgAuthor = SocketUnknownUser.Create(Discord, state, refMsg.Author.Value);
                }
                else
                    // Message author wasn't specified in the payload, so create a completely anonymous unknown user
                    refMsgAuthor = new SocketUnknownUser(Discord, id: 0);
                _referencedMessage = SocketUserMessage.Create(Discord, state, refMsgAuthor, Channel, refMsg);
            }

            if (model.Stickers.IsSpecified)
            {
                var value = model.Stickers.Value;
                if (value.Length > 0)
                {
                    var stickers = ImmutableArray.CreateBuilder<Sticker>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        stickers.Add(Sticker.Create(value[i]));
                    _stickers = stickers.ToImmutable();
                }
                else
                    _stickers = ImmutableArray.Create<Sticker>();
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
            if (Channel.HasValue && !(Channel.Value is INewsChannel))
            {
                throw new InvalidOperationException("Publishing (crossposting) is only valid in news channels.");
            }

            await MessageHelper.CrosspostAsync(this, Discord, options);
        }

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
        internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;
    }
}
