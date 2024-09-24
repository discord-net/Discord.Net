using Discord.Converters;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

[JsonConverter(typeof(DiscordErrorConverter))]
public sealed class DiscordError
{
    public required string Message { get; set; }

    public required int Code { get; set; }

    public required Dictionary<string, List<Error>> Errors { get; set; }
}
