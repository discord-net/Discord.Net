namespace Discord;

public interface IMemberRelationship : IMemberRelationship<IGuildMember>;
public interface IMemberRelationship<TMember> :
    IRelationship<ulong, TMember, ILoadableGuildMemberActor<TMember>>
    where TMember : class, IGuildMember
{
    ILoadableGuildMemberActor<TMember> Member { get; }

    ILoadableGuildMemberActor<TMember> IRelationship<ulong, TMember, ILoadableGuildMemberActor<TMember>>.RelationshipLoadable => Member;
}
