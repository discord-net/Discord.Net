using System.Text.Json.Serialization;

namespace Discord.API;

public class User
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    public string Username { get; set; }

    public string Discriminator { get; set; }

    public string? GlobalName { get; set; }

    public string? AvatarHash { get; set; }

    public bool? IsBot { get; set; }

    public bool? IsSystem { get; set; }

    public bool? MFAEnabled { get; set; }

    public string? Locale { get; set; }

    public bool? Verified { get; set; }

    public string? Email { get; set; }

    public UserProperties? Flags { get; set; }

    public PremiumType? Premium { get; set; }

    public UserProperties? PublicFlags { get; set; }
}
