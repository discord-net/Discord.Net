using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(IGuildEmoteModel.Id))]
[HasPartialVariant]
public abstract class Emote : IEmoteModel
{
    [JsonPropertyName("name"), NullableInPartial]
    public required string Name { get; set; }
}