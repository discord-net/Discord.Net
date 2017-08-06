#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API
{
    internal class Guild
    {
        [ModelProperty("id")]
        public ulong Id { get; set; }
        [ModelProperty("name")]
        public string Name { get; set; }
        [ModelProperty("icon")]
        public string Icon { get; set; }
        [ModelProperty("splash")]
        public string Splash { get; set; }
        [ModelProperty("owner_id")]
        public ulong OwnerId { get; set; }
        [ModelProperty("region")]
        public string Region { get; set; }
        [ModelProperty("afk_channel_id")]
        public ulong? AFKChannelId { get; set; }
        [ModelProperty("afk_timeout")]
        public int AFKTimeout { get; set; }
        [ModelProperty("embed_enabled")]
        public bool EmbedEnabled { get; set; }
        [ModelProperty("embed_channel_id")]
        public ulong? EmbedChannelId { get; set; }
        [ModelProperty("verification_level")]
        public VerificationLevel VerificationLevel { get; set; }
        [ModelProperty("voice_states")]
        public VoiceState[] VoiceStates { get; set; }
        [ModelProperty("roles")]
        public Role[] Roles { get; set; }
        [ModelProperty("emojis")]
        public Emoji[] Emojis { get; set; }
        [ModelProperty("features")]
        public string[] Features { get; set; }
        [ModelProperty("mfa_level")]
        public MfaLevel MfaLevel { get; set; }
        [ModelProperty("default_message_notifications")]
        public DefaultMessageNotifications DefaultMessageNotifications { get; set; }
    }
}
