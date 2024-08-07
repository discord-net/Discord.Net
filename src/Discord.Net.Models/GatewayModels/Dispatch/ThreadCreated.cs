using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class ThreadCreated : IThreadCreatedPayloadData
{
    [JsonPropertyName("newly_created")]
    public bool NewlyCreated { get; set; }

    [JsonIgnore, JsonExtend]
    public required ThreadChannelBase Thread { get; set; }
}
