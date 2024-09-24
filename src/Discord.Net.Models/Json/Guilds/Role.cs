using System.Numerics;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Role : IRoleModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("color")]
    public uint Color { get; set; }

    [JsonPropertyName("hoist")]
    public bool Hoist { get; set; }

    [JsonPropertyName("icon")]
    public Optional<string?> Icon { get; set; }

    [JsonPropertyName("unicode_emoji")]
    public Optional<string?> UnicodeEmoji { get; set; }

    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("permissions")]
    public BigInteger Permissions { get; set; }

    [JsonPropertyName("managed")]
    public bool Managed { get; set; }

    [JsonPropertyName("mentionable")]
    public bool Mentionable { get; set; }

    [JsonPropertyName("tags")]
    public Optional<RoleTags> Tags { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    bool IRoleModel.IsHoisted => Hoist;

    string? IRoleModel.Icon => ~Icon;

    string? IRoleModel.UnicodeEmoji => ~UnicodeEmoji;

    bool IRoleModel.IsManaged => Managed;

    bool IRoleModel.IsMentionable => Mentionable;

    int IRoleModel.Flags => Flags;

    IRoleTagsModel? IRoleModel.Tags => ~Tags;
}
