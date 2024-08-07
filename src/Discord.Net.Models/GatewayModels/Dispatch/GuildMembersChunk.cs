namespace Discord.Models.Json;

public sealed class GuildMembersChunk : IGuildMembersChunkPayloadData
{
    public ulong GuildId { get; set; }
    public required GuildMember[] Members { get; set; }
    public int ChunkIndex { get; set; }
    public int ChunkCount { get; set; }
    public Optional<ulong[]> NotFound { get; set; }
    public Optional<Presence[]> Presences { get; set; }
    public Optional<string> Nonce { get; set; }

    IEnumerable<IMemberModel> IGuildMembersChunkPayloadData.Members => Members;
    ulong[]? IGuildMembersChunkPayloadData.NotFound => ~NotFound;
    IEnumerable<IPresenceModel>? IGuildMembersChunkPayloadData.Presences => ~Presences;
    string? IGuildMembersChunkPayloadData.Nonce => ~Nonce;
}
