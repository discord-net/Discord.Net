using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class EntitlementPayload : IEntitlementPayloadData
{
    [JsonIgnore, JsonExtend] public Entitlement Entitlement { get; set; } = null!;
}
