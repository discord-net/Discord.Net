using Discord.Net.JsonConverters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.IO;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModifyGuildRequest : IRestRequest<Guild>
    {
        string IRestRequest.Method => "PATCH";
        string IRestRequest.Endpoint => $"guilds/{GuildId}";
        object IRestRequest.Payload => this;

        public ulong GuildId { get; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public VoiceRegion Region { get; set; }
        [JsonProperty("verification_level")]
        public int VerificationLevel { get; set; }
        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }
        [JsonProperty("icon"), JsonConverter(typeof(ImageConverter))]
        public Stream Icon { get; set; }
        [JsonProperty("owner_id")]
        public GuildPresence Owner { get; set; }
        [JsonProperty("splash"), JsonConverter(typeof(ImageConverter))]
        public Stream Splash { get; set; }

        public ModifyGuildRequest(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
