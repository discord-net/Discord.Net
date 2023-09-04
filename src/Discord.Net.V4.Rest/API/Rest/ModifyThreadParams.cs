using Discord.Entities.Channels.Threads;
using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyThreadParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("archived")]
    public Optional<bool> IsArchived { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("locked")]
    public Optional<bool> IsLocked { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> SlowmodeInterval { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags> Flags { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> AppliedTags { get; set; }
}
