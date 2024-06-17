namespace Discord;

public interface IThreadMember :
    ISnowflakeEntity,
    IThreadMemberActor
{
    DateTimeOffset JoinedAt { get; }
}
