using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildMemberUpdated : IGuildMemberUpdatedPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("roles")]
    public required ulong[] Roles { get; set; }

    [JsonPropertyName("user")]
    public required User User { get; set; }

    [JsonPropertyName("nick")]
    public Optional<string?> Nickname { get; set; }

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }

    [JsonPropertyName("joined_at")]
    public DateTimeOffset? JoinedAt { get; set; }

    [JsonPropertyName("premium_since")]
    public Optional<DateTimeOffset?> PremiumSince { get; set; }

    [JsonPropertyName("deaf")]
    public Optional<bool> IsDeaf { get; set; }

    [JsonPropertyName("mute")]
    public Optional<bool> IsMute { get; set; }

    [JsonPropertyName("pending")]
    public Optional<bool> IsPending { get; set; }

    [JsonPropertyName("communications_disabled_until")]
    public Optional<DateTimeOffset?> CommunicationsDisabledUntil { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    public Optional<AvatarDecorationData?> AvatarDecoration { get; set; }

    IUserModel IGuildMemberUpdatedPayloadData.User => User;

    string? IGuildMemberUpdatedPayloadData.Nickname => ~Nickname;

    DateTimeOffset? IGuildMemberUpdatedPayloadData.PremiumSince => ~PremiumSince;

    bool? IGuildMemberUpdatedPayloadData.IsDeaf => IsDeaf.ToNullable();

    bool? IGuildMemberUpdatedPayloadData.IsMute => IsMute.ToNullable();

    bool? IGuildMemberUpdatedPayloadData.IsPending => IsPending.ToNullable();

    DateTimeOffset? IGuildMemberUpdatedPayloadData.CommunicationsDisabledUntil => ~CommunicationsDisabledUntil;

    int? IGuildMemberUpdatedPayloadData.Flags => Flags.ToNullable();

    IAvatarDecorationDataModel? IGuildMemberUpdatedPayloadData.AvatarDecoration => ~AvatarDecoration;
}
