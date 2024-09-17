using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionCallbackActivityInstance : IActivityInstanceModel
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
}