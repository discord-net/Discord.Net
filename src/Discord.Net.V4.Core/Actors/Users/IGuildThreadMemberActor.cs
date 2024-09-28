namespace Discord;

public partial interface IGuildThreadMemberActor :
    IThreadMemberActor,
    IMemberRelationship,
    IGuildRelationship
{
    [SourceOfTruth] new IGuildThreadChannelActor Thread { get; }

    [LinkExtension]
    protected new interface WithCurrentMemberExtension : IThreadMemberActor.WithCurrentMemberExtension
    {
        new IGuildThreadMemberActor Current { get; }

        IThreadMemberActor IThreadMemberActor.WithCurrentMemberExtension.Current => Current;
    }

    [LinkExtension]
    protected new interface WithPagedVariantExtension : IThreadMemberActor.WithPagedVariantExtension
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