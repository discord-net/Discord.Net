using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class InteractionCreated : IInteractionCreatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required Interaction Interaction { get; set; }
}
