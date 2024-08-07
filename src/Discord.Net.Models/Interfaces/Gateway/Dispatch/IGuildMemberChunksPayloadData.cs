namespace Discord.Models;

public interface IGuildMembersChunkPayloadData : IGatewayPayloadData
{
    ulong GuildId { get; }
    IEnumerable<IMemberModel> Members { get; }
    int ChunkIndex { get; }
    int ChunkCount { get; }
    ulong[]? NotFound { get; }
    IEnumerable<IPresenceModel>? Presences { get; }
    string? Nonce { get; }
}
