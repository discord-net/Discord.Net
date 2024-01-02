using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class ChannelThreads
{
    [JsonPropertyName("threads")]
    public required Channel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required ThreadMember[] Members { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
