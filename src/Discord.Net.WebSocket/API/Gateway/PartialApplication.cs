using Newtonsoft.Json;

namespace Discord.API.Gateway
{
    internal class PartialApplication
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("flags")]
        public ApplicationFlags Flags { get; set; }
    }
}
