namespace Discord;

public partial interface IGuildThreadMemberActor :
    IThreadMemberActor,
    IMemberRelationship,
    IGuildRelationship
{
    [SourceOfTruth]
    new IGuildThreadChannelActor Thread { get; }
}