using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InviteCreated : IInviteCreatedPayloadData
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("code")]
    public required string Code { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("inviter")]
    public Optional<User> Inviter { get; set; }

    [JsonPropertyName("max_age")]
    public int MaxAge { get; set; }

    [JsonPropertyName("max_uses")]
    public int MaxUses { get; set; }

    [JsonPropertyName("target_type")]
    public Optional<int> TargetType { get; set; }

    [JsonPropertyName("target_user")]
    public Optional<User> TargetUser { get; set; }

    [JsonPropertyName("target_application")]
    public Optional<PartialApplication> TargetApplication { get; set; }

    [JsonPropertyName("temporary")]
    public bool IsTemporary { get; set; }

    [JsonPropertyName("uses")]
    public int Uses { get; set; }

    ulong? IInviteCreatedPayloadData.GuildId => GuildId.ToNullable();

    IUserModel? IInviteCreatedPayloadData.Inviter => ~Inviter;

    int? IInviteCreatedPayloadData.TargetType => TargetType.ToNullable();

    IUserModel? IInviteCreatedPayloadData.TargetUser => ~TargetUser;

    IPartialApplicationModel? IInviteCreatedPayloadData.TargetApplication => ~TargetApplication;
}
