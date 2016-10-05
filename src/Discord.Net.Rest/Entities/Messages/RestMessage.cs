using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    public abstract class RestMessage : RestEntity<ulong>, IMessage, IUpdateable
    {
        private long _timestampTicks;

        public ulong ChannelId { get; }
        public IUser Author { get; }

        public string Content { get; private set; }

        public virtual bool IsTTS => false;
        public virtual bool IsPinned => false;
        public virtual bool IsWebhook => false;
        public virtual DateTimeOffset? EditedTimestamp => null;

        public virtual IReadOnlyCollection<IAttachment> Attachments => ImmutableArray.Create<IAttachment>();
        public virtual IReadOnlyCollection<IEmbed> Embeds => ImmutableArray.Create<IEmbed>();
        public virtual IReadOnlyCollection<ulong> MentionedChannelIds => ImmutableArray.Create<ulong>();
        public virtual IReadOnlyCollection<IRole> MentionedRoles => ImmutableArray.Create<IRole>();
        public virtual IReadOnlyCollection<IUser> MentionedUsers => ImmutableArray.Create<IUser>();

        public DateTimeOffset Timestamp => DateTimeUtils.FromTicks(_timestampTicks);

        internal RestMessage(BaseDiscordClient discord, ulong id, ulong channelId)
            : base(discord, id)
        {
            ChannelId = channelId;
        }
        internal static RestMessage Create(BaseDiscordClient discord, Model model)
        {
            if (model.Type == MessageType.Default)
                return RestUserMessage.Create(discord, model);
            else
                return RestSystemMessage.Create(discord, model);
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

        public override string ToString() => Content;

        MessageType IMessage.Type => MessageType.Default;
    }
}
