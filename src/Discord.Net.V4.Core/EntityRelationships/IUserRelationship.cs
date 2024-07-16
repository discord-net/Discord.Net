namespace Discord;

public interface IUserRelationship :
    IRelationship<IUserActor, ulong, IUser>
{
    IUserActor User { get; }

    IUserActor IRelationship<IUserActor, ulong, IUser>.RelationshipActor => User;
}
