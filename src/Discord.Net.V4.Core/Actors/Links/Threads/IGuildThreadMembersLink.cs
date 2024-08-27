namespace Discord;

public partial interface IGuildThreadMembersLink : 
    IThreadMembersLink,
    GuildThreadMemberLink.Enumerable.Indexable.BackLink<IGuildThreadChannelActor>
{
    [SourceOfTruth]
    new IGuildThreadMemberActor Current { get; }
}