namespace Discord.EntityRelationships;

public interface IUserRelationship : IUserRelationship<IUser>;
public interface IUserRelationship<TUser> : IRelationship<ulong, TUser>
    where TUser : class, IUser
{
    ILoadableEntity<ulong, TUser> User { get; }
}
