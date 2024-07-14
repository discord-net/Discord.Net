using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildEmoteActor :
    IGuildEmoteActor,
    ILoadableEntity<ulong, IGuildEmote>;

[Deletable(nameof(Routes.DeleteGuildEmoji))]
[Modifiable<EmoteProperties>(nameof(Routes.ModifyGuildEmoji))]
public partial interface IGuildEmoteActor :
    IActor<ulong, IGuildEmote>,
    IGuildRelationship;
