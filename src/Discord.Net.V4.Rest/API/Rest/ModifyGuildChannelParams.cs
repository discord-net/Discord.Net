using System.Text.Json.Serialization;

namespace Discord.API.Rest;

internal class ModifyGuildChannelParams
{
    [JsonPropertyName("name")]
    public Optional<string> Name { get; set; }

    [JsonPropertyName("position")]
    public Optional<int?> Position { get; set; }

    [JsonPropertyName("parent_id")]
    public Optional<ulong?> CategoryId { get; set; }

    [JsonPropertyName("permission_overwrites")]
    public Optional<Overwrite[]?> Overwrites { get; set; }

    [JsonPropertyName("flags")]
    public Optional<ChannelFlags?> Flags { get; set; }

    [JsonPropertyName("nsfw")]
    public Optional<bool?> IsNsfw { get; set; }
}
