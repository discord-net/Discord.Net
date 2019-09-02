using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
        private bool _isMentioningEveryone, _isTTS, _isPinned, _isSuppressed;
        private long? _editedTimestampTicks;
        private ImmutableArray<Attachment> _attachments = ImmutableArray.Create<Attachment>();
        private ImmutableArray<Embed> _embeds = ImmutableArray.Create<Embed>();
        private ImmutableArray<ITag> _tags = ImmutableArray.Create<ITag>();

        /// <inheritdoc />
        public override bool IsTTS => _isTTS;
        /// <inheritdoc />
        public override bool IsPinned => _isPinned;
        /// <inheritdoc />
        public override bool IsSuppressed => _isSuppressed;
        /// <inheritdoc />
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        /// <inheritdoc />
        public override IReadOnlyCollection<Attachment> Attachments => _attachments;
        /// <inheritdoc />
        public override IReadOnlyCollection<Embed> Embeds => _embeds;
        /// <inheritdoc />
        public override IReadOnlyCollection<ulong> MentionedChannelIds => MessageHelper.FilterTagsByKey(TagType.ChannelMention, _tags);
        /// <inheritdoc />
        public override IReadOnlyCollection<ulong> MentionedRoleIds => MessageHelper.FilterTagsByKey(TagType.RoleMention, _tags);
        /// <inheritdoc />
        public override IReadOnlyCollection<RestUser> MentionedUsers => MessageHelper.FilterTagsByValue<RestUser>(TagType.UserMention, _tags);
        /// <inheritdoc />
        public override IReadOnlyCollection<ITag> Tags => _tags;

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
            if (model.Flags.IsSpecified)
            {
                _isSuppressed = model.Flags.Value.HasFlag(API.MessageFlags.Suppressed);
            }

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

            ImmutableArray<IUser> mentions = ImmutableArray.Create<IUser>();
            if (model.UserMentions.IsSpecified)
            {
                var value = model.UserMentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<IUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var val = value[i];
                        if (val.Object != null)
                            newMentions.Add(RestUser.Create(Discord, val.Object));
                    }
                    mentions = newMentions.ToImmutable();
                }
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                var guildId = (Channel as IGuildChannel)?.GuildId;
                var guild = guildId != null ? (Discord as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).Result : null;
                _tags = MessageHelper.ParseTags(text, null, guild, mentions);
                model.Content = text;
            }
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
        /// <inheritdoc />
        public Task ModifySuppressionAsync(bool suppressEmbeds, RequestOptions options = null)
            => MessageHelper.SuppressEmbedsAsync(this, Discord, suppressEmbeds, options);

        public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, startIndex, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);
        /// <inheritdoc />
        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, 0, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
    }
}
