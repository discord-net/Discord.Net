using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ListActiveGuildThreadsResponse : IEntityModelSource
{
    [JsonPropertyName("threads")]
    public required Channel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required ThreadMember[] Members { get; set; }

    public IEnumerable<IEntityModel> GetEntities() => [..Threads, ..Members];
}
