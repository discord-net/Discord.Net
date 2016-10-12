using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class Pan
    {
        [JsonProperty("left")]
        public float Left { get; set; }
        [JsonProperty("right")]
        public float Right { get; set; }
    }
}
