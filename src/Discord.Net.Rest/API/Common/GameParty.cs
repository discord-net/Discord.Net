using Newtonsoft.Json;

namespace Discord.API
{
    internal class GameParty
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("size")]
        public ulong[] Size { get; set; }
    }
}