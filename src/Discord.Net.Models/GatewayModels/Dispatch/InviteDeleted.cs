using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class InviteDeleted : IInviteDeletedEventPayload
{
    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("code")]
    public required string Code { get; set; }

    ulong? IInviteDeletedEventPayload.GuildId => GuildId.ToNullable();
}
