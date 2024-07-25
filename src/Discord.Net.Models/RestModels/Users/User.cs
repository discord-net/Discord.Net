using Discord.Converters;
using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public class User : IUserModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("discriminator")]
    public required string Discriminator { get; set; }

    [JsonPropertyName("global_name")]
    public string? GlobalName { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("bot")]
    public Optional<bool> IsBot { get; set; }

    [JsonPropertyName("system")]
    public Optional<bool> IsSystem { get; set; }

    [JsonPropertyName("banner")]
    public Optional<string?> Banner { get; set; }

    [JsonPropertyName("accent_color")]
    public Optional<int?> AccentColor { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("public_flags")]
    public Optional<int> PublicFlags { get; set; }

    [JsonPropertyName("avatar_decoration")]
    public Optional<string?> AvatarDecoration { get; set; }
    bool? IUserModel.IsBot => ~IsBot;
    bool? IUserModel.IsSystem => ~IsSystem;
    int? IUserModel.Flags => ~Flags;
    int? IUserModel.PublicFlags => ~PublicFlags;
}
