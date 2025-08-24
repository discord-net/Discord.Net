namespace Discord;

public interface IReactionUserActor :
    IActor<Snowflake, IUser>,
    IDeletable;