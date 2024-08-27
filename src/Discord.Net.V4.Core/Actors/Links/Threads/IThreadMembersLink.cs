namespace Discord;

public interface IThreadMembersLink : ThreadMemberLink.Enumerable.Indexable.BackLink<IThreadChannelActor>
{
    IThreadMemberActor Current { get; }
}