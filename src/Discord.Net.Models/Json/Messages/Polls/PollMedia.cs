using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class PollMedia : IPollMediaModel
{
    [JsonPropertyName("text")]
    public Optional<string> Text { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<DiscordEmojiId> Emoji { get; set; }

    DiscordEmojiId? IPollMediaModel.Emoji => Emoji.ToNullable();

    string? IPollMediaModel.Text => ~Text;
}