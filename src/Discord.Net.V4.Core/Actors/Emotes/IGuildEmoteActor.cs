using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildEmoji)), Deletable(nameof(Routes.DeleteGuildEmoji)),
    Creatable<CreateGuildEmoteProperties>(nameof(Routes.CreateGuildEmoji), nameof(IGuildActor.Emotes)),
    Modifiable<ModifyGuildEmoteProperties>(nameof(Routes.ModifyGuildEmoji))
]
public partial interface IGuildEmoteActor :
    IActor<ulong, IGuildEmote>,
    IGuildActor.CanonicalRelationship;