
namespace Discord.Models;

public interface IThreadMemberModel : IEntityModel<ulong?>
{
    ulong? UserId { get; }
    DateTimeOffset JoinTimestamp { get; }
    int Flags { get; }
    
}
