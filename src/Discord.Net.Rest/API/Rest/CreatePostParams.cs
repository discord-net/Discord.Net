using Newtonsoft.Json;

namespace Discord.API.Rest;

internal class CreatePostParams
{
    // thread
    [JsonProperty("name")]
    public string Title { get; set; }

    [JsonProperty("auto_archive_duration")]
    public ThreadArchiveDuration ArchiveDuration { get; set; }

    [JsonProperty("rate_limit_per_user")]
    public Optional<int?> Slowmode { get; set; }

    [JsonProperty("message")]
    public ForumThreadMessage Message { get; set; }

    [JsonProperty("applied_tags")]
    public Optional<ulong[]> Tags { get; set; }
}
