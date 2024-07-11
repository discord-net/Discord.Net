namespace Discord.Models;

[ModelEquality]
public partial interface IThreadChannelModel : IGuildChannelModel
{
    [SourceOfTruth]
    new ulong ParentId { get; }
    bool IsArchived { get; }
    int AutoArchiveDuration { get; }
    DateTimeOffset ArchiveTimestamp { get; }
    bool IsLocked { get; }
    int MemberCount { get; }
    int MessageCount { get; }
    bool? IsInvitable { get; }
    bool HasJoined { get; }
    DateTimeOffset? CreatedAt { get; }
    ulong[] AppliedTags { get; }
    ulong? OwnerId { get; }
    int RatelimitPerUser { get; }
    bool IsNsfw { get; }
    string? Topic { get; }
}
