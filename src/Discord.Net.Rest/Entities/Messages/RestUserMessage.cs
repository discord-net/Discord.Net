using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUserMessage : RestMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private ImmutableArray<Attachment> _attachments;
        private ImmutableArray<Embed> _embeds;
        private ImmutableArray<ITag> _tags;
        private ImmutableArray<RestReaction> _reactions;
        
        public override bool IsTTS => _isTTS;
        public override bool IsPinned => _isPinned;
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        public override IReadOnlyCollection<Attachment> Attachments => _attachments;
        public override IReadOnlyCollection<Embed> Embeds => _embeds;
        public override IReadOnlyCollection<ulong> MentionedChannelIds => MessageHelper.FilterTagsByKey(TagType.ChannelMention, _tags);
        public override IReadOnlyCollection<ulong> MentionedRoleIds => MessageHelper.FilterTagsByKey(TagType.RoleMention, _tags);
        public override IReadOnlyCollection<RestUser> MentionedUsers => MessageHelper.FilterTagsByValue<RestUser>(TagType.UserMention, _tags);
        public override IReadOnlyCollection<ITag> Tags => _tags;
        public IReadOnlyDictionary<Emoji, ReactionMetadata> Reactions => _reactions.ToDictionary(x => x.Emoji, x => new ReactionMetadata { ReactionCount = x.Count, IsMe = x.Me });

        internal RestUserMessage(BaseDiscordClient discord, ulong id, IMessageChannel channel, IUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal static new RestUserMessage Create(BaseDiscordClient discord, IMessageChannel channel, IUser author, Model model)
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

            if (model.Reactions.IsSpecified)
            {
                var value = model.Reactions.Value;
                if (value.Length > 0)
                {
                    var reactions = ImmutableArray.CreateBuilder<RestReaction>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        reactions.Add(RestReaction.Create(value[i]));
                    _reactions = reactions.ToImmutable();
                }
                else
                    _reactions = ImmutableArray.Create<RestReaction>();
            }
            else
                _reactions = ImmutableArray.Create<RestReaction>();

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                var guildId = (Channel as IGuildChannel)?.GuildId;
                var guild = guildId != null ? (Discord as IDiscordClient).GetGuildAsync(guildId.Value, CacheMode.CacheOnly).Result : null;
                _tags = MessageHelper.ParseTags(text, null, guild, mentions);
                model.Content = text;
            }
        }

        public async Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
        {
            var model = await MessageHelper.ModifyAsync(this, Discord, func, options).ConfigureAwait(false);
            Update(model);
        }

        public Task AddReactionAsync(Emoji emoji, RequestOptions options = null)
            => MessageHelper.AddReactionAsync(this, emoji, Discord, options);
        public Task AddReactionAsync(string emoji, RequestOptions options = null)
            => MessageHelper.AddReactionAsync(this, emoji, Discord, options);

        public Task RemoveReactionAsync(Emoji emoji, IUser user, RequestOptions options = null)
            => MessageHelper.RemoveReactionAsync(this, user, emoji, Discord, options);
        public Task RemoveReactionAsync(string emoji, IUser user, RequestOptions options = null)
            => MessageHelper.RemoveReactionAsync(this, user, emoji, Discord, options);

        public Task RemoveAllReactionsAsync(RequestOptions options = null)
            => MessageHelper.RemoveAllReactionsAsync(this, Discord, options);
        
        public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(string emoji, int limit = 100, ulong? afterUserId = null, RequestOptions options = null)
            => MessageHelper.GetReactionUsersAsync(this, emoji, x => { x.Limit = limit; x.AfterUserId = afterUserId.HasValue ? afterUserId.Value : Optional.Create<ulong>(); }, Discord, options);
        

        public Task PinAsync(RequestOptions options = null)
            => MessageHelper.PinAsync(this, Discord, options);
        public Task UnpinAsync(RequestOptions options = null)
            => MessageHelper.UnpinAsync(this, Discord, options);

        public string Resolve(int startIndex, TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, startIndex, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);
        public string Resolve(TagHandling userHandling = TagHandling.Name, TagHandling channelHandling = TagHandling.Name,
            TagHandling roleHandling = TagHandling.Name, TagHandling everyoneHandling = TagHandling.Ignore, TagHandling emojiHandling = TagHandling.Name)
            => MentionUtils.Resolve(this, 0, userHandling, channelHandling, roleHandling, everyoneHandling, emojiHandling);

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
    }
}
