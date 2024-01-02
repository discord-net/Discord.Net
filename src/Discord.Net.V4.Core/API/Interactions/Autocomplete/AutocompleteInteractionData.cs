using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class AutocompleteInteractionData : IDiscordInteractionData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; }

    [JsonPropertyName("version")]
    public ulong Version { get; set; }

    [JsonPropertyName("options")]
    public required AutocompleteInteractionDataOption[] Options { get; set; }
}
