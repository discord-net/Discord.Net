using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class Message
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("author")]
    public Optional<User> Author { get; set; }

    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("timestamp")]
    public Optional<DateTimeOffset> Timestamp { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedTimestamp { get; set; }

    [JsonPropertyName("tts")]
    public bool IsTextToSpeech { get; set; }

    [JsonPropertyName("mention_everyone")]
    public bool MentionEveryone { get; set; }

    [JsonPropertyName("mentions")]
    public required User[] UserMentions { get; set; }

    [JsonPropertyName("mention_roles")]
    public required ulong[] RoleMentions { get; set; }

    [JsonPropertyName("mention_channels")]
    public Optional<MessageMentionedChannel[]> ChannelMentions { get; set; }

    [JsonPropertyName("embeds")]
    public required Embed[] Embeds { get; set; }

    [JsonPropertyName("reactions")]
    public Optional<Reaction[]> Reactions { get; set; }

    [JsonPropertyName("attachments")]
    public Optional<Attachment[]> Attachments { get; set; }

    [JsonPropertyName("nonce")]
    public Optional<string> Nonce { get; set; }

    [JsonPropertyName("pinned")]
    public bool IsPinned { get; set; }

    [JsonPropertyName("webhook_id")]
    public Optional<ulong> WebhookId { get; set; }

    [JsonPropertyName("type")]
    public MessageType Type { get; set; }

    // sent with Rich Presence-related chat embeds
    [JsonPropertyName("activity")]
    public Optional<MessageActivity> Activity { get; set; }

    // sent with Rich Presence-related chat embeds
    [JsonPropertyName("application")]
    public Optional<MessageApplication> Application { get; set; }

    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    [JsonPropertyName("message_reference")]
    public Optional<MessageReference> Reference { get; set; }

    [JsonPropertyName("flags")]
    public Optional<MessageFlags> Flags { get; set; }

    [JsonPropertyName("referenced_message")]
    public Optional<Message?> ReferencedMessage { get; set; }

    [JsonPropertyName("interaction")]
    public Optional<MessageInteraction> Interaction { get; set; }

    [JsonPropertyName("thread")]
    public Optional<Channel> Thread { get; set; }

    [JsonPropertyName("components")]
    public Optional<ActionRowComponent[]> Components { get; set; }

    [JsonPropertyName("sticker_items")]
    public Optional<StickerItem[]> StickerItems { get; set; }

    [JsonPropertyName("position")]
    public Optional<int> Position { get; set; }

    [JsonPropertyName("role_subscription_data")]
    public Optional<MessageRoleSubscriptionData> RoleSubscriptionData { get; set; }
}
