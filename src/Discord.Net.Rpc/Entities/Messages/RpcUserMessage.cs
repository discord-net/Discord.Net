using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Rpc.Message;

namespace Discord.Rpc
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RpcUserMessage : RpcMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned, _isBlocked;
        private long? _editedTimestampTicks;
        private ulong? _webhookId;
        private ImmutableArray<Attachment> _attachments;
        private ImmutableArray<Embed> _embeds;
        private ImmutableArray<ITag> _tags;        

        public override bool IsTTS => _isTTS;
        public override bool IsPinned => _isPinned;
        public override bool IsBlocked => _isBlocked;
        public override ulong? WebhookId => _webhookId;
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        public override IReadOnlyCollection<Attachment> Attachments => _attachments;
        public override IReadOnlyCollection<Embed> Embeds => _embeds;
        public override IReadOnlyCollection<ulong> MentionedChannelIds => MessageHelper.FilterTagsByKey(TagType.ChannelMention, _tags);
        public override IReadOnlyCollection<ulong> MentionedRoleIds => MessageHelper.FilterTagsByKey(TagType.RoleMention, _tags);
        public override IReadOnlyCollection<ulong> MentionedUserIds => MessageHelper.FilterTagsByKey(TagType.UserMention, _tags);
        public override IReadOnlyCollection<ITag> Tags => _tags;
        public IReadOnlyDictionary<Emoji, ReactionMetadata> Reactions => ImmutableDictionary.Create<Emoji, ReactionMetadata>();

        internal RpcUserMessage(DiscordRpcClient discord, ulong id, RestVirtualMessageChannel channel, RpcUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal new static RpcUserMessage Create(DiscordRpcClient discord, ulong channelId, Model model)
        {
            var entity = new RpcUserMessage(discord, model.Id, 
                RestVirtualMessageChannel.Create(discord, channelId), 
                RpcUser.Create(discord, model.Author.Value, model.WebhookId.ToNullable()),
                MessageHelper.GetSource(model));
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
            if (model.WebhookId.IsSpecified)
                _webhookId = model.WebhookId.Value;

            if (model.IsBlocked.IsSpecified)
                _isBlocked = model.IsBlocked.Value;

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

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                _tags = MessageHelper.ParseTags(text, null, null, ImmutableArray.Create<IUser>());
                model.Content = text;
            }
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options)
            => MessageHelper.ModifyAsync(this, Discord, func, options);

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
        
        public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(string emoji, int limit, ulong? afterUserId, RequestOptions options = null)
            => MessageHelper.GetReactionUsersAsync(this, emoji, x => { x.Limit = limit; x.AfterUserId = afterUserId.HasValue ? afterUserId.Value : Optional.Create<ulong>(); }, Discord, options);

        public Task PinAsync(RequestOptions options)
            => MessageHelper.PinAsync(this, Discord, options);
        public Task UnpinAsync(RequestOptions options)
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
