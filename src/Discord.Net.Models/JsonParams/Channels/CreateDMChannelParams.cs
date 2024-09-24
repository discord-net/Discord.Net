using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class CreateDMChannelParams
{
    [JsonPropertyName("recipient_id")]
    public required ulong RecipientId { get; set; }
}
