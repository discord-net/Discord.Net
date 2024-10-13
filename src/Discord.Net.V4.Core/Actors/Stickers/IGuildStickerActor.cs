using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[
    Loadable(nameof(Routes.GetGuildSticker)), Deletable(nameof(Routes.DeleteGuildSticker)),
    Modifiable<ModifyStickerProperties>(nameof(Routes.ModifyGuildSticker)),
    Creatable<CreateGuildStickerProperties>(
        nameof(Routes.CreateGuildSticker),
        nameof(IGuildActor.Stickers)
    ),
    SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")
]
public partial interface IGuildStickerActor :
    IGuildActor.CanonicalRelationship,
    IStickerActor,
    IActor<ulong, IGuildSticker>;