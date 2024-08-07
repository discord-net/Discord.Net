using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ResumePayloadData : IResumePayloadData
{
    [JsonPropertyName("token")]
    public required string SessionToken { get; set; }

    [JsonPropertyName("session_id")]
    public required string SessionId { get; set; }

    [JsonPropertyName("seq")]
    public int Sequence { get; set; }
}
