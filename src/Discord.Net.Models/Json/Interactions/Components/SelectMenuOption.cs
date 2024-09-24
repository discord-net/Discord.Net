using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class SelectMenuOption : ISelectMenuOptionModel
{
    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("value")]
    public required string Value { get; set; }

    [JsonPropertyName("description")]
    public Optional<string> Description { get; set; }

    [JsonPropertyName("emoji")]
    public Optional<DiscordEmojiId> Emoji { get; set; }

    [JsonPropertyName("default")]
    public Optional<bool> IsDefault { get; set; }

    string? ISelectMenuOptionModel.Description => ~Description;

    DiscordEmojiId? ISelectMenuOptionModel.Emote => Emoji.ToNullable();

    bool? ISelectMenuOptionModel.IsDefault => ~IsDefault;
}
