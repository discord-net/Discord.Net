using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildScheduledEventUser : IEntityModelSource
{
    [JsonPropertyName("guild_scheduled_event_id")]
    public ulong GuildScheduledEventId { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }

    public IEnumerable<IEntityModel> GetEntities()
    {
        yield return User;

        if (Member)
            yield return Member.Value;

    }
}
