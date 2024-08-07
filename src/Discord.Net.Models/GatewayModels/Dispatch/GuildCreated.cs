using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(Unavailable))]
public abstract class GuildCreated : IGuildCreatePayloadData
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("unavailable")]
    public bool Unavailable { get; set; }
}
