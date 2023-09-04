using System.Text.Json.Serialization;

namespace Discord.API;

public class ThreadMetadata
{
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public ThreadArchiveDuration AutoArchiveDuration { get; set; }

    [JsonPropertyName("archive_timestamp")]
    public DateTimeOffset ArchiveTimestamp { get; set; }

    [JsonPropertyName("locked")]
    public Optional<bool> Locked { get; set; }

    [JsonPropertyName("invitable")]
    public Optional<bool> Invitable { get; set; }

    [JsonPropertyName("create_timestamp")]
    public Optional<DateTimeOffset?> CreatedAt { get; set; }
}
