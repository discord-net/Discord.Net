using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ModifyMessageParams
{
    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonPropertyName("flags")]
    public Optional<MessageFlags> Flags { get; set; }

    [JsonPropertyName("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonPropertyName("components")]
    public Optional<ActionRowComponent[]> Components { get; set; }
}
