using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ApplicationIntegrationTypes : IApplicationIntegrationTypes
{
    [JsonPropertyName("0")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("1")]
    public Optional<ulong> UserId { get; set; }

    ulong? IApplicationIntegrationTypes.UserId => UserId.ToNullable();

    ulong? IApplicationIntegrationTypes.GuildId => GuildId.ToNullable();
}
