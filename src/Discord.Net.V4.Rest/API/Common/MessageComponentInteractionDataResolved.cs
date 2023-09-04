using System.Text.Json.Serialization;

namespace Discord.API;

public class MessageComponentInteractionDataResolved
{
    [JsonPropertyName("users")]
    public Optional<Dictionary<string, User>> Users { get; set; }

    [JsonPropertyName("members")]
    public Optional<Dictionary<string, GuildMember>> Members { get; set; }

    [JsonPropertyName("channels")]
    public Optional<Dictionary<string, Channel>> Channels { get; set; }

    [JsonPropertyName("roles")]
    public Optional<Dictionary<string, Role>> Roles { get; set; }
}
