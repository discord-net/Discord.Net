using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ApplicationCommandInteractionData : IDiscordInteractionData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public ApplicationCommandType Type { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<ApplicationCommandInteractionDataResolved> Resolved { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionDataOption[]> Options { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public Optional<ulong> TargetId { get; set; }
}
