using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildStickerActor<TSticker> :
    IGuildStickerActor<TSticker>,
    ILoadableEntity<ulong, TSticker>
    where TSticker : class, IGuildSticker;

public interface IGuildStickerActor<out TSticker> :
    IGuildRelationship,
    IModifiable<ulong, IGuildStickerActor<TSticker>, ModifyStickerProperties, ModifyGuildStickersParams>,
    IDeletable<ulong, IGuildStickerActor<TSticker>>,
    IActor<ulong, TSticker>
    where TSticker : IGuildSticker
{
    static BasicApiRoute IDeletable<ulong, IGuildStickerActor<TSticker>>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static ApiBodyRoute<ModifyGuildStickersParams> IModifiable<ulong, IGuildStickerActor<TSticker>, ModifyStickerProperties, ModifyGuildStickersParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildStickersParams args)
        => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);
}
