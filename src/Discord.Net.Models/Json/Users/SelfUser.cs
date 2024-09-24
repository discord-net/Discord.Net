using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class SelfUser : User, ISelfUserModel
{
    [JsonPropertyName("premium_type")]
    public Optional<int> PremiumType { get; set; }

    [JsonPropertyName("mfa_enabled")]
    public Optional<bool> MFAEnabled { get; set; }

    [JsonPropertyName("email")]
    public Optional<string?> Email { get; set; }

    [JsonPropertyName("locale")]
    public Optional<string> Locale { get; set; }

    [JsonPropertyName("verified")]
    public Optional<bool> IsVerified { get; set; }

    bool? ISelfUserModel.MFAEnabled => ~MFAEnabled;
    string? ISelfUserModel.Locale => ~Locale;
    bool? ISelfUserModel.Verified => ~IsVerified;
    string? ISelfUserModel.Email => ~Email;
    int? ISelfUserModel.PremiumType => ~PremiumType;

}
