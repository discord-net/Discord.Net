using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public abstract class SocketMessage : SocketCacheableEntity<ulong, IMessageModel>, IMessage
    {
        public MessageChannelCacheable Channel { get; }

        public UserCacheable Author { get; }

        public MessageType Type
            => _source.Type;

        public abstract MessageSource Source { get; }

        public bool IsTTS
            => _source.IsTTS;

        public bool IsPinned
            => _source.IsPinned;

        public virtual bool IsSuppressed
            => false;

        public bool MentionedEveryone
            => _source.MentionsEveryone;

        public string? Content
            => _source.Content;

        public string? CleanContent
            => _source.Content; // MessageHelper.SanitizeMessage(this); TODO

        public DateTimeOffset Timestamp
            => _source.Timestamp;

        public DateTimeOffset? EditedTimestamp
            => _source.EditedTimestamp;

        public IReadOnlyCollection<ulong> MentionedChannelIds
            => _source.MentionedChannels.ToReadOnlyCollection();

        public IReadOnlyCollection<ulong> MentionedRoleIds
            => _source.MentionedRoles.ToReadOnlyCollection();

        public IReadOnlyCollection<ulong> MentionedUserIds
            => _source.MentionedUsers.ToReadOnlyCollection();

        public MessageFlags? Flags
            => _source.Flags;

        public DateTimeOffset CreatedAt
            => SnowflakeUtils.FromSnowflake(Id);

        public MessageActivity? Activity { get; private set; }

        public MessageApplication? Application { get; private set; }

        public MessageReference? Reference { get; private set; }

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException(); // TODO

        public IReadOnlyCollection<IMessageComponent> Components => throw new NotImplementedException(); // TODO

        public IReadOnlyCollection<IStickerItem> Stickers => throw new NotImplementedException(); // TODO

        public IMessageInteraction Interaction => throw new NotImplementedException(); // TODO

        public IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException(); // TODO: model -> class

        public IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException(); // TODO: model -> class

        public IReadOnlyCollection<ITag> Tags => throw new NotImplementedException(); // TODO: model -> class

        private IMessageModel _source;

        public SocketMessage(DiscordSocketClient discord, ulong channelId, ulong id, IMessageModel model)
            : base(discord, id)
        {
            _source = model;
            Channel = new(
                channelId,
                discord,
                discord.State.Channels
                    .SourceSpecific(channelId)
                    .TransformChannel<SocketMessageChannel>()
            );

            Author = new(model.AuthorId, discord, discord.State.Users.SourceSpecific(model.AuthorId));

            UpdateStructures();
        }

        internal override IMessageModel GetModel()
            => _source;
        internal override void Update(IMessageModel model)
        {
            _source = model;
            UpdateStructures();
        }

        private void UpdateStructures()
        {
            var hasActivity = _source.MessageActivityId is not null || _source.MessageActivityType is not null;
            
            if (Activity is not null)
            {
                if(!hasActivity)
                {
                    Activity = null;
                }
                else
                {
                    if (Activity.PartyId != _source.MessageActivityId)
                        Activity.PartyId = _source.MessageActivityId;

                    if (Activity.Type != _source.MessageActivityType)
                        Activity.Type = _source.MessageActivityType.GetValueOrDefault(MessageActivityType.Join); // TODO: nullable?
                }
            }
            else if (Activity is null && hasActivity)
            {
                Activity = new MessageActivity()
                {
                    PartyId = _source.MessageActivityId,
                    Type = _source.MessageActivityType.GetValueOrDefault(MessageActivityType.Join)
                };
            }

            var hasApplication = _source.MessageAppId is not null
                || _source.MessageAppIcon is not null
                || _source.MessageAppCoverImage is not null
                || _source.MessageAppDescription is not null
                || _source.MessageAppName is not null;

            if(Application is not null)
            {
                if (!hasApplication)
                    Application = null;
                else
                {
                    if (Application.CoverImage != _source.MessageAppCoverImage)
                        Application.CoverImage = _source.MessageAppCoverImage;

                    if (Application.Description != _source.MessageAppDescription)
                        Application.Description = _source.MessageAppDescription;

                    if (Application.Icon != _source.MessageAppIcon)
                        Application.Icon = _source.MessageAppIcon;

                    if (Application.Id != _source.MessageAppId && _source.MessageAppId.HasValue)
                        Application.Id = _source.MessageAppId.Value;

                    if (Application.Name != _source.MessageAppName)
                        Application.Name = _source.MessageAppName;
                }
            } 
            else if(Application is null && hasApplication && _source.MessageAppId.HasValue)
            {
                Application = new MessageApplication
                {
                    Name = _source.MessageAppName,
                    CoverImage = _source.MessageAppCoverImage,
                    Description = _source.MessageAppDescription,
                    Icon = _source.MessageAppIcon,
                    Id = _source.MessageAppId.Value
                };
            }

            var hasReference = _source.ReferenceMessageId.HasValue
                || _source.ReferenceChannelId.HasValue
                || _source.ReferenceGuildId.HasValue;

            if(Reference is not null)
            {
                if (!_source.ReferenceMessageId.HasValue)
                    Reference = null;
                else
                {
                    if (!Reference.MessageId.Equals(_source.ReferenceMessageId))
                        Reference.MessageId = _source.ReferenceGuildId.ToOptional();

                    if (!Reference.InternalChannelId.Equals(_source.ReferenceChannelId))
                        Reference.InternalChannelId = _source.ReferenceChannelId.ToOptional();

                    if (Reference.GuildId.Equals(_source.ReferenceGuildId))
                        Reference.GuildId = _source.ReferenceGuildId.ToOptional();
                }
            }
            else if (hasReference)
            {
                Reference = new MessageReference(
                    _source.ReferenceMessageId,
                    _source.ReferenceChannelId,
                    _source.ReferenceGuildId
                );
            }
        }

        public Task AddReactionAsync(IEmote emote, RequestOptions options = null) => throw new NotImplementedException();
        public Task DeleteAsync(RequestOptions options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions options = null) => throw new NotImplementedException();
        public Task RemoveAllReactionsAsync(RequestOptions options = null) => throw new NotImplementedException();
        public Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions options = null) => throw new NotImplementedException();
        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions options = null) => throw new NotImplementedException();
        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions options = null) => throw new NotImplementedException();
        internal override object Clone() => throw new NotImplementedException();
        internal override void DisposeClone() => throw new NotImplementedException();

        IMessageChannel? IMessage.Channel => Channel.Value;

        IUser? IMessage.Author => Author.Value;
    }
}
