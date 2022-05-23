using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageModel : IEntityModel<ulong>
    {
        MessageType Type { get; set; }
        ulong ChannelId { get; set; }
        ulong? GuildId { get; set; }
        ulong AuthorId { get; set; }
        bool IsWebhookMessage { get; set; }
        string Content { get; set; }
        long Timestamp { get; set; }
        long? EditedTimestamp { get; set; }
        bool IsTextToSpeech { get; set; }
        bool MentionEveryone { get; set; }
        ulong[] UserMentionIds { get; set; }
        ulong[] RoleMentionIds { get; set; }

        IAttachmentModel[] Attachments { get; set; }
        IEmbedModel[] Embeds { get; set; }
        IReactionMetadataModel[] Reactions { get; set; }
        bool Pinned { get; set; }
        IMessageActivityModel Activity { get; set; }
        IPartialApplicationModel Application { get; set; }
        ulong? ApplicationId { get; set; }

        // message reference
        ulong? ReferenceMessageId { get; set; }
        ulong? ReferenceMessageChannelId { get; set; }
        ulong? ReferenceMessageGuildId { get; set; }

        MessageFlags Flags { get; set; }

        // interaction
        ulong? InteractionId { get; set; }
        string InteractionName { get; set; }
        InteractionType? InteractionType { get; set; }
        ulong? InteractionUserId { get; set; }

        IMessageComponentModel[] Components { get; set; }
        IStickerItemModel[] Stickers { get; set; }
    }
}
