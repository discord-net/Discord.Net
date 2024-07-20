using Newtonsoft.Json;

namespace Discord.API;

internal class PollAnswerVoters
{
    [JsonProperty("users")]
    public User[] Users { get; set; }
}
