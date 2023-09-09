using System.Text.Json.Serialization;

namespace Discord.API;

public class GuildScheduledEventUser
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }
}
