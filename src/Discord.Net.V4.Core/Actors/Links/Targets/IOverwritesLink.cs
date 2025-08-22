using Discord.Models;

namespace Discord;

public interface IOverwritesLink :
    IIndexableLink<Snowflake, IOverwriteActor>;