using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessageActivity : IMessageActivityModel
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("party_id")]
    public Optional<string> PartyId { get; set; }

    string? IMessageActivityModel.PartyId => PartyId;
}
