namespace Discord;

public interface IUserRelationship : IUserRelationship<IUser>;
public interface IUserRelationship<TUser> : IRelationship<ulong, TUser, ILoadableUserActor<TUser>>
    where TUser : class, IUser
{
    ILoadableUserActor<TUser> User { get; }

    ILoadableUserActor<TUser> IRelationship<ulong, TUser, ILoadableUserActor<TUser>>.RelationshipLoadable => User;
}
