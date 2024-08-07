using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class IntegrationCreatedUpdated : IIntegrationCreateUpdatePayloadData
{
    [JsonIgnore, JsonExtend]
    public required Integration Integration { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }
}
