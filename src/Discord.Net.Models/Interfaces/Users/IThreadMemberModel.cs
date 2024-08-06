
namespace Discord.Models;

[ModelEquality]
public partial interface IThreadMemberModel : IEntityModel<ulong>
{
    ulong? ThreadId { get; }
    ulong? UserId { get; }
    DateTimeOffset JoinTimestamp { get; }
    int Flags { get; }

    ulong IEntityModel<ulong>.Id => UserId ?? 0;
}
