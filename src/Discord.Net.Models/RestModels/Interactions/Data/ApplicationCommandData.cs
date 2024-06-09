using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[InteractionDataType(InteractionDataTypes.ApplicationCommand)]
public sealed class ApplicationCommandData : InteractionData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("resolved")]
    public Optional<InteractionDataResolved> Resolved { get; set; }

    [JsonPropertyName("options")]
    public Optional<ApplicationCommandInteractionDataOption> Options { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("target_id")]
    public Optional<ulong> TargetId { get; set; }
}
