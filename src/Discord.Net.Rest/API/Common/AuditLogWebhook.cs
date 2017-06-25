using Newtonsoft.Json;

namespace Discord.API
{
    internal class AuditLogWebhook : User
    {
        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
