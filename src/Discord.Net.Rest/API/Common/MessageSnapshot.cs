using Newtonsoft.Json;

namespace Discord.API;

internal class MessageSnapshot
{
    [JsonProperty("message")]
    public Message Message { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }
}
