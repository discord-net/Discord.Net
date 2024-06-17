using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

public interface ILoadableGuildStickerActor<TSticker> :
    IGuildStickerActor,
    ILoadableEntity<ulong, TSticker>
    where TSticker : class, IGuildSticker;

public interface IGuildStickerActor :
    IGuildRelationship,
    IModifiable<ulong, IGuildStickerActor, ModifyStickerProperties, ModifyGuildStickersParams>,
    IDeletable<ulong, IGuildStickerActor>,
    IActor<ulong, IGuildSticker>
{
    static BasicApiRoute IDeletable<ulong, IGuildStickerActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static ApiBodyRoute<ModifyGuildStickersParams> IModifiable<ulong, IGuildStickerActor, ModifyStickerProperties, ModifyGuildStickersParams>.ModifyRoute(IPathable path, ulong id,
        ModifyGuildStickersParams args)
        => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);
}
