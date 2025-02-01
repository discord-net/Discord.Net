using Newtonsoft.Json;

namespace Discord.API;

internal class ThumbnailComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }

    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("description")]
    public Optional<string> Description { get; set; }

    [JsonProperty("spoiler")]
    public Optional<bool> IsSpoiler { get; set; }

    public ThumbnailComponent() { }

    int? IMessageComponent.Id => Id.ToNullable();
}
