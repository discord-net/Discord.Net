using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class VoiceServerUpdated : IVoiceServerUpdatedPayloadData
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; set; }
}
