using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    public abstract class SocketMessage : SocketEntity<ulong>, IMessage
    {
        private long _timestampTicks;
        
        public SocketUser Author { get; }
        public ISocketMessageChannel Channel { get; }
        public MessageSource Source { get; }

        public string Content { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public virtual bool IsTTS => false;
        public virtual bool IsPinned => false;
        public virtual DateTimeOffset? EditedTimestamp => null;
        public virtual IReadOnlyCollection<Attachment> Attachments => ImmutableArray.Create<Attachment>();
        public virtual IReadOnlyCollection<Embed> Embeds => ImmutableArray.Create<Embed>();
        public virtual IReadOnlyCollection<SocketGuildChannel> MentionedChannels => ImmutableArray.Create<SocketGuildChannel>();
        public virtual IReadOnlyCollection<SocketRole> MentionedRoles => ImmutableArray.Create<SocketRole>();
        public virtual IReadOnlyCollection<SocketUser> MentionedUsers => ImmutableArray.Create<SocketUser>();
        public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();

        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal SocketMessage(DiscordSocketClient discord, ulong id, ISocketMessageChannel channel, SocketUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }
        internal static SocketMessage Create(DiscordSocketClient discord, ClientState state, SocketUser author, ISocketMessageChannel channel, Model model)
        {
            if (model.Type == MessageType.Default)
                return SocketUserMessage.Create(discord, state, author, channel, model);
            else
                return SocketSystemMessage.Create(discord, state, author, channel, model);
        }
        internal virtual void Update(ClientState state, Model model)
        {
            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
                Content = model.Content.Value;
        }

        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        public override string ToString() => Content;
        internal SocketMessage Clone() => MemberwiseClone() as SocketMessage;

        //IMessage
        IUser IMessage.Author => Author;
        IMessageChannel IMessage.Channel => Channel;
        MessageType IMessage.Type => MessageType.Default;
        IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
        IReadOnlyCollection<ulong> IMessage.MentionedChannelIds => MentionedChannels.Select(x => x.Id).ToImmutableArray();
        IReadOnlyCollection<ulong> IMessage.MentionedRoleIds => MentionedRoles.Select(x => x.Id).ToImmutableArray();
        IReadOnlyCollection<ulong> IMessage.MentionedUserIds => MentionedUsers.Select(x => x.Id).ToImmutableArray();
    }
}
