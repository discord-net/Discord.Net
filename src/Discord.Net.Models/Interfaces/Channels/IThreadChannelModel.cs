namespace Discord.Models;

public interface IThreadChannelModel : IGuildTextChannelModel
{
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
}
