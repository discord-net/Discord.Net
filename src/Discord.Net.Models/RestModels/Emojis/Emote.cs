using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(IGuildEmoteModel.Id))]
[HasPartialVariant]
public abstract partial class Emote : IEmoteModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
}