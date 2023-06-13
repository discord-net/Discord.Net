using Discord.Net.V4.Core.API.Models.Messages;
using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public sealed class SocketMessage : SocketCacheableEntity<ulong, IMessageModel>, IMessage
    {
        public MessageType Type
            => _source.Type;

        public MessageSource Source => throw new NotImplementedException();

        public bool IsTTS => throw new NotImplementedException();

        public bool IsPinned => throw new NotImplementedException();

        public bool IsSuppressed => throw new NotImplementedException();

        public bool MentionedEveryone => throw new NotImplementedException();

        public string Content => throw new NotImplementedException();

        public string CleanContent => throw new NotImplementedException();

        public DateTimeOffset Timestamp => throw new NotImplementedException();

        public DateTimeOffset? EditedTimestamp => throw new NotImplementedException();

        public IMessageChannel Channel => throw new NotImplementedException();

        public IUser Author => throw new NotImplementedException();

        public IReadOnlyCollection<IAttachment> Attachments => throw new NotImplementedException();

        public IReadOnlyCollection<IEmbed> Embeds => throw new NotImplementedException();

        public IReadOnlyCollection<ITag> Tags => throw new NotImplementedException();

        public IReadOnlyCollection<ulong> MentionedChannelIds => throw new NotImplementedException();

        public IReadOnlyCollection<ulong> MentionedRoleIds => throw new NotImplementedException();

        public IReadOnlyCollection<ulong> MentionedUserIds => throw new NotImplementedException();

        public MessageActivity Activity => throw new NotImplementedException();

        public MessageApplication Application => throw new NotImplementedException();

        public MessageReference Reference => throw new NotImplementedException();

        public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => throw new NotImplementedException();

        public IReadOnlyCollection<IMessageComponent> Components => throw new NotImplementedException();

        public IReadOnlyCollection<IStickerItem> Stickers => throw new NotImplementedException();

        public MessageFlags? Flags => throw new NotImplementedException();

        public IMessageInteraction Interaction => throw new NotImplementedException();

        public DateTimeOffset CreatedAt => throw new NotImplementedException();

        private IMessageModel _source;
        public SocketMessage(DiscordSocketClient discord, ulong id, IMessageModel model)
            : base(discord, id)
        {
            _source = model;
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

        internal override IMessageModel GetModel()
            => _source;
        internal override void Update(IMessageModel model)
        {
            _source = model;
        }
    }
}
