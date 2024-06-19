namespace Discord;

public interface ILoadableGuildBanActor :
    IGuildBanActor,
    ILoadableEntity<ulong, IBan>;

public interface IGuildBanActor :
    IGuildRelationship,
    IUserRelationship,
    IActor<ulong, IBan>;
