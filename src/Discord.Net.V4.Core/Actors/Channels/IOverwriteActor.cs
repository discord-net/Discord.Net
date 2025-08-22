using Discord.Models.Models;

namespace Discord.Models;

public interface IOverwriteActor :
    IActor<Snowflake, IOverwrite>,
    IDeletable,
    IModifiable<IModifyOverwriteParams>;