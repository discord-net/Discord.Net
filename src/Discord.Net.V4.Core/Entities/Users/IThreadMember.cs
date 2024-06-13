namespace Discord;

public interface IThreadMember :
    ISnowflakeEntity,
    IThreadMemberActor<IThreadMember>
{
    DateTimeOffset JoinedAt { get; }
}
