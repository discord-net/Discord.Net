using Newtonsoft.Json;

namespace Discord.API;

internal class MediaGalleryItem
{
    [JsonProperty("media")]
    public UnfurledMediaItem Media { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("spoiler")]
    public Optional<bool> IsSpoiler { get; set; }
}
