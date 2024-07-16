using Discord.Models;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetInvite))]
[Deletable(nameof(Routes.DeleteInvite))]
public partial interface IInviteActor :
    IActor<string, IInvite>;

