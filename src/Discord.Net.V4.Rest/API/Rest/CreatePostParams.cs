using Discord.Entities.Channels.Threads;
using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class CreatePostParams
{
    // thread
    [JsonPropertyName("name")]
    public string Title { get; set; }

    [JsonPropertyName("auto_archive_duration")]
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    [JsonPropertyName("rate_limit_per_user")]
    public Optional<int?> Slowmode { get; set; }

    [JsonPropertyName("message")]
    public ForumThreadMessage Message { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> Tags { get; set; }
}
