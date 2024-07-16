namespace Discord;

public interface IThreadRelationship :
    IRelationship<IThreadChannelActor, ulong, IThreadChannel>
{
    IThreadChannelActor Thread { get; }

    IThreadChannelActor IRelationship<IThreadChannelActor, ulong, IThreadChannel>.RelationshipActor => Thread;
}
