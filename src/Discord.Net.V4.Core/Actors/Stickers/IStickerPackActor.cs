using Discord.Rest;

namespace Discord;

[Loadable(nameof(Routes.GetStickerPack))]
public partial interface IStickerPackActor :
    IActor<ulong, IStickerPack>;
