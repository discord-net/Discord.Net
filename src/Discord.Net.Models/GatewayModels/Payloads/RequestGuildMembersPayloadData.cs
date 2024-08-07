using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class RequestGuildMembersPayloadData : IRequestGuildMembersPayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("query")]
    public Optional<string> Query { get; set; }

    [JsonPropertyName("limit")]
    public Optional<int> Limit { get; set; }

    [JsonPropertyName("presences")]
    public Optional<bool> IncludePresences { get; set; }

    [JsonPropertyName("user_ids")]
    public Optional<ulong[]> UserIds { get; set; }

    [JsonPropertyName("nonce")]
    public Optional<string> Nonce { get; set; }

    string? IRequestGuildMembersPayloadData.Query => ~Query;

    int? IRequestGuildMembersPayloadData.Limit => ~Limit;

    bool? IRequestGuildMembersPayloadData.IncludePresences => ~IncludePresences;

    ulong[]? IRequestGuildMembersPayloadData.UserIds => ~UserIds;

    string? IRequestGuildMembersPayloadData.Nonce => ~Nonce;
}
