using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildSoundboardSound)),
    Creatable<CreateGuildSoundboardSoundProperties>(nameof(Routes.CreateGuildSoundboardSound)),
    Modifiable<ModifyGuildSoundboardSoundProperties>(nameof(Routes.ModifyGuildSoundboardSound)),
    Deletable(nameof(Routes.DeleteGuildSoundboardSound))
]
public partial interface IGuildSoundboardSoundActor :
    ISoundboardSoundActor,
    IActor<ulong, IGuildSoundboardSound>,
    IGuildRelationship;