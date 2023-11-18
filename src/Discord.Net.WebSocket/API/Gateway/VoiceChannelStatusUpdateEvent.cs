using Newtonsoft.Json;

namespace Discord.API.Gateway;

internal class VoiceChannelStatusUpdateEvent
{
    [JsonProperty("id")]
    public ulong Id { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
}
