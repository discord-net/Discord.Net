using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IOverwriteActor :
    IActor<Snowflake, IOverwrite>,
    IDeletable,
    IModifiable<IModifyOverwriteParams>;