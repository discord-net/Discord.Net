namespace Discord;

public interface IReactionUsersLink : 
    IIndexableLink<Snowflake, IReactionUserActor>,
    ILoadable<IReadOnlyList<IUser>>;