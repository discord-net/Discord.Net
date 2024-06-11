using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateMessageParams : AttachmentUploadParams
{
    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("nonce")]
    public Optional<string> Nonce { get; set; }

    [JsonPropertyName("tts")]
    public Optional<bool> IsTTS { get; set; }

    [JsonPropertyName("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonPropertyName("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonPropertyName("message_reference")]
    public Optional<MessageReference> MessageReference { get; set; }

    [JsonPropertyName("components")]
    public Optional<MessageComponent[]> Components { get; set; }

    [JsonPropertyName("sticker_ids")]
    public Optional<ulong[]> StickerIds { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }
}
