using Newtonsoft.Json;

namespace Discord.API;

internal class FileComponent : IMessageComponent
{
    [JsonProperty("type")]
    public ComponentType Type { get; set; }
    [JsonProperty("id")]
    public Optional<int> Id { get; set; }

    [JsonProperty("file")]
    public UnfurledMediaItem File { get; set; }

    [JsonProperty("spoiler")]
    public Optional<bool> IsSpoiler { get; set; }

    public FileComponent() { }

    int? IMessageComponent.Id => Id.ToNullable();
}
