using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetUserVoiceState))]
[Modifiable<ModifyUserVoiceStateProperties>(nameof(Routes.ModifyUserVoiceState))]
public partial interface IVoiceStateActor :
    IMemberActor.CanonicalRelationship,
    IActor<ulong, IVoiceState>;
