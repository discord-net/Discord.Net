using Discord.Models;
using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetGuildSticker))]
[Deletable(nameof(Routes.DeleteGuildSticker))]
[Modifiable<ModifyStickerProperties>(nameof(Routes.ModifyGuildSticker))]
public partial interface IGuildStickerActor :
    IGuildRelationship,
    IActor<ulong, IGuildSticker>;
