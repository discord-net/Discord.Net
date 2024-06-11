using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(ComponentConverter))]
public class MessageComponent
{
    [JsonPropertyName("type")]
    public required uint Type { get; set; }
}
