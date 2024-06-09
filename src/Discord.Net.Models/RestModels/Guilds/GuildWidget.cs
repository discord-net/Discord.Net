using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildWidget
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("instant_invite")]
    public string? InstantInvite { get; set; }

    [JsonPropertyName("channels")]
    public required Channel[] PartialChannels { get; set; }

    [JsonPropertyName("members")]
    public required GuildMember[] PartialMembers { get; set; }

    [JsonPropertyName("presence_count")]
    public int PresenceCount { get; set; }
}
