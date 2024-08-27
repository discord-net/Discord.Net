namespace Discord;

public interface IMembersLink : MemberLink.Paged<PageGuildMembersParams>.Indexable.BackLink<IGuildActor>
{
    ICurrentMemberActor Current { get; }
}