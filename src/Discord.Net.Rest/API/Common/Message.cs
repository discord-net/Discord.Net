#pragma warning disable CS1591
using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class Message
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("type")]
        public MessageType Type { get; set; }
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonProperty("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonProperty("webhook_id")]
        public Optional<ulong> WebhookId { get; set; }
        [JsonProperty("author")]
        public Optional<User> Author { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonProperty("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonProperty("content")]
        public Optional<string> Content { get; set; }
        [JsonProperty("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }
        [JsonProperty("edited_timestamp")]
        public Optional<DateTimeOffset?> EditedTimestamp { get; set; }
        [JsonProperty("tts")]
        public Optional<bool> IsTextToSpeech { get; set; }
        [JsonProperty("mention_everyone")]
        public Optional<bool> MentionEveryone { get; set; }
        [JsonProperty("mentions")]
        public Optional<EntityOrId<User>[]> UserMentions { get; set; }
        [JsonProperty("mention_roles")]
        public Optional<ulong[]> RoleMentions { get; set; }
        [JsonProperty("attachments")]
        public Optional<Attachment[]> Attachments { get; set; }
        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonProperty("pinned")]
        public Optional<bool> Pinned { get; set; }
        [JsonProperty("reactions")]
        public Optional<Reaction[]> Reactions { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonProperty("activity")]
        public Optional<MessageActivity> Activity { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonProperty("application")]
        public Optional<MessageApplication> Application { get; set; }
    }
}
