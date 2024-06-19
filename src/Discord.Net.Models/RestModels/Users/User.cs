using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class User : IUserModel, ISelfUserModel
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("discriminator")]
    public required ushort Discriminator { get; set; }

    [JsonPropertyName("global_name")]
    public string? GlobalName { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("bot")]
    public Optional<bool> IsBot { get; set; }

    [JsonPropertyName("system")]
    public Optional<bool> IsSystem { get; set; }

    [JsonPropertyName("mfa_enabled")]
    public Optional<bool> MFAEnabled { get; set; }

    [JsonPropertyName("banner")]
    public Optional<string?> Banner { get; set; }

    [JsonPropertyName("accent_color")]
    public Optional<int?> AccentColor { get; set; }

    [JsonPropertyName("locale")]
    public Optional<string> Locale { get; set; }

    [JsonPropertyName("verified")]
    public Optional<bool> IsVerified { get; set; }

    [JsonPropertyName("email")]
    public Optional<string?> Email { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("premium_type")]
    public Optional<int> PremiumType { get; set; }

    [JsonPropertyName("public_flags")]
    public Optional<int> PublicFlags { get; set; }

    [JsonPropertyName("avatar_decoration")]
    public Optional<string?> AvatarDecoration { get; set; }

    bool? IUserModel.IsBot => IsBot;
    bool? IUserModel.IsSystem => IsSystem;
    bool? ISelfUserModel.MFAEnabled => MFAEnabled;
    string? ISelfUserModel.Locale => Locale;
    bool? ISelfUserModel.Verified => IsVerified;
    string? ISelfUserModel.Email => Email;
    int? IUserModel.Flags => Flags;
    int? ISelfUserModel.PremiumType => PremiumType;
    int? IUserModel.PublicFlags => PublicFlags;
}
