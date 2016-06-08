using Newtonsoft.Json;

namespace Discord.API
{
    public class Presence
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("status")]
        public UserStatus Status { get; set; }
        [JsonProperty("game")]
        public Game Game { get; set; }
    }
}
