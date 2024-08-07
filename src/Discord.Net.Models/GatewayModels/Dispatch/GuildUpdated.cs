using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class GuildUpdated : IGuildUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required Guild Guild { get; set; }
}
