using Discord.Net.Converters;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyGuildParams
    {        
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region"), JsonConverterAttribute(typeof(StringEntityConverter))]
        public IVoiceRegion Region { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }
        [JsonProperty("icon"), JsonConverter(typeof(ImageConverter))]
        public Stream Icon { get; set; }
        [JsonProperty("owner_id")]
        public GuildMember Owner { get; set; }
        [JsonProperty("splash"), JsonConverter(typeof(ImageConverter))]
        public Stream Splash { get; set; }
    }
}
