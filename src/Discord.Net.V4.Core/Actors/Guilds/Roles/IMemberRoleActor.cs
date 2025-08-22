namespace Discord.Models;

public interface IMemberRoleActor :
    IActor<Snowflake, IRole>,
    IDeletable;