using Discord.Models;
using Discord.Gateway.Cache;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Discord.Gateway
{
    public abstract class GatewayMessage : GatewayCacheableEntity<ulong, IMessageModel>, IMessage
    {
        public MessageChannelCacheable Channel { get; }

        public UserCacheable Author { get; }

        public ThreadChannelCacheable? Thread { get; }

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


        public IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException();

        public IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException();

        public IReadOnlyCollection<ITag> Tags => throw new NotImplementedException();

        public ILoadableEntityEnumerable<IChannel, ulong>? MentionedChannels => throw new NotImplementedException();

        public ILoadableEntityEnumerable<IRole, ulong>? MentionedRoles => throw new NotImplementedException();

        public ILoadableEntityEnumerable<IUser, ulong>? MentionedUsers => throw new NotImplementedException();

        public MessageActivity? Activity => throw new NotImplementedException();

        public MessageApplication? Application => throw new NotImplementedException();

        public MessageReference? Reference => throw new NotImplementedException();

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException();

        public IReadOnlyCollection<IMessageComponent> Components => throw new NotImplementedException();

        public IReadOnlyCollection<IStickerItem> Stickers => throw new NotImplementedException();

        public IMessageInteractionMetadata? InteractionMetadata => throw new NotImplementedException();

        public MessageRoleSubscriptionData? RoleSubscriptionData => throw new NotImplementedException();

        private IMessageModel _source;

        public GatewayMessage(DiscordGatewayClient discord, IMessageModel model)
            : base(discord, model.Id)
        {
            Update(model);

            Channel = new(
                model.ChannelId,
                discord,
                discord.State.Channels
                    .ProvideSpecific(model.ChannelId)
                    .Transform(channel => channel is IGatewayMessageChannel mc
                        ? mc
                        : throw new InvalidCastException($"Expected ISocketMessageChannel, got {channel.GetType().Name}")
                    )
            );

            Author = new(model.AuthorId, discord, discord.State.Users.ProvideSpecific(model.AuthorId));

        }

        [MemberNotNull(nameof(_source))]
        internal override void Update(IMessageModel model)
        {
            _source = model;
        }

        public Task AddReactionAsync(IEmote emote, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveReactionAsync(IEmote emote, IUser user, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveReactionAsync(IEmote emote, ulong userId, RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveAllReactionsAsync(RequestOptions? options = null) => throw new NotImplementedException();
        public Task RemoveAllReactionsForEmoteAsync(IEmote emote, RequestOptions? options = null) => throw new NotImplementedException();
        public IAsyncEnumerable<IReadOnlyCollection<IUser>> GetReactionUsersAsync(IEmote emoji, int limit, RequestOptions? options = null) => throw new NotImplementedException();
        public Task DeleteAsync(RequestOptions? options = null) => throw new NotImplementedException();

        ILoadableEntity<IMessageChannel, ulong> IMessage.Channel => Channel;

        ILoadableEntity<IUser, ulong> IMessage.Author => Author;

        ILoadableEntity<IThreadChannel, ulong>? IMessage.Thread => Thread;
    }
}
