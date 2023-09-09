using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.API;

public class GuildMember : IMemberModel, IUserModel
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
    public Optional<GuildPermission> Permissions { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

    string? IMemberModel.Nickname => Nick.GetValueOrDefault();

    string? IMemberModel.GuildAvatar => Avatar.GetValueOrDefault();

    ulong[] IMemberModel.RoleIds => RoleIds ^ Array.Empty<ulong>();

    DateTimeOffset? IMemberModel.JoinedAt => JoinedAt.ToNullable();

    DateTimeOffset? IMemberModel.PremiumSince => PremiumSince.GetValueOrDefault();

    bool? IMemberModel.IsPending => Pending.ToNullable();

    DateTimeOffset? IMemberModel.CommunicationsDisabledUntil => TimedOutUntil.GetValueOrDefault();

    ulong IEntityModel<ulong>.Id => User.GetValueOrDefault()?.Id ?? 0; // only null for message create

    string IUserModel.Username => throw new NotImplementedException();

    string IUserModel.Discriminator => throw new NotImplementedException();

    string? IUserModel.GlobalName => throw new NotImplementedException();

    string? IUserModel.AvatarHash => throw new NotImplementedException();

    bool? IUserModel.IsBot => throw new NotImplementedException();

    bool? IUserModel.IsSystem => throw new NotImplementedException();

    bool? IUserModel.MFAEnabled => throw new NotImplementedException();

    string? IUserModel.Locale => throw new NotImplementedException();

    bool? IUserModel.Verified => throw new NotImplementedException();

    string? IUserModel.Email => throw new NotImplementedException();

    UserFlags? IUserModel.Flags => throw new NotImplementedException();

    PremiumType? IUserModel.Premium => throw new NotImplementedException();

    UserFlags? IUserModel.PublicFlags => throw new NotImplementedException();
}
