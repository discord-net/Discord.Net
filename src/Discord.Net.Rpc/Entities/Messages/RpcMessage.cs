using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Model = Discord.API.Rpc.Message;

namespace Discord.Rpc
{
    public abstract class RpcMessage : RpcEntity<ulong>, IMessage
    {
        private long _timestampTicks;

        public IMessageChannel Channel { get; }
        public RpcUser Author { get; }
        public MessageSource Source { get; }

        public string Content { get; private set; }
        public Color AuthorColor { get; private set; }

        public DateTimeOffset CreatedAt => DateTimeUtils.FromSnowflake(Id);
        public virtual bool IsTTS => false;
        public virtual bool IsPinned => false;
        public virtual bool IsBlocked => false;
        public virtual DateTimeOffset? EditedTimestamp => null;
        public virtual IReadOnlyCollection<Attachment> Attachments => ImmutableArray.Create<Attachment>();
        public virtual IReadOnlyCollection<Embed> Embeds => ImmutableArray.Create<Embed>();
        public virtual IReadOnlyCollection<ulong> MentionedChannelIds => ImmutableArray.Create<ulong>();
        public virtual IReadOnlyCollection<ulong> MentionedRoleIds => ImmutableArray.Create<ulong>();
        public virtual IReadOnlyCollection<ulong> MentionedUserIds => ImmutableArray.Create<ulong>();
        public virtual IReadOnlyCollection<ITag> Tags => ImmutableArray.Create<ITag>();
        public virtual ulong? WebhookId => null;
        public bool IsWebhook => WebhookId != null;

        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal RpcMessage(DiscordRpcClient discord, ulong id, RestVirtualMessageChannel channel, RpcUser author, MessageSource source)
            : base(discord, id)
        {
            Channel = channel;
            Author = author;
            Source = source;
        }
        internal static RpcMessage Create(DiscordRpcClient discord, ulong channelId, Model model)
        {
            //model.ChannelId is always 0, needs to be passed from the event
            if (model.Type == MessageType.Default)
                return RpcUserMessage.Create(discord, channelId, model);
            else
                return RpcSystemMessage.Create(discord, channelId, model);
        }
        internal virtual void Update(Model model)
        {
            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
                Content = model.Content.Value;
            if (model.AuthorColor.IsSpecified)
                AuthorColor = new Color(Convert.ToUInt32(model.AuthorColor.Value.Substring(1), 16));
        }

        public Task DeleteAsync(RequestOptions options = null)
            => MessageHelper.DeleteAsync(this, Discord, options);

        public override string ToString() => Content;
        
        //IMessage
        IMessageChannel IMessage.Channel => Channel;
        MessageType IMessage.Type => MessageType.Default;
        IUser IMessage.Author => Author;
        IReadOnlyCollection<IAttachment> IMessage.Attachments => Attachments;
        IReadOnlyCollection<IEmbed> IMessage.Embeds => Embeds;
    }
}
