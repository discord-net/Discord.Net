#pragma warning disable CS1591
using Newtonsoft.Json;

namespace Discord.API
{
    internal class Guild
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
        [JsonProperty("splash")]
        public string Splash { get; set; }
        [JsonProperty("owner_id")]
        public ulong OwnerId { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [JsonProperty("afk_timeout")]
        public int AFKTimeout { get; set; }
        [JsonProperty("embed_enabled")]
        public bool EmbedEnabled { get; set; }
        [JsonProperty("embed_channel_id")]
        public ulong? EmbedChannelId { get; set; }
        [JsonProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }
        [JsonProperty("voice_states")]
        public VoiceState[] VoiceStates { get; set; }
        [JsonProperty("roles")]
        public Role[] Roles { get; set; }
        [JsonProperty("emojis")]
        public Emoji[] Emojis { get; set; }
        [JsonProperty("features")]
        public string[] Features { get; set; }
        [JsonProperty("mfa_level")]
        public MfaLevel MfaLevel { get; set; }
        [JsonProperty("default_message_notifications")]
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
    }
}
