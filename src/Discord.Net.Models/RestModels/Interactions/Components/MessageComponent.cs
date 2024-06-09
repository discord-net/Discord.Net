using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(ComponentConverter))]
public class MessageComponent
{
    [JsonPropertyName("type")]
    public uint Type { get; set; }
}
