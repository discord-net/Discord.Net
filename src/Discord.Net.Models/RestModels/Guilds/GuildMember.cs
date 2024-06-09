using Discord.Models;
using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class GuildMember : IMemberModel, IEntityModelSource
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
    public int Flags { get; set; }

    [JsonPropertyName("pending")]
    public Optional<bool> Pending { get; set; }

    // Only returned with interaction object
    [JsonPropertyName("permissions")]
    public Optional<ulong> Permissions { get; set; }

    [JsonPropertyName("communication_disabled_until")]
    public Optional<DateTimeOffset?> CommunicationsDisabledUntil { get; set; }

    string? IMemberModel.Avatar => Avatar;

    ulong[] IMemberModel.RoleIds => RoleIds | [];
    DateTimeOffset? IMemberModel.JoinedAt => JoinedAt;
    DateTimeOffset? IMemberModel.PremiumSince => PremiumSince;

    bool? IMemberModel.IsPending => Pending;

    DateTimeOffset? IMemberModel.CommunicationsDisabledUntil => CommunicationsDisabledUntil;
    string? IMemberModel.Nickname => Nick;
    ulong? IEntityModel<ulong?>.Id => User.Map(v => v.Id);

    public IEnumerable<IEntityModel> GetEntities()
    {
        if (User.IsSpecified)
            yield return User.Value;
    }
}