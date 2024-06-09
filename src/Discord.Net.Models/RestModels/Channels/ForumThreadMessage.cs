using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ForumThreadMessage
{
    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("embeds")]
    public Optional<Embed[]> Embeds { get; set; }

    [JsonPropertyName("allowed_mentions")]
    public Optional<AllowedMentions> AllowedMentions { get; set; }

    [JsonPropertyName("components")]
    public Optional<ActionRow[]> Components { get; set; }

    [JsonPropertyName("sticker_ids")]
    public Optional<ulong[]> Stickers { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }
}
