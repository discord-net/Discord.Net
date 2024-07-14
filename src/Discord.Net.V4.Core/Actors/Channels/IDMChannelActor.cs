using Discord.Rest;

namespace Discord;

public interface ILoadableDMChannelActor :
    IDMChannelActor,
    ILoadableEntity<ulong, IDMChannel>;

[Deletable(nameof(Routes.DeleteChannel))]
public partial interface IDMChannelActor :
    IMessageChannelActor,
    IActor<ulong, IDMChannel>;
