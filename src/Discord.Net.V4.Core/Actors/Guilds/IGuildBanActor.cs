namespace Discord;

public interface ILoadableGuildBanActor<TBan> :
    IGuildBanActor,
    ILoadableEntity<ulong, TBan>
    where TBan : class, IBan;

public interface IGuildBanActor :
    IGuildRelationship,
    IUserRelationship,
    IActor<ulong, IBan>;
