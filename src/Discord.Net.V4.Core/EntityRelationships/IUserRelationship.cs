namespace Discord.EntityRelationships;

public interface IUserRelationship : IUserRelationship<IUser>;
public interface IUserRelationship<TUser> : ILoadableRelationship<ulong, TUser>
    where TUser : class, IUser
{
    ILoadableEntity<ulong, TUser> User { get; }
}
