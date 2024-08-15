using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class InteractionCreated : IInteractionCreatedPayloadData
{
    [JsonIgnore, JsonExtend] public Interaction Interaction { get; set; } = null!;
}
