using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InvalidSessionPayloadData : IInvalidSessionPayloadData
{
    [JsonIgnore, JsonExtend]
    public bool CanResume { get; set; }
}
