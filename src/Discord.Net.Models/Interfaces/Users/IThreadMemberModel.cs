
namespace Discord.Models;

[ModelEquality]
public partial interface IThreadMemberModel : IEntityModel<ulong>
{
    ulong? UserId { get; }
    DateTimeOffset JoinTimestamp { get; }
    int Flags { get; }
}
