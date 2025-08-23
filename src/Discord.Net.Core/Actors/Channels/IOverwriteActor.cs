using Discord.Models;

namespace Discord;

public interface IOverwriteActor :
    IActor<Snowflake, IOverwrite>,
    IDeletable,
    IModifiable<IModifyOverwriteParams>;