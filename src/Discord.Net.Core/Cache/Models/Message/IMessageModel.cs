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
        DateTimeOffset Timestamp { get; set; }
        DateTimeOffset? EditedTimestamp { get; set; }
        bool IsTextToSpeech { get; set; }
        bool MentionEveryone { get; set; }
        ulong[] UserMentionIds { get; set; }
        ulong[] RoleMentionIds { get; set; }
    }
}
