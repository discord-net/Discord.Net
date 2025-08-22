using Discord.Models;

namespace Discord;

public interface IGuildIntegrationsLink :
    IIndexableLink<Snowflake, IIntegrationActor>,
    IBatchLink<IIntegration>;