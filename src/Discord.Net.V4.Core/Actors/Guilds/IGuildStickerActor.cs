using Discord.Models;
using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

using IModifiable =
    IModifiable<ulong, IGuildStickerActor, ModifyStickerProperties, ModifyGuildStickersParams, IGuildSticker,
        IStickerModel>;

public interface ILoadableGuildStickerActor :
    IGuildStickerActor,
    ILoadableEntity<ulong, IGuildSticker>;

public interface IGuildStickerActor :
    IGuildRelationship,
    IModifiable,
    IDeletable<ulong, IGuildStickerActor>,
    IActor<ulong, IGuildSticker>
{
    static IApiRoute IDeletable<ulong, IGuildStickerActor>.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static IApiInOutRoute<ModifyGuildStickersParams, IEntityModel> IModifiable.ModifyRoute(
        IPathable path,
        ulong id,
        ModifyGuildStickersParams args
    ) => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);
}
