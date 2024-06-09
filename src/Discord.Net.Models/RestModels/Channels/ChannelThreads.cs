using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ChannelThreads : IEntityModelSource
{
    [JsonPropertyName("threads")]
    public required Channel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required ThreadMember[] Members { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    public IEnumerable<IEntityModel> GetEntities() => [..Threads, ..Members];
}
