using System.Text.Json.Serialization;
using System;

namespace Discord.API
{
    internal class Message
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("type")]
        public MessageType Type { get; set; }
        [JsonPropertyName("channel_id")]
        public ulong ChannelId { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonPropertyName("guild_id")]
        public Optional<ulong> GuildId { get; set; }
        [JsonPropertyName("webhook_id")]
        public Optional<ulong> WebhookId { get; set; }
        [JsonPropertyName("author")]
        public Optional<User> Author { get; set; }
        // ALWAYS sent on WebSocket messages
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; set; }
        [JsonPropertyName("content")]
        public Optional<string> Content { get; set; }
        [JsonPropertyName("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }
        [JsonPropertyName("edited_timestamp")]
        public Optional<DateTimeOffset?> EditedTimestamp { get; set; }
        [JsonPropertyName("tts")]
        public Optional<bool> IsTextToSpeech { get; set; }
        [JsonPropertyName("mention_everyone")]
        public Optional<bool> MentionEveryone { get; set; }
        [JsonPropertyName("mentions")]
        public Optional<User[]> UserMentions { get; set; }
        [JsonPropertyName("mention_roles")]
        public Optional<ulong[]> RoleMentions { get; set; }
        [JsonPropertyName("attachments")]
        public Optional<Attachment[]> Attachments { get; set; }
        [JsonPropertyName("embeds")]
        public Optional<Embed[]> Embeds { get; set; }
        [JsonPropertyName("pinned")]
        public Optional<bool> Pinned { get; set; }
        [JsonPropertyName("reactions")]
        public Optional<Reaction[]> Reactions { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonPropertyName("activity")]
        public Optional<MessageActivity> Activity { get; set; }
        // sent with Rich Presence-related chat embeds
        [JsonPropertyName("application")]
        public Optional<MessageApplication> Application { get; set; }
        [JsonPropertyName("message_reference")]
        public Optional<MessageReference> Reference { get; set; }
        [JsonPropertyName("flags")]
        public Optional<MessageFlags> Flags { get; set; }
        [JsonPropertyName("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        [JsonPropertyName("referenced_message")]
        public Optional<Message> ReferencedMessage { get; set; }
        [JsonPropertyName("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }
        public Optional<MessageInteraction> Interaction { get; set; }
        [JsonPropertyName("sticker_items")]
        public Optional<StickerItem[]> StickerItems { get; set; }
    }
}
