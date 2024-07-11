namespace Discord;

public interface ILoadableCategoryChannelActor :
    ICategoryChannelActor,
    ILoadableEntity<ulong, ICategoryChannel>;

public interface ICategoryChannelActor :
    IGuildChannelActor,
    IActor<ulong, ICategoryChannel>;
