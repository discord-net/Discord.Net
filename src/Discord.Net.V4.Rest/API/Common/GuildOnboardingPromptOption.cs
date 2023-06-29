using System.Text.Json.Serialization;

namespace Discord.API;

internal class GuildOnboardingPromptOption
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("channel_ids")]
    public ulong[] ChannelIds { get; set; }

    [JsonPropertyName("role_ids")]
    public ulong[] RoleIds { get; set; }

    [JsonPropertyName("emoji")]
    public Emoji Emoji { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
