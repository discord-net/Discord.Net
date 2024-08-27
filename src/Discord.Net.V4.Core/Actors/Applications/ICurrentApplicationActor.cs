using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetCurrentApplication)),
    Modifiable<ModifyCurrentApplicationProperties>(nameof(Routes.ModifyCurrentApplication))
]
public partial interface ICurrentApplicationActor : 
    IApplicationActor,
    IActor<ulong, ICurrentApplication>;