using Discord.Models;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetGuildBan))]
[Deletable(nameof(Routes.RemoveGuildBan))]
public partial interface IBanActor :
    IGuildRelationship,
    IUserRelationship,
    IActor<ulong, IBan>;

