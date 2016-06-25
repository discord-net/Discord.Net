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
        public Optional<VerificationLevel> VerificationLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public Optional<DefaultMessageNotifications> DefaultMessageNotifications { get; set; }
        [JsonProperty("afk_timeout")]
        public Optional<int> AFKTimeout { get; set; }
        [JsonProperty("icon"), Image]
        public Optional<Stream> Icon { get; set; }
        [JsonProperty("splash"), Image]
        public Optional<Stream> Splash { get; set; }

        [JsonProperty("afk_channel_id")]
        public Optional<ulong?> AFKChannelId { get; set; }
        [JsonIgnore]
        public Optional<IVoiceChannel> AFKChannel { set { OwnerId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }

        [JsonProperty("owner_id")]
        public Optional<ulong> OwnerId { get; set; }
        [JsonIgnore]
        public Optional<IGuildUser> Owner { set { OwnerId = value.IsSpecified ? value.Value.Id : Optional.Create<ulong>(); } }
    }
}
