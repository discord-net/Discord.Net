using Discord.API.Converters;
using Newtonsoft.Json;

namespace Discord.API.Client
{
    public class MemberPresence : MemberReference
    {
        public class GameInfo
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }
        [JsonProperty("game")]
        public GameInfo Game { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("roles"), JsonConverter(typeof(LongStringArrayConverter))]
        public ulong[] Roles { get; set; }  //TODO: Might be temporary
    }
}
