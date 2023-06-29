using System.Text.Json.Serialization;

namespace Discord.API;

internal class ChannelThreads
{
    [JsonPropertyName("threads")]
    public Channel[] Threads { get; set; }

    [JsonPropertyName("members")]
    public ThreadMember[] Members { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }
}
