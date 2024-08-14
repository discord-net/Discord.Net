using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class PresenceUpdated : IPresenceUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required Presence Presence { get; set; }
}
