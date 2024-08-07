using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildScheduledEventPayload : IGuildScheduledEventPayloadData
{
    [JsonIgnore, JsonExtend]
    public required GuildScheduledEvent Event { get; set; }
}
