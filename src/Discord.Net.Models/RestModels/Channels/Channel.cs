using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(ChannelConverter))]
public class Channel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public uint Type { get; set; }
}
