using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class VoiceDevice
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
