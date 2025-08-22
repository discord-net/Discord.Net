namespace Discord.Models;

public interface IGuildIntegrationsLink :
    IIndexableLink<Snowflake, IIntegrationActor>,
    IBatchLink<IIntegration>;