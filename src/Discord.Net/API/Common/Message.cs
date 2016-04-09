using Newtonsoft.Json;
using System;

namespace Discord.API
{
    public class Message
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        [JsonProperty("author")]
        public User Author { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public DateTime? EditedTimestamp { get; set; }
        [JsonProperty("tts")]
        public bool IsTextToSpeech { get; set; }
        [JsonProperty("mention_everyone")]
        public bool IsMentioningEveryone { get; set; }
        [JsonProperty("mentions")]
        public User[] Mentions { get; set; }
        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }
        [JsonProperty("embeds")]
        public Embed[] Embeds { get; set; }
        [JsonProperty("nonce")]
        public uint? Nonce { get; set; }
    }
}
