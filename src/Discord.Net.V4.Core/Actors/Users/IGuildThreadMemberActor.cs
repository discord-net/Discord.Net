namespace Discord;

public partial interface IGuildThreadMemberActor :
    IThreadMemberActor,
    IMemberRelationship,
    IGuildRelationship
{
    [SourceOfTruth]
    new IGuildThreadChannelActor Thread { get; }

    [LinkExtension]
    private interface WithCurrentMemberExtension : IThreadMemberActor.WithCurrentMemberExtension
    {
        new IGuildThreadMemberActor Current { get; }
        IThreadMemberActor IThreadMemberActor.WithCurrentMemberExtension.Current => Current;
    }
}