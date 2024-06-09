using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InteractionDataResolved
{
    [JsonPropertyName("users")]
    public Optional<Dictionary<string, User>> Users { get; set; }

    [JsonPropertyName("members")]
    public Optional<Dictionary<string, GuildMember>> Members { get; set; }

    [JsonPropertyName("roles")]
    public Optional<Dictionary<string, Role>> Roles { get; set; }

    [JsonPropertyName("channels")]
    public Optional<Dictionary<string, Channel>> Channels { get; set; }

    [JsonPropertyName("messages")]
    public Optional<Dictionary<string, Message>> Messages { get; set; }

    [JsonPropertyName("attachments")]
    public Optional<Dictionary<string, Attachment>> Attachments { get; set; }
}
