using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildOnboardings
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("prompts")]
    public required GuildOnboardingPrompt[] Prompts { get; set; }

    [JsonPropertyName("default_channel_ids")]
    public required ulong[] DefaultChannelIds { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("mode")]
    public required int Mode { get; set; }
}
