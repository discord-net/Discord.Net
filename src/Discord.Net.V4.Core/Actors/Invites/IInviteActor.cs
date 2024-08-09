using Discord.Models;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetInvite))]
public partial interface IInviteActor :
    IActor<string, IInvite>;

