using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class MessageActivity
{
    [JsonPropertyName("type")]
    public MessageActivityType Type { get; set; }

    [JsonPropertyName("party_id")]
    public Optional<string> PartyId { get; set; }
}
