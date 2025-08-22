using Discord.Models;

namespace Discord;

public interface IMemberRoleActor :
    IActor<Snowflake, IRole>,
    IDeletable;