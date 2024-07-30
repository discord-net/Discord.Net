using System.Text.Json.Serialization;

namespace Discord.Models.GatewayModels;

public sealed class UnavailableGuild : IUnavailableGuild
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("unavailable")]
    public bool Unavailable { get; set; }
}
