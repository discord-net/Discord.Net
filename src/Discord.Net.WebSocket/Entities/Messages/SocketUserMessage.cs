using Discord.API.Rest;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SocketUserMessage : SocketMessage, IUserMessage
    {
        private bool _isMentioningEveryone, _isTTS, _isPinned;
        private long? _editedTimestampTicks;
        private ImmutableArray<RestAttachment> _attachments;
        private ImmutableArray<RestEmbed> _embeds;
        private ImmutableArray<ulong> _mentionedChannelIds;
        private ImmutableArray<RestRole> _mentionedRoles;
        private ImmutableArray<RestUser> _mentionedUsers;

        public ulong? WebhookId { get; private set; }

        public override bool IsTTS => _isTTS;
        public override bool IsPinned => _isPinned;
        public override bool IsWebhook => WebhookId != null;
        public override DateTimeOffset? EditedTimestamp => DateTimeUtils.FromTicks(_editedTimestampTicks);

        public override IReadOnlyCollection<IAttachment> Attachments => _attachments;
        public override IReadOnlyCollection<IEmbed> Embeds => _embeds;
        public override IReadOnlyCollection<ulong> MentionedChannelIds => _mentionedChannelIds;
        public override IReadOnlyCollection<IRole> MentionedRoles => _mentionedRoles;
        public override IReadOnlyCollection<IUser> MentionedUsers => _mentionedUsers;

        internal SocketUserMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author)
            : base(discord, id, channel, author)
        {
        }
        internal new static SocketUserMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            var entity = new SocketUserMessage(discord, model.Id, channel, author);
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
            if (model.WebhookId.IsSpecified)
                WebhookId = model.WebhookId.Value;

            if (model.Attachments.IsSpecified)
            {
                var value = model.Attachments.Value;
                if (value.Length > 0)
                {
                    var attachments = ImmutableArray.CreateBuilder<RestAttachment>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        attachments.Add(RestAttachment.Create(value[i]));
                    _attachments = attachments.ToImmutable();
                }
                else
                    _attachments = ImmutableArray.Create<RestAttachment>();
            }

            if (model.Embeds.IsSpecified)
            {
                var value = model.Embeds.Value;
                if (value.Length > 0)
                {
                    var embeds = ImmutableArray.CreateBuilder<RestEmbed>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        embeds.Add(RestEmbed.Create(value[i]));
                    _embeds = embeds.ToImmutable();
                }
                else
                    _embeds = ImmutableArray.Create<RestEmbed>();
            }

            ImmutableArray<RestUser> mentions = ImmutableArray.Create<RestUser>();
            if (model.Mentions.IsSpecified)
            {
                var value = model.Mentions.Value;
                if (value.Length > 0)
                {
                    var newMentions = ImmutableArray.CreateBuilder<RestUser>(value.Length);
                    for (int i = 0; i < value.Length; i++)
                        newMentions.Add(RestUser.Create(Discord, value[i]));
                    mentions = newMentions.ToImmutable();
                }
            }

            if (model.Content.IsSpecified)
            {
                var text = model.Content.Value;
                var guild = (Channel as SocketGuildChannel)?.Guild;

                _mentionedUsers = MentionUtils.GetUserMentions(text, Channel, mentions);
                _mentionedChannelIds = MentionUtils.GetChannelMentions(text, guild);
                _mentionedRoles = MentionUtils.GetRoleMentions<RestRole>(text, guild);
                model.Content = text;
            }
        }

        public Task ModifyAsync(Action<ModifyMessageParams> func, RequestOptions options = null)
            => MessageHelper.ModifyAsync(this, Discord, func, options);
        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        public Task PinAsync(RequestOptions options = null)
            => MessageHelper.PinAsync(this, Discord, options);
        public Task UnpinAsync(RequestOptions options = null)
            => MessageHelper.UnpinAsync(this, Discord, options);

        public string Resolve(UserMentionHandling userHandling = UserMentionHandling.Name, ChannelMentionHandling channelHandling = ChannelMentionHandling.Name,
            RoleMentionHandling roleHandling = RoleMentionHandling.Name, EveryoneMentionHandling everyoneHandling = EveryoneMentionHandling.Ignore)
            => Resolve(Content, userHandling, channelHandling, roleHandling, everyoneHandling);
        public string Resolve(int startIndex, int length, UserMentionHandling userHandling = UserMentionHandling.Name, ChannelMentionHandling channelHandling = ChannelMentionHandling.Name,
            RoleMentionHandling roleHandling = RoleMentionHandling.Name, EveryoneMentionHandling everyoneHandling = EveryoneMentionHandling.Ignore)
            => Resolve(Content.Substring(startIndex, length), userHandling, channelHandling, roleHandling, everyoneHandling);
        public string Resolve(string text, UserMentionHandling userHandling, ChannelMentionHandling channelHandling,
            RoleMentionHandling roleHandling, EveryoneMentionHandling everyoneHandling)
        {
            text = MentionUtils.ResolveUserMentions(text, null, MentionedUsers, userHandling);
            text = MentionUtils.ResolveChannelMentions(text, null, channelHandling);
            text = MentionUtils.ResolveRoleMentions(text, MentionedRoles, roleHandling);
            text = MentionUtils.ResolveEveryoneMentions(text, everyoneHandling);
            return text;
        }

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}{(Attachments.Count > 0 ? $", {Attachments.Count} Attachments" : "")})";
        internal new SocketUserMessage Clone() => MemberwiseClone() as SocketUserMessage;
    }
}
