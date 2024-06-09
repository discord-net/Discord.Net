namespace Discord.Models;

public interface IThreadChannelModel : IGuildTextChannelModel
{
    bool HasJoined { get; }
    bool IsArchived { get; }
    int AutoArchiveDuration { get; }
    DateTimeOffset ArchiveTimestamp { get; }
    bool IsLocked { get; }
    int MemberCount { get; }
    int MessageCount { get; }
    bool IsInvitable { get; }
    DateTimeOffset? CreatedAt { get; }
}
