using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface ILoadableBanActor :
    IBanActor,
    ILoadableEntity<ulong, IBan>;

[Deletable(nameof(Routes.RemoveGuildBan))]
public partial interface IBanActor :
    IGuildRelationship,
    IUserRelationship,
    IActor<ulong, IBan>;
