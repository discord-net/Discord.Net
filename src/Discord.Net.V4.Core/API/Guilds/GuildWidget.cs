using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class GuildWidget
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("instant_invite")]
    public string? InstantInvite { get; set; }

    [JsonPropertyName("channels")]
    public Channel[] PartialChannels { get; set; }

    [JsonPropertyName("members")]
    public GuildMember[] PartialMembers { get; set; }

    [JsonPropertyName("presence_count")]
    public int PresenceCount { get; set; }
}
