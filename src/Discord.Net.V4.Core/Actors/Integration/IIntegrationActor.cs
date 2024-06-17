namespace Discord.Integration;

public interface IIntegrationActor :
    IGuildRelationship,
    IActor<ulong, IIntegration>;
