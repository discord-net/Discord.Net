using Newtonsoft.Json;

namespace Discord.API;

internal class PollMedia
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("emoji")]
    public Optional<Emoji> Emoji { get; set; }
}
