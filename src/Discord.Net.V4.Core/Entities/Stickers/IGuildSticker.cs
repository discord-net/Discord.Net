using Discord.Models.Json.Stickers;
using Discord.Rest;

namespace Discord;

using Deletable = IDeletable<ulong, IGuildSticker>;
using Modifiable = IModifiable<ulong, IGuildSticker, ModifyStickerProperties, ModifyGuildStickersParams>;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker :
    ISticker,
    Modifiable,
    Deletable
{
    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableEntity<ulong, IGuildUser>? Author { get; }

    /// <summary>
    ///     Gets the guild that this custom sticker is in.
    /// </summary>
    ILoadableEntity<ulong, IGuild> Guild { get; }

    static BasicApiRoute Deletable.DeleteRoute(IPathable path, ulong id)
        => Routes.DeleteMessage(path.Require<IChannel>(), id);

    static ApiBodyRoute<ModifyGuildStickersParams> Modifiable.ModifyRoute(IPathable path, ulong id,
        ModifyGuildStickersParams args)
        => Routes.ModifyGuildSticker(path.Require<IGuild>(), id, args);
}
