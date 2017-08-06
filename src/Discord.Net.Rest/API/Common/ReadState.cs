#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class ReadState
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("mention_count")]
        public int MentionCount { get; set; }
        [ModelProperty("last_message_id")]
        public Optional<ulong> LastMessageId { get; set; }
    }
}
