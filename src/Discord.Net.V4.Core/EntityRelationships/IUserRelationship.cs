namespace Discord;

public interface IUserRelationship : IRelationship<ulong, IUser, ILoadableUserActor>
{
    ILoadableUserActor User { get; }

    ILoadableUserActor IRelationship<ulong, IUser, ILoadableUserActor>.RelationshipLoadable => User;
}
