using System.Text.Json.Serialization;

namespace Discord.API;

internal class ApplicationCommandInteractionDataResolved
{
    [JsonPropertyName("users")]
    public Optional<Dictionary<string, User>> Users { get; set; }

    [JsonPropertyName("members")]
    public Optional<Dictionary<string, GuildMember>> Members { get; set; }

    [JsonPropertyName("channels")]
    public Optional<Dictionary<string, Channel>> Channels { get; set; }

    [JsonPropertyName("roles")]
    public Optional<Dictionary<string, Role>> Roles { get; set; }

    [JsonPropertyName("messages")]
    public Optional<Dictionary<string, Message>> Messages { get; set; }

    [JsonPropertyName("attachments")]
    public Optional<Dictionary<string, Attachment>> Attachments { get; set; }
}
