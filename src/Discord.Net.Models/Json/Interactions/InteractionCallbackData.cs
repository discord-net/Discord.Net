using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionCallbackData : AttachmentUploadParams
{
    [JsonPropertyName("tts")]
    public Optional<bool> TTS { get; set; }

    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonPropertyName("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("components")]
    public Optional<MessageComponent[]> Components { get; set; }

    [JsonPropertyName("choices")]
    public Optional<IApplicationCommandOptionChoiceModel[]> Choices { get; set; }

    [JsonPropertyName("title")]
    public Optional<string> Title { get; set; }

    [JsonPropertyName("custom_id")]
    public Optional<string> CustomId { get; set; }
}
