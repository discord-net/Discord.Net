namespace Discord;

public partial interface IGuildThreadMemberActor :
    IThreadMemberActor,
    IMemberRelationship,
    IGuildRelationship
{
    [SourceOfTruth] new IGuildThreadChannelActor Thread { get; }

    [LinkExtension]
    private interface WithCurrentMemberExtension : IThreadMemberActor.WithCurrentMemberExtension
    {
        new IGuildThreadMemberActor Current { get; }

        IThreadMemberActor IThreadMemberActor.WithCurrentMemberExtension.Current => Current;
    }

    [LinkExtension]
    private interface WithPagedVariantExtension : IThreadMemberActor.WithPagedVariantExtension
    {
        new IGuildThreadMemberActor.Paged<PageThreadMembersParams>.Indexable AsPaged { get; }

        IThreadMemberActor
            .Paged<PageThreadMembersParams>
            .Indexable
            IThreadMemberActor
            .WithPagedVariantExtension
            .AsPaged => AsPaged;
    }
}