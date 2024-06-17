namespace Discord;

public interface ILoadableUserActor<TUser> :
    IUserActor,
    ILoadableEntity<ulong, TUser>
    where TUser : class, IUser;

public interface IUserActor :
    IActor<ulong, IUser>;
