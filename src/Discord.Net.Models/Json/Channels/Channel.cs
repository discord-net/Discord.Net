using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[DiscriminatedUnionRootType(nameof(Type))]
public class Channel : IChannelModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public uint Type { get; set; }
}
