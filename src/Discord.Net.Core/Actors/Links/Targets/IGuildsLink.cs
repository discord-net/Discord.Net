using Discord.Models;

namespace Discord;

public interface IGuildsLink :
    IIndexableLink<Snowflake, IGuildActor>;