using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class EntitlementPayload : IEntitlementPayloadData
{
    [JsonIgnore, JsonExtend]
    public required Entitlement Entitlement { get; set; }
}
