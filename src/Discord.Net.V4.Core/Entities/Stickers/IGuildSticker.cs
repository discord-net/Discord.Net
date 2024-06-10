using Discord.Models.Json.Stickers;

namespace Discord;

/// <summary>
///     Represents a custom sticker within a guild.
/// </summary>
public interface IGuildSticker : ISticker, IModifiable<ModifyStickerProperties, ModifyGuildStickersParams>, IDeletable
{
    /// <summary>
    ///     Gets the user that uploaded the guild sticker.
    /// </summary>
    ILoadableEntity<ulong, IGuildUser>? Author { get; }

    /// <summary>
    ///     Gets the guild that this custom sticker is in.
    /// </summary>
    ILoadableEntity<ulong, IGuild> Guild { get; }
}
