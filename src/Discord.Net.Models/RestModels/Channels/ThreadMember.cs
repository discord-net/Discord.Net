using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ThreadMember : IThreadMemberModel
{
    [JsonPropertyName("id")]
    public Optional<ulong> Id { get; set; }

    [JsonPropertyName("user_id")]
    public Optional<ulong> UserId { get; set; }

    [JsonPropertyName("join_timestamp")]
    public DateTimeOffset JoinTimestamp { get; set; }

    [JsonPropertyName("flags")]
    public int Flags { get; set; } // No enum type (yet?)

    [JsonPropertyName("member")]
    public Optional<GuildMember> GuildMember { get; set; }

    ulong? IThreadMemberModel.UserId => UserId;
    ulong? IEntityModel<ulong?>.Id => Id;
}
