using Newtonsoft.Json;

namespace Discord.API
{
    public class MemberPresenceGame
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
