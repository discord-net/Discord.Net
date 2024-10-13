using Discord.Rest;

namespace Discord;

[
    Deletable(nameof(Routes.DeleteApplicationEmoji)),
    Loadable(nameof(Routes.GetApplicationEmoji)),
    Modifiable<ModifyApplicationEmoteProperties>(nameof(Routes.ModifyApplicationEmoji)),
    Creatable<CreateApplicationEmoteProperties>(
        nameof(Routes.CreateApplicationEmoji),
        nameof(IApplicationActor.Emotes)
    )
]
public partial interface IApplicationEmoteActor :
    IActor<ulong, IApplicationEmote>,
    IApplicationActor.CanonicalRelationship;