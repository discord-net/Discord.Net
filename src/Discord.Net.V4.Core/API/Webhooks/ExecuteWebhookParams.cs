using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ExecuteWebhookParams
{
    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("username")]
    public Optional<string> Username { get; set; }

    [JsonPropertyName("avatar_url")]
    public Optional<string> AvatarUrl { get; set; }

    [JsonPropertyName("tts")]
    public Optional<bool> IsTTS { get; set; }

    [JsonPropertyName("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonPropertyName("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonPropertyName("components")]
    public Optional<MessageComponent[]> Components { get; set; }

    [JsonPropertyName("flags")]
    public Optional<MessageFlags> Flags { get; set; }

    [JsonPropertyName("thread_name")]
    public Optional<string> ThreadName { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> AppliedTags { get; set; }
}
