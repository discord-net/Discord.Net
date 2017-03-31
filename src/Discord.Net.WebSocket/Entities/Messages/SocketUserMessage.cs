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
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUserMessage : SocketMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private ImmutableArray<Attachment> _attachments;
        private ImmutableArray<Embed> _embeds;
        private ImmutableArray<ITag> _tags;
        private List<SocketReaction> _reactions = new List<SocketReaction>();
        
        public override bool IsTTS => _isTTS;
        public override bool IsPinned => _isPinned;
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);
        public override IReadOnlyCollection<Attachment> Attachments => _attachments;
        public override IReadOnlyCollection<Embed> Embeds => _embeds;
        public override IReadOnlyCollection<ITag> Tags => _tags;
        public override IReadOnlyCollection<SocketGuildChannel> MentionedChannels => MessageHelper.FilterTagsByValue<SocketGuildChannel>(TagType.ChannelMention, _tags);
        public override IReadOnlyCollection<SocketRole> MentionedRoles => MessageHelper.FilterTagsByValue<SocketRole>(TagType.RoleMention, _tags);
        public override IReadOnlyCollection<SocketUser> MentionedUsers => MessageHelper.FilterTagsByValue<SocketUser>(TagType.UserMention, _tags);
        public IReadOnlyDictionary<Emoji, ReactionMetadata> Reactions => _reactions.GroupBy(r => r.Emoji).ToDictionary(x => x.Key, x => new ReactionMetadata { ReactionCount = x.Count(), IsMe = x.Any(y => y.UserId == Discord.CurrentUser.Id) });

        internal SocketUserMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id, channel, author, source)
        {
        }
        internal static new SocketUserMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            var entity = new SocketUserMessage(discord, model.Id, channel, author, MessageHelper.GetSource(model));
            entity.Update(state, model);
            return entity;
        }

        internal override void Update(ClientState state, Model model)
        {
            base.Update(state, model);

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

            IReadOnlyCollection<IUser> mentions = ImmutableArray.Create<SocketUnknownUser>(); //Is passed to ParseTags to get real mention collection
            if (model.UserMentions.IsSpecified)
            {
                var value = model.UserMentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<SocketUnknownUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                    {
                        var val = value[i];
                        if (val.Object != null)
                            newMentions.Add(SocketUnknownUser.Create(Discord, state, val.Object));
                    }
                    mentions = newMentions.ToImmutable();
                }
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                var guild = (Channel as SocketGuildChannel)?.Guild;
                _tags = MessageHelper.ParseTags(text, Channel, guild, mentions);
                model.Content = text;
            }
        }
        internal void AddReaction(SocketReaction reaction)
        {
            _reactions.Add(reaction);
        }
        internal void RemoveReaction(SocketReaction reaction)
        {
            if (_reactions.Contains(reaction))
                _reactions.Remove(reaction);
        }
        internal void ClearReactions()
        {
            _reactions.Clear();
        }

        public Task ModifyAsync(Action<MessageProperties> func, RequestOptions options = null)
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
        internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;
    }
}
