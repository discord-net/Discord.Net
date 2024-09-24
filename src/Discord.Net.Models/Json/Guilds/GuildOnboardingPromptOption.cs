using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildOnboardingPromptOption
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("channel_ids")]
    public required ulong[] ChannelIds { get; set; }

    [JsonPropertyName("role_ids")]
    public required ulong[] RoleIds { get; set; }

    [JsonPropertyName("emoji")]
    public IEmoteModel? Emoji { get; set; }

    [JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
