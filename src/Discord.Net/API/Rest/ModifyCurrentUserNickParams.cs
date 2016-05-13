using Newtonsoft.Json;

namespace Discord.API.Rest
{
    public class ModifyCurrentUserNickParams
    {
        [JsonProperty("nick")]
        public string Nickname { get; set; }
    }
}
