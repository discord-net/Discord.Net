using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class PresenceUpdated : IPresenceUpdatedPayloadData
{
    [JsonIgnore, JsonExtend] public Presence Presence { get; set; } = null!;
}
