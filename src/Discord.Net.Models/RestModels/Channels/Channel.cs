using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(ChannelConverter))]
public class Channel : IChannelModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }
}
