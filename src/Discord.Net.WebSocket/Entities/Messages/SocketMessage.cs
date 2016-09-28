using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.WebSocket
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public abstract class SocketMessage : SocketEntity<ulong>, IMessage, IUpdateable
    {
        private long _timestampTicks;

        public ulong ChannelId { get; }
        public SocketUser Author { get; }

        public string Content { get; private set; }

        public virtual bool IsTTS => false;
        public virtual bool IsPinned => false;
        public virtual DateTimeOffset? EditedTimestamp => null;

        public virtual IReadOnlyCollection<IAttachment> Attachments => ImmutableArray.Create<IAttachment>();
        public virtual IReadOnlyCollection<IEmbed> Embeds => ImmutableArray.Create<IEmbed>();
        public virtual IReadOnlyCollection<ulong> MentionedChannelIds => ImmutableArray.Create<ulong>();
        public virtual IReadOnlyCollection<IRole> MentionedRoles => ImmutableArray.Create<IRole>();
        public virtual IReadOnlyCollection<IUser> MentionedUsers => ImmutableArray.Create<IUser>();

        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal SocketMessage(DiscordSocketClient discord, ulong id, ulong channelId, SocketUser author)
            : base(discord, id)
        {
            ChannelId = channelId;
            Author = author;
        }
        internal static SocketMessage Create(DiscordSocketClient discord, SocketUser author, Model model)
        {
            if (model.Type == MessageType.Default)
                return SocketUserMessage.Create(discord, author, model);
            else
                return SocketSystemMessage.Create(discord, author, model);
        }
        internal virtual void Update(Model model)
        {
            if (model.Timestamp.IsSpecified)
                _timestampTicks = model.Timestamp.Value.UtcTicks;

            if (model.Content.IsSpecified)
                Content = model.Content.Value;
        }

        public async Task UpdateAsync()
        {
            var model = await Discord.ApiClient.GetChannelMessageAsync(ChannelId, Id).ConfigureAwait(false);
            Update(model);
        }

        //IMessage
        IUser IMessage.Author => Author;
        MessageType IMessage.Type => MessageType.Default;
    }
}
