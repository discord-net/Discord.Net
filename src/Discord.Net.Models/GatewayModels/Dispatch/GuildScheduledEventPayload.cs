using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildScheduledEventPayload : IGuildScheduledEventPayloadData
{
    [JsonIgnore, JsonExtend] public GuildScheduledEvent Event { get; set; } = null!;
}
