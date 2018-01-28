using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    internal class Pan
    {
        [JsonProperty("left")]
        public float Left { get; set; }
        [JsonProperty("right")]
        public float Right { get; set; }
    }
}
