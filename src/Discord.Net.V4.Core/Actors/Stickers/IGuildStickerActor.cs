using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

[Loadable(nameof(Routes.GetGuildSticker))]
[Deletable(nameof(Routes.DeleteGuildSticker))]
[Modifiable<ModifyStickerProperties>(nameof(Routes.ModifyGuildSticker))]
[Creatable<CreateGuildStickerProperties>(nameof(Routes.CreateGuildSticker))]
[SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
public partial interface IGuildStickerActor :
    IGuildRelationship,
    IStickerActor,
    IActor<ulong, IGuildSticker>;
