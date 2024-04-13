using Newtonsoft.Json;

namespace Discord.API;

internal class PollAnswer
{
    [JsonProperty("answer_id")]
    public uint AnswerId { get; set; }

    [JsonProperty("poll_media")]
    public PollMedia PollMedia { get; set; }
}
