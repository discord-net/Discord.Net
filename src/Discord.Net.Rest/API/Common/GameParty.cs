using Newtonsoft.Json;

namespace Discord.API
{
    internal class GameParty
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("size")]
        public int[] Size { get; set; }
    }
}