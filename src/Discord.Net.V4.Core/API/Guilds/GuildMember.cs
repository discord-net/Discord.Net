using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.API;

public sealed class GuildMember : IMemberModel
{
    [JsonPropertyName("user")]
    public Optional<User> User { get; set; }

    [JsonPropertyName("nick")]
    public Optional<string?> Nick { get; set; }

    [JsonPropertyName("avatar")]
    public Optional<string?> Avatar { get; set; }
    [JsonPropertyName("roles")]
    public Optional<ulong[]> RoleIds { get; set; }

    [JsonPropertyName("joined_at")]
    public Optional<DateTimeOffset> JoinedAt { get; set; }

    [JsonPropertyName("premium_since")]
    public Optional<DateTimeOffset?> PremiumSince { get; set; }

    [JsonPropertyName("deaf")]
    public Optional<bool> Deaf { get; set; }

    [JsonPropertyName("mute")]
    public Optional<bool> Mute { get; set; }

    [JsonPropertyName("flags")]
    public GuildUserFlags Flags { get; set; }

    [JsonPropertyName("pending")]
    public Optional<bool> Pending { get; set; }

    // Only returned with interaction object
    [JsonPropertyName("permissions")]
    public Optional<ChannelPermission> Permissions { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

    ulong IEntityModel<ulong>.Id => User.Value.Id;
    string? IMemberModel.GuildAvatar => ~Avatar;

    ulong[] IMemberModel.RoleIds => RoleIds ^ Array.Empty<ulong>();

    DateTimeOffset? IMemberModel.JoinedAt => JoinedAt.ToNullable();

    DateTimeOffset? IMemberModel.PremiumSince => ~PremiumSince;

    bool? IMemberModel.IsPending => Pending.ToNullable();

    string? IMemberModel.Nickname => ~Nick;
    DateTimeOffset? IMemberModel.CommunicationsDisabledUntil => ~TimedOutUntil;
}
