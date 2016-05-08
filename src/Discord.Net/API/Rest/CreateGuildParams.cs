using Discord.Net.Converters;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class CreateGuildParams
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("icon"), JsonConverter(typeof(ImageConverter))]
        public Stream Icon { get; set; }
    }
}
