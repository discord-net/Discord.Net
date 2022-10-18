using System.Text.Json.Serialization;

namespace Discord.API.Rest
{
    internal class ChannelThreads
    {
        [JsonPropertyName("threads")]
        public Channel[] Threads { get; set; }

        [JsonPropertyName("members")]
        public ThreadMember[] Members { get; set; }
    }
}
