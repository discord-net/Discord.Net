using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class CreateDMChannelParams
{
    [JsonPropertyName("recipient_id")]
    public required ulong RecipientId { get; set; }
}
