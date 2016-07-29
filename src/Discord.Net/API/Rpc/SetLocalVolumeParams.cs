using Newtonsoft.Json;

namespace Discord.API.Rpc
{
    public class SetLocalVolumeParams
    {
        [JsonProperty("volume")]
        public int Volume { get; set; }
    }
}
