using Discord.Rest;
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

    public FileComponent(Discord.FileComponent component)
    {
        Type = component.Type;
        Id = component.Id ?? Optional<int>.Unspecified;
        File = component.File.ToModel();
        IsSpoiler = component.IsSpoiler ?? Optional<bool>.Unspecified;
    }

    int? IMessageComponent.Id => Id.ToNullable();
    IMessageComponentBuilder IMessageComponent.ToBuilder() => null;
}
