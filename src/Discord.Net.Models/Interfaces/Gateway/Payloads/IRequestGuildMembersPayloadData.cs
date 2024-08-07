namespace Discord.Models;

public interface IRequestGuildMembersPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    string? Query { get; }
    int? Limit { get; }
    bool? IncludePresences { get; }
    ulong[]? UserIds { get; }
    string? Nonce { get; }
}
