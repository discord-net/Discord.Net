using Discord.Net.Converters;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    public class ModifyGuildParams
    {        
        [JsonProperty("name")]
        public Optional<string> Name { get; set; }
        [JsonProperty("region")]
        public Optional<IVoiceRegion> Region { get; set; }
        [JsonProperty("verification_level")]
        public Optional<int> VerificationLevel { get; set; }
        [JsonProperty("afk_channel_id")]
        public Optional<ulong?> AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public Optional<int> AFKTimeout { get; set; }
        [JsonProperty("icon"), JsonConverter(typeof(ImageConverter))]
        public Optional<Stream> Icon { get; set; }
        [JsonProperty("owner_id")]
        public Optional<GuildMember> Owner { get; set; }
        [JsonProperty("splash"), JsonConverter(typeof(ImageConverter))]
        public Optional<Stream> Splash { get; set; }
    }
}
